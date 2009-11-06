using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace seanfoy.mvcutils {
    public static class MvcUtils {
        /// <see cref="addMvcRoute" />
        /// <see cref="Potpourri.MvcMangle" />
        public static string [] aliases = new string [] {".mvc", ".aspx"};

        /// <remarks>
        /// Extensions on urls can break the encapsulation
        /// of a resource on the web. See http://www.w3.org/Provider/Style/URI
        /// But IIS6 is built around file extensions, so the convention
        /// is to either extend it with ISAPI to fix that or to use .mvc.
        /// In addition to supporting Cool URIs that can not change as we
        /// move from asp.net to asp or jsp to asp.net mvc to the next great
        /// thing, I'll also support the usual .mvc hack and a local .aspx
        /// hack that allows for zero configuration changes on servers.
        /// </remarks>
        public static void addMvcRoute(RouteCollection routes, string url, object defaults, object constraints) {
            try {
                foreach (string u in
                         Enumerable.Select(
                             Enumerable.Union(
                                 new string [] {String.Empty},
                                 aliases),
                             i => url + i)) {
                    routes.MapRoute(
                        u,
                        u,
                        defaults,
                        constraints);
                }
            }
            catch (ArgumentException e) {
                if (!e.Message.Contains("catch-all")) throw;
                // don't append extensions to catch-all routes
                // (aside from the zero-length extension,
                // which we've already mapped).
            }
        }
        public static void addMvcRoute(string url, object defaults, object constraints) {
            addMvcRoute(RouteTable.Routes, url, defaults, constraints);
        }
        public static void addMvcRoute(string url, object defaults) {
            addMvcRoute(url, defaults, null);
        }
        public static readonly object _aliasPatternLock = new object();
        public static volatile Regex _aliasPattern;
        public static Regex aliasPattern {
            get {
                if (_aliasPattern == null) {
                    lock(_aliasPatternLock) {
                        if (_aliasPattern == null) {
                            _aliasPattern =
                                new Regex(
                                    String.Format(
                                        "(?<alias>{0})(?:/?)$",
                                        String.Join(
                                            "|",
                                            Enumerable.Select(
                                                aliases,
                                                a => Regex.Escape(a)).ToArray())));
                        }
                    }
                }
                return _aliasPattern;
            }
        }

        /// <summary>
        /// Translate URLs that are relative to the app root
        /// into the (potentially different) MVC url space
        /// </summary>
        /// <remarks>
        /// <para>The browser will interpret relative URLs
        /// according to its view of the URL space, but
        /// when you're writing a view you can't necessarily
        /// predict the URL clients will use to reach you;
        /// if it's a job for any of MVC, it would be a job
        /// for the controller, but in ASP.NET MVC, it's
        /// actually a job of the underlying
        /// System.Web.Routing infrastructure to make that
        /// determination.</para>
        /// <para>The filesystem and an MVC app's url-space
        /// are decoupled by routes. We should try to make
        /// these coincide, but it's not guaranteed to work
        /// out that way.</para>
        /// </remarks>
        public static string appRelativeUrl(HttpRequestBase request, string relativeToAppRoot) {
            return appRelativeUrl(request.ApplicationPath, relativeToAppRoot);
        }
        public static string appRelativeUrl(HttpRequest request, string relativeToAppRoot) {
            return appRelativeUrl(request.ApplicationPath, relativeToAppRoot);
        }
        static string appRelativeUrl(string applicationPath, string relativeToAppRoot) {
            return
                String.Format(
                    "{0}{1}{2}",
                    applicationPath,
                    slash[0],
                    relativeToAppRoot.TrimStart(slash));
        }

        /// <summary>hierarchical separator according to
        /// <see href="http://tools.ietf.org/html/rfc3986#section-1.2.3" />
        /// </summary>
        private static readonly char [] slash = new char [] {'/'};

        /// <summary>
        /// IIS6 is lame, but this method can help <see cref="addMvcRoute" />.
        /// </summary>
        public static string action(this UrlHelper helper, string actionName, string controllerName, object routeValues) {
            Uri to;
            try {
                to = new Uri(helper.Action(actionName, controllerName, routeValues), UriKind.RelativeOrAbsolute);
            }
            catch (ArgumentNullException e) {
                var routes = RouteTable.Routes;
                if (routes == null) {
                    throw new InvalidOperationException("the route table is empty", e);
                }
                var routeInfo =
                    Enumerable.Select(
                        routes,
                        x =>
                        x is Route ?
                        ((Route)x).Url.ToString() :
                        String.Format(
                            "({0} non-Route {1})",
                            x.GetType().FullName,
                            x));
                throw new Exception(
                              String.Format(
                                  "helper.Action({0}, {1}, {2}) is null. Route table: {3}.",
                                  actionName ?? "null",
                                  controllerName ?? "null",
                                  routeValues ?? "null",
                                  String.Join(" ", routeInfo.ToArray())),
                              e);
            }
            var Request = HttpContext.Current.Request;
            var fromstring = Request.ServerVariables["HTTP_X_REWRITE_URL"];
            Uri from =
                fromstring != null ?
                    new Uri(Request.Url, fromstring) :
                    Request.Url;

            return MvcMangle(to, from);
        }
        public static string action(this UrlHelper helper, string actionName, string controllerName) {
            return action(helper, actionName, controllerName, null);
        }
        public static string action(this UrlHelper helper, string actionName) {
            return action(helper, actionName, null);
        }


        /// <summary>
        /// Add cruft to a URL in accordance with the
        /// precedent exemplified in <paramref name="from" />.
        /// </summary>
        /// <param name="to">the ASP.NET-routing-canonical url</param>
        /// <param name="from">the url of the current request</param>
        public static string MvcMangle(Uri to, Uri from) {
            Match m = MvcUtils.aliasPattern.Match(from.AbsolutePath);
            if (!m.Success) return to.ToString();

            Uri absTo = new Uri(from, to);
            if (MvcUtils.aliasPattern.IsMatch(absTo.AbsolutePath)) {
                return to.ToString();
            }

            if (to.IsAbsoluteUri) {
                string result =
                    to.Scheme + "://" +
                    (to.UserInfo == String.Empty ? String.Empty : to.UserInfo + "@") +
                    to.Authority +
                    to.AbsolutePath +
                    m.Groups["alias"].Value +
                    to.Query +
                    to.Fragment;
                return result;
            }
            else {
                int qAndF =
                    absTo.Query.Length +
                    absTo.Fragment.Length;
                string result = to.ToString();
                result =
                    result.Substring(0, result.Length - qAndF) +
                    m.Groups["alias"].Value +
                    result.Substring(result.Length - qAndF, qAndF);
                return result;
            }
        }

        /// <summary>
        /// Translate from a string in the
        /// <see href="http://www.w3.org/TR/html5/syntax.html#syntax-text">CDATA repertoire</see> to a string in the <see href="http://www.w3.org/TR/html4/types.html#h-6.2">ID and NAME token repertoire</see>.
        /// <summary>
        /// <remarks>
        /// Note that the <c>name</c> attribute of form controls has type
        /// <c>CDATA</c>, not <c>NAME</c>. So, it is not necessary to use
        /// this method to map cdata values to acceptable form control
        /// names.
        /// </remarks>
        public static String idFromCDATA(String cdata) {
            if (String.IsNullOrEmpty(cdata)) return cdata;
            var result = new StringBuilder();
            if (!Regex.IsMatch(cdata, "^[A-Za-z]", RegexOptions.Compiled)) {
                result.Append("i");
            }
            result.Append(
                Regex.Replace(cdata, @"[^A-Za-z0-9\-_:.]", "_", RegexOptions.Compiled));
            return result.ToString();
        }

        /// <summary>
        /// Determines whether the <paramref name="p">principal</paramref>
        /// belongs to a specified <paramref name="role">role</paramref>.
        /// </summary>
        /// <remarks>
        /// The namesake method from
        /// <see cref="System.Security.Principal.IPrincipal" />
        /// is implemented in <see cref="System.WindowsPrincipal" />
        /// to throw/propagate exceptions when the role does not
        /// have the form domain\group or group@domain. This behavior
        /// is not documented and the exception types are not
        /// appropriate for clients of <c>IPrincipal</c>. This method
        /// avoids those problems by returning <c>true</c> when the
        /// principal has the given role and <c>false</c> otherwise.
        /// <seealso cref="System.Security.Principal.IPrincipal.IsInRole" />
        /// </remarks>
        public static Boolean IsInRole(IPrincipal p, String role) {
            try {
                return p.Identity.IsAuthenticated && p.IsInRole(role);
            }
            catch (SystemException e) {
                // see http://seanfoy.blogspot.com/2009/09/trust-relationship-between-primary.html
                if (!e.Message.StartsWith("The trust relationship")) {
                    //... between the primary domain and the trusted domain failed.
                    // --> DC doesn't like the form of the role
                    //... between this workstation and the primary domain failed.
                    // --> no DC available
                    // otherwise
                    throw;
                }

                return false;
            }
        }

        public static Boolean IsInRole(String role) {
            return IsInRole(HttpContext.Current.User, role);
        }
    }

    /// <summary>
    /// Accepts colonless ISO-8601 date/time strings.
    /// </summary>
    /// <remarks>
    /// IIS historically maps URLs to the filesystem namespace, and
    /// MS-DOS traditionally reserves colon in filenames to indicate
    /// access to various devices (CON, PRN, NUL, LPTn, ...).
    /// Consequently, IIS has a cow when it sees a colon mentioned
    /// in the path component of a URL.
    /// </remarks>
    public class ColectomyDateTimeBinder : DefaultModelBinder {
        /// <summary>
        /// ISO-8601 Basic date and time YYYYMMDDTHHmmss+0400
        /// </summary>
        /// <remarks>
        /// Although a DateTime format string could be used with
        /// <see cref="DateTime.Parse" /> and
        /// <see cref="DateTime.ToString" />, this pattern is
        /// useful for routing constraints.
        /// <seealso href="http://isotc.iso.org/livelink/livelink/4021199/ISO_8601_2004_E.zip?func=doc.Fetch&amp;nodeid=4021199" />
        /// </remarks>
        public static Regex ISO8601_Basic_DateTime =
            new Regex(@"(?<YYYY>\d{4})(?<MM>\d{2})(?<DD>\d{2})T(?<hh>\d\d)(?<mm>\d\d)(?<ss>\d\d)(?<zonedesignator>Z|([+-]\d\d(?:\d\d)?))?");
        /// <summary>
        /// Use this pattern with <see cref="DateTime.ToString" /> to
        /// construct URLs.
        /// </summary>
        private static String ISO8601_Basic_DateTimeFormat = "yyyyMMddTHHmmssK";

        public static String ToString(DateTime dt) {
            return dt.ToString(ISO8601_Basic_DateTimeFormat).Replace(":", String.Empty);
        }

        public static DateTime? parse(String stringImage) {
            var match = ISO8601_Basic_DateTime.Match(stringImage);
            if (match.Success) {
                return
                    DateTime.Parse(
                        String.Format(
                            "{0}-{1}-{2}T{3}:{4}:{5}{6}",
                            match.Groups["YYYY"].Value,
                            match.Groups["MM"].Value,
                            match.Groups["DD"].Value,
                            match.Groups["hh"].Value,
                            match.Groups["mm"].Value,
                            match.Groups["ss"].Value,
                            match.Groups["zonedesignator"].Value));
            }
            return null;
        }

        override public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            // we deal with an ISO standard here, culture matters not.
            var bindingValue = bindingContext.ValueProvider[bindingContext.ModelName];
            if (bindingValue == null) return null;

            var stringImage = bindingValue.AttemptedValue;
            object result = parse(stringImage);

            if (ReferenceEquals(result, null)) {
                result = base.BindModel(controllerContext, bindingContext);
            }
            return result;
        }
    }

    public abstract class NullableBooleanBinder : DefaultModelBinder {
        abstract public Boolean? parse(String stringImage);

        override public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var success = bindingContext.ValueProvider[bindingContext.ModelName];
            if (success == null) return null;
            var stringImage = success.AttemptedValue;
            object result = parse(stringImage);

            if (ReferenceEquals(result, null)) {
                result = base.BindModel(controllerContext, bindingContext);
            }
            return result;
        }
    }
    public class CheckboxBinder : NullableBooleanBinder {
        override public Boolean? parse(String stringImage) {
            return (stringImage == "on") ? true : false;
        }
    }
    public class ButtonBinder : NullableBooleanBinder {
        override public Boolean? parse(String stringImage) {
            return (stringImage != null);
        }
    }

    /// <summary>
    /// A response including an entity representing an arbitrary
    /// and potentially infinite stream of data.
    /// <summary>
    /// </remarks>
    /// Microsoft's implementation fails to <c>Flush</c> and so
    /// sometimes delivers incomplete responses to clients.
    /// <seealso cref="IEnumerableStream" />
    /// <remarks>
    public class WorkingFileStreamResult : FileStreamResult {
        public WorkingFileStreamResult(System.IO.Stream fileStream, String contentType) : base(fileStream, contentType) { }

        public override void ExecuteResult(ControllerContext context) {
            base.ExecuteResult(context);
            context.HttpContext.Response.Flush();
        }
    }
}
