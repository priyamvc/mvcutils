using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace seanfoy.mvcutils {
    public class CookieAuthenticationHttpModule : HttpModuleBase {
        public override void Init(HttpApplication app) {
            base.Init(app);
            app.AuthenticateRequest += authenticateRequest;
        }

        public void authenticateRequest(Object sender, EventArgs args) {
            if (recessive &&
                Context.User != null &&
                Context.User.Identity.IsAuthenticated) return;
            Context.User =
                authenticateRequest(Request.Cookies[cookieName]) ??
                Context.User;
        }

        public void deauthenticateRequest() {
            Context.User = null;
        }

        public HttpCookie GenerateLogoutCookie() {
            return GenerateCookie(null, new String [] {}, DateTime.Now.AddDays(-1), "0.0.0.0");
        }

        public static int rolesInCookie(HttpCookie c) {
            return int.Parse(c.Values["role-count"]);
        }

        public static String keyForRole(int i) {
            return String.Format("role{0}", i);
        }

        public IPrincipal authenticateRequest(HttpCookie cookie) {
            if (!ValidP(cookie)) {
                return null;
            }
            var roles = new List<String>();
            for (int i = 0; i < rolesInCookie(cookie); ++i) {
                roles.Add(cookie.Values[keyForRole(i)]);
            }
            return
                new GenericPrincipal(
                    new GenericIdentity(
                        cookie.Values["username"],
                        cookieName),
                    roles.ToArray());
        }
        public Boolean authenticatedThisWay(IPrincipal p) {
            return
                p.Identity != null &&
                cookieName.Equals(p.Identity.AuthenticationType);
        }
        public Boolean ValidP(HttpCookie cookie) {
            if (Object.ReferenceEquals(cookie, null)) return false;
            var check =
                GenerateCookie(
                    cookie.Values["username"],
                    Enumerable.Select(
                        Enumerable.Range(0, rolesInCookie(cookie)),
                        i => cookie.Values[keyForRole(i)]).ToArray(),
                    DateTime.Parse(cookie.Values["expiry"]),
                    cookie.Values["client-ip"]);
            return cookie.Values.ToString().Equals(check.Values.ToString());
        }
        public HttpCookie GenerateCookie(String username, String [] roles, DateTime expiry, String clientIP) {
            var result = new HttpCookie(cookieName);
            result.Expires = expiry;
            result.Values["username"] = username;
            result.Values["expiry"] = expiry.ToString();
            result.Values["client-ip"] = clientIP;
            result.Values["role-count"] = roles.Length.ToString();
            foreach (var r in
                     Enumerable.Select(
                         Enumerable.OrderBy(roles, r => r),
                         (r, i) => new {
                                 key = keyForRole(i),
                                 value = r})) {
                result.Values[r.key] = r.value;
            }
            result.Values["sig"] = sign(result.Values);
            return result;
        }
        public String sign(NameValueCollection cookieValues) {
            using (var signer = new HMACSHA512(signingKey)) {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using (var ms = new System.IO.MemoryStream()) {
                    bf.Serialize(ms, cookieValues);
                    ms.Seek(0, SeekOrigin.Begin);
                    return writeKey(signer.ComputeHash(ms));
                }
            }
        }

        public String cookieName;
        public Byte [] signingKey;
        public Boolean recessive;
        public CookieAuthenticationHttpModule() : this(null) {}
        public CookieAuthenticationHttpModule(NameValueCollection config) {
            Configure(config);
        }
        public static Byte [] readKey(String image) {
            return
                Enumerable.Select(
                    image.Split('-'),
                    bi => Byte.Parse(bi, System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
        }
        public static String writeKey(Byte [] key) {
            return BitConverter.ToString(key);
        }
        public void Configure(NameValueCollection config) {
            if (config == null) config = new NameValueCollection();
            cookieName =
                config[configKey("cookieName")] ??
                String.Format("{0}-token", this.GetType().FullName);
            var skConfigKey = configKey("signingKey");
            if (config[skConfigKey] != null) {
                signingKey = readKey(config[skConfigKey]);
            }
            else {
                lock(syncroot) {
                    if (sharedKey == null) {
                        // see http://msdn.microsoft.com/en-us/library/w5y83hkf.aspx
                        // about key size
                        var sk = new Byte [64];
                        new RNGCryptoServiceProvider().GetBytes(sk);
                        sharedKey = sk;
                    }
                }
                signingKey = sharedKey;
            }
            recessive =
                config[configKey("recessive")] == null ?
                false :
                Boolean.Parse(config[configKey("recessive")]);
        }
        public String configKey(String setting) {
            return String.Format("{0}.{1}", GetType().FullName, setting);
        }
        private static Object syncroot = new Object();
        /// <summary>
        /// If no signing key is configured, we'll
        /// generate a key on demand and share it
        /// amongst module instances that lack
        /// key config data.
        /// </summary>
        private static volatile Byte [] sharedKey;
    }
    public class UrlAuthorizationHttpModule : HttpModuleBase {
        public override void Init(HttpApplication app) {
            base.Init(app);
            app.AuthorizeRequest += authorizeRequest;
        }

        public class NotHttpContextBase : HttpContextBase {
            override public HttpRequestBase Request {
                get {
                    return request;
                }
            }
            public HttpRequestBase request { get; set; }
        }
        public class NotHttpRequestBase : HttpRequestBase {
            public HttpRequestBase inner { get; set; }
            override public Uri Url {
                get {
                    return url;
                }
            }
            public Uri url { get; set; }
            override public String AppRelativeCurrentExecutionFilePath {
                get {
                    if (inner != null) {
                        return inner.AppRelativeCurrentExecutionFilePath;
                    }
                    return base.AppRelativeCurrentExecutionFilePath;
                }
            }
            override public String PathInfo {
                get {
                    if (inner != null) {
                        return inner.PathInfo;
                    }
                    return base.PathInfo;
                }
            }
        }

        /// <summary>
        /// Given request and app config data, determine
        /// an equivalent url that UrlHelper would
        /// generate.
        /// </summary>
        public static String CanonicalRequestUrl(HttpContextBase ctx, RouteCollection routes) {
            var url = ctx.Request.Url.ToString();
            var rd = routes.GetRouteData(ctx);
            if (rd == null) {
                //404
            }
            else if (rd.Values.ContainsKey("action")) {
                //canonicalize
                var rctx = new RequestContext(ctx, rd);
                var uh = new UrlHelper(rctx, routes);
                url =
                    uh.Action(
                        (String)rd.Values["action"],
                        rd.Values);
            }
            return url;
        }

        /// <summary>
        /// Extract a relative url <c>r</c>
        /// from the server-relative <paramref name="canonicalUrl" /> such that
        /// <c>new Uri(approot, r) = <paramref name="canonicalUrl" /></c>.
        /// </summary>
        /// <remarks>
        /// <see cref="CanonicalRequestUrl" />
        /// See also <see cref="System.Web.VirtualPathUtility.ToAppRelative" />,
        /// which is not so testable and whose result is always either an
        /// absolute URL or a url beginning with ~ (and possibly ending
        /// exactly there).
        /// </remarks>
        public static String AppSpecificUrlStem(HttpContextBase ctx, String canonicalUrl) {
            var appPath =
                ctx.Request.ApplicationPath;
            if (appPath.StartsWith("/")) {
                appPath = appPath.Substring(1);
            }
            if (appPath.EndsWith("/")) {
                appPath = appPath.Substring(0, appPath.Length - 1);
            }
            if (canonicalUrl.StartsWith("/")) {
                canonicalUrl = canonicalUrl.Substring(1);
            }
            if (canonicalUrl.StartsWith(appPath)) {
                canonicalUrl = canonicalUrl.Substring(appPath.Length);
                if (canonicalUrl.StartsWith("/")) {
                    canonicalUrl = canonicalUrl.Substring(1);
                }
            }
            return canonicalUrl;
        }

        public ACLEntry authorize(HttpContextBase ctx, RouteCollection routes) {
            var url =
                AppSpecificUrlStem(
                    ctx,
                    CanonicalRequestUrl(ctx, routes));
            foreach (var rule in
                     Enumerable.Union(rules, new [] { defaultRule })) {
                if (!rule.Key.IsMatch(url)) continue;
                return
                    Enumerable.FirstOrDefault(
                        rule.Value,
                        acl => acl.Principal.Matches(ctx.User));
            }
            throw new Exception("this should be unreachable.");
        }

        public class PrincipalEquality {
            public String PrincipalPattern { get; set; }
            public virtual Boolean Matches(IPrincipal principal) {
                if ("*".Equals(PrincipalPattern)) {
                    return true;
                }
                else if ("?".Equals(PrincipalPattern)) {
                    return !principal.Identity.IsAuthenticated;
                }
                return true;
            }
        }
        public class PrincipalNameICEquality : PrincipalEquality {
            override public Boolean Matches(IPrincipal principal) {
                return base.Matches(principal) &&
                    String.Equals(principal.Identity.Name, PrincipalPattern, StringComparison.OrdinalIgnoreCase);
            }
        }
        public class PrincipalNameEquality : PrincipalEquality {
            override public Boolean Matches(IPrincipal principal) {
                return base.Matches(principal) &&
                    String.Equals(principal.Identity.Name, PrincipalPattern, StringComparison.Ordinal);
            }
        }
        public class PrincipalRoleEquality : PrincipalEquality {
            override public Boolean Matches(IPrincipal principal) {
                return base.Matches(principal) &&
                    principal.IsInRole(PrincipalPattern);
            }
        }

        public void authorizeRequest(Object sender, EventArgs args) {
            var authzn =
                authorize(new HttpContextWrapper(Context), RouteTable.Routes);
            if (authzn != null && authzn.Polarity == Polarity.Allow) return;
            Response.StatusCode = 401;
            app.CompleteRequest();
        }

        public enum Polarity {
            Deny,
            Allow
        }
        public class ACLEntry {
            public Polarity Polarity { get; set; }
            public PrincipalEquality Principal { get; set; }
            public ACLEntry(Polarity polarity, PrincipalEquality principal) {
                this.Polarity = polarity;
                this.Principal = principal;
            }
            public ACLEntry(PrincipalEquality principalPattern) : this(Polarity.Allow, principalPattern) {}
            public ACLEntry(Polarity polarity, String principalPattern) : this(polarity,  new PrincipalEquality { PrincipalPattern = principalPattern}) {}
            public ACLEntry(String principalPattern) : this(Polarity.Allow, principalPattern) {}
        }

        public Dictionary<Regex, List<ACLEntry>> rules =
            new Dictionary<Regex, List<ACLEntry>>();

        public KeyValuePair<Regex, List<ACLEntry>> defaultRule =
            new KeyValuePair<Regex, List<ACLEntry>>(
                new Regex(".*"),
                new List<ACLEntry> {{new ACLEntry(Polarity.Allow, @"*")}});

        public UrlAuthorizationHttpModule() : this(null) {}
        public UrlAuthorizationHttpModule(UrlAuthorizationConfigurationSection config) {
            Configure(config);
        }
        public void Configure(UrlAuthorizationConfigurationSection config) {
            if (config == null) return;
            foreach (RuleElement rule in config.Rules) {
                var interp = rule.Interpret();
                rules.Add(interp.Key, interp.Value);
            }
        }

        public static UrlAuthorizationConfigurationSection GetSection(String name) {
            return (UrlAuthorizationConfigurationSection)System.Configuration.ConfigurationManager.GetSection(name);
        }

        public class UrlAuthorizationConfigurationSection : System.Configuration.ConfigurationSection {
            [ConfigurationProperty("rules")]
            public RuleCollectionElement Rules {
                get {
                    return (RuleCollectionElement)base["rules"];
                }
            }
        }
        public class CEC<T> : ConfigurationElementCollection where T : ConfigurationElement, new() {
            public override ConfigurationElementCollectionType CollectionType {
                get {
                    return ConfigurationElementCollectionType.AddRemoveClearMap;
                }
            }

            protected override ConfigurationElement CreateNewElement() {
                return new T();
            }

            protected override Object GetElementKey(ConfigurationElement elt) {
                return ((T)elt).GetHashCode();
            }

            public T this[int index] {
                get {
                    return (T)BaseGet(index);
                }
            }
        }
        public class RuleCollectionElement : CEC<RuleElement> {}
        public class RuleElement : ConfigurationElement {
            [ConfigurationProperty("urlPattern")]
            public String UrlPattern {
                get {
                    return (String)base["urlPattern"];
                }
            }

            [ConfigurationProperty("acl")]
            public ACLElement Acl {
                get {
                    return (ACLElement)base["acl"];
                }
            }

            public KeyValuePair<Regex, List<ACLEntry>> Interpret() {
                return
                    new KeyValuePair<Regex, List<ACLEntry>>(
                        new Regex(UrlPattern),
                        Enumerable.Select(
                            Enumerable.OfType<ACLEntryElement>(Acl),
                            i => i.Interpret()).ToList());
            }
        }
        public class ACLElement : CEC<ACLEntryElement> {}
        public class ACLEntryElement : ConfigurationElement {
            [ConfigurationProperty("polarity")]
            public String Polarity {
                get {
                    return (String)base["polarity"];
                }
            }
            [ConfigurationProperty("principalEquality")]
            public String PrincipalEquality {
                get {
                    return (String)base["principalEquality"];
                }
            }
            [ConfigurationProperty("principalPattern")]
            public String PrincipalPattern {
                get {
                    return (String)base["principalPattern"];
                }
            }

            public ACLEntry Interpret() {
                var peClassname =
                    String.IsNullOrEmpty(PrincipalEquality) ?
                    typeof(PrincipalEquality).AssemblyQualifiedName :
                    PrincipalEquality;
                var pector =
                    Type.GetType(peClassname).
                    GetConstructor(emptyTypeArgs);
                var pe = (PrincipalEquality)pector.Invoke(emptyArgs);
                pe.PrincipalPattern = PrincipalPattern;
                return
                    new ACLEntry(
                        (Polarity)Enum.Parse(
                            typeof(Polarity),
                            Polarity),
                        pe);
            }

            private static Type [] emptyTypeArgs = new Type [] {};
            private static Object [] emptyArgs = new Object [] {};
        }
    }

    public class HttpModuleBase : IHttpModule, IDisposable {
        protected HttpApplication app;
        public virtual void Init(HttpApplication app) {
            this.app = app;
            app.Register(this);
        }
        public void Dispose() {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
        }

        protected HttpContext Context {
            get {
                // app.Context is sometimes
                // null when HttpContext.Current
                // is not! See bug #4529.
                return app.Context ?? HttpContext.Current;
            }
        }
        protected HttpRequest Request {
            get {
                return app.Request;
            }
        }
        protected HttpResponse Response {
            get {
                return app.Response;
            }
        }
    }

    public static class HttpModuleBaseExtensions {
        /// <summary>
        /// Permit later uses of <see cref="Find" />
        /// to locate the <paramref name="module" />.
        /// </summary>
        /// <remarks>
        /// <see cref="HttpApplication.Application" />
        /// has a <c>Modules</c> property, but it is
        /// read-only and incomplete.
        /// <see cref="HttpApplication.Init" />
        /// would be a good call site for this
        /// method.
        /// <c>Register</c> and <c>Find</c> rely on
        /// which is initialized in
        /// <see cref="HttpApplication.Init" />. So,
        /// this method can't be used from the app
        /// constructor (which runs before). Nor may
        /// you register from <c>Application_Start</c>,
        /// which is only invoked once per application
        /// rather than once per <c>HttpApplication</c>.
        /// <c>Application_Start</c> runs before
        /// <c>Init</c>, so <c>Find</c> will not work
        /// if invoked from <c>Application_Start</c>.
        /// </remarks>
        public static void Register(this HttpApplication app, IHttpModule module) {
            app.Application[module.GetType().FullName] = module;
        }
        /// <summary>
        /// Locate the previously registered
        /// <c>IHttpModule</c>.
        /// </summary>
        /// <remarks>
        /// <see cref="HttpApplication.Init" />
        /// would be a good call site for this
        /// method.
        /// Don't try to use this from
        /// <c>Application_Start</c> or from
        /// the constructor of an
        /// <see cref="HttpApplication" />
        /// subtype.
        /// <seealso cref="Register" /> for
        /// extended remarks.
        /// </remarks>
        public static T Find<T>(this HttpApplication app) where T : IHttpModule {
            var module = app.Application[typeof(T).FullName];
            return (T)module;
        }
    }
}