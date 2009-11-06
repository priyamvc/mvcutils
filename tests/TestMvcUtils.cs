using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using Moq;

namespace seanfoy.mvcutils {
    [TestFixture]
    public class TestMvcUtils {
        [Test]
        public void ISO8601Binder() {
            var expected = DateTime.Parse("1979-11-17T13:24:05-0600");
            Assert.AreEqual(
                expected,
                ColectomyDateTimeBinder.parse(ColectomyDateTimeBinder.ToString(expected)));
        }

        [Test]
        public void ISO8601WholeDate() {
            var input = DateTime.Parse("1979-11-17T13:24:05-0630");
            var str = ColectomyDateTimeBinder.ToString(input);
            var m = ColectomyDateTimeBinder.ISO8601_Basic_DateTime.Match(str);
            Assert.IsTrue(m.Success);
            Assert.AreEqual(str, m.Value);
        }

        Uri appRoot = new Uri("http://domain.tld/");

        [Test]
        public void catchAllRouting() {
            var routes = new RouteCollection();
            MvcUtils.addMvcRoute(
                routes,
                "{*_}",
                new { controller = "Foo", action="Wei" },
                null);
            var ctx = mockContext(appRoot.ToString(), "http://domain.tld/foo");
            var c = (HttpContextBase)ctx.Object;
            Assert.IsNotNull(routes.GetRouteData(c));
            ctx.Setup(x => x.Request.Url).Returns(new Uri(appRoot, "foo.aspx"));
            Assert.IsNotNull(routes.GetRouteData(c));
            // N.B., the catch-all routeValue will capture
            // pseudofileextensions if they're on the
            // Request.Url. I'm not sure how best to
            // address that.
        }

        [Test]
        public void rootSlash() {
            var authoritySansSlash = "http://domain.tld";
            var authorityAvecSlash = "http://domain.tld/";
            var medialSansSlash = "http://domain.tld/app";
            var medialAvecSlash = "http://domain.tld/app/";
            foreach (var approot in
                     new [] {
                         authoritySansSlash, authorityAvecSlash,
                         medialSansSlash, medialAvecSlash}) {
                foreach (var rurl in
                         new [] {
                             String.Empty, "?qs", "f", "d/f"}) {
                    var requestUrl = new Uri(new Uri(approot), rurl);
                    var serverRelativeUrl =
                        new Uri(requestUrl.GetLeftPart(UriPartial.Authority)).MakeRelativeUri(requestUrl);
                    var ctx = (HttpContextBase)mockContext(approot, requestUrl.ToString()).Object;
                    Assert.AreEqual(
                        rurl,
                        UrlAuthorizationHttpModule.AppSpecificUrlStem(
                            ctx,
                            serverRelativeUrl.ToString()));
                }
            }
        }

        [Test]
        public void PermissiveDefault() {
            var ctx = mockContext(appRoot.ToString(), "http://domain.tld/foo");
            var routes = new RouteCollection();
            MvcUtils.addMvcRoute(
                routes,
                "{*_}",
                new { controller = "Foo", action="Wei" },
                null);
            var urlauthnz =
                new UrlAuthorizationHttpModule();
            UrlAuthorizationHttpModule.ACLEntry authorization;
            foreach (String suffix in
                     new [] {String.Empty, "foo", "w/?x=y", "bar/x.y?a=b&c=d"}) {
                ctx.Setup(c => c.Request.Url).Returns(new Uri(appRoot, suffix));
                authorization =
                    urlauthnz.authorize((HttpContextBase)ctx.Object, routes);
                Assert.IsNotNull(authorization);
                Assert.AreEqual(
                    UrlAuthorizationHttpModule.Polarity.Allow,
                    authorization.Polarity);
            }
        }

        [Test]
        public void RulesAreOrdered() {
            var ctx = mockContext(appRoot.ToString(), "http://domain.tld/");
            var routes = new RouteCollection();
            MvcUtils.addMvcRoute(
                routes,
                String.Empty,
                new { controller = "Foo", action = "Wei" },
                null);
            var urlauthnz =
                new UrlAuthorizationHttpModule();
            var acl =
                new List<UrlAuthorizationHttpModule.ACLEntry> {
                    {new UrlAuthorizationHttpModule.ACLEntry(
                         UrlAuthorizationHttpModule.Polarity.Deny,
                         @"*")}};
            urlauthnz.rules.Add(
                new Regex(".*"),
                acl);
            Assert.AreEqual(
                UrlAuthorizationHttpModule.Polarity.Deny,
                urlauthnz.authorize((HttpContextBase)ctx.Object, routes).Polarity);
            acl.Insert(
                0,
                new UrlAuthorizationHttpModule.ACLEntry(
                    UrlAuthorizationHttpModule.Polarity.Allow,
                    @"*"));
            Assert.AreEqual(
                UrlAuthorizationHttpModule.Polarity.Allow,
                urlauthnz.authorize((HttpContextBase)ctx.Object, routes).Polarity);
        }

        [Test]
        public void validCookieExists() {
            var authn = new CookieAuthenticationHttpModule();
            var tastey = authn.GenerateCookie("sean", new [] {"teacher", "student"}, DateTime.Now.AddHours(3), "209.144.4.100");
            Assert.IsTrue(authn.ValidP(tastey));
        }

        [Test]
        public void tamperResistentCookies() {
            //sanity
            var authn = new CookieAuthenticationHttpModule();
            var tastey = authn.GenerateCookie("sean", new [] {"teacher", "student"}, DateTime.Now.AddHours(3), "209.144.4.100");
            Assert.IsTrue(authn.ValidP(tastey));

            //surgical tampering
            int insertionPosition = int.Parse(tastey.Values["role-count"]);
            tastey.Values["role-count"] = (insertionPosition + 1).ToString();
            tastey.Values[CookieAuthenticationHttpModule.keyForRole(insertionPosition)] = "bogus";
            Assert.IsFalse(authn.ValidP(tastey));

            //wholesale naughtiness; this portion of the test
            // is more likely to survive implementation changes
            // in the cookie module.
            var samples =
                Enumerable.Select(
                    new [] {
                        new {name = "sean", role = "serf"},
                        new {name = "sohail", role="plebeian"}},
                    nr => authn.GenerateCookie(nr.name, new [] {nr.role}, DateTime.Now.AddHours(3), "209.144.4.100")).ToList();
            Assert.IsTrue(Enumerable.All(samples, s => authn.ValidP(s)));
            foreach (var i in Enumerable.Range(0, samples.Count)) {
                if (i % 2 == 1) continue;
                var tradingPartner = (i + 1) % samples.Count;
                var t = samples[tradingPartner].Values["sig"];
                samples[tradingPartner].Values["sig"] = samples[i].Values["sig"];
                samples[i].Values["sig"] = t;
            }
            Assert.IsTrue(Enumerable.All(samples, s => !authn.ValidP(s)));
        }

        [Test]
        public void roundtripCookie() {
            var roles =
                new String [] {"iDeal", "teacher", "student"};
            var principal =
                new GenericPrincipal(
                    new GenericIdentity("sean", "test"),
                    roles);
            Assert.IsTrue(
                Enumerable.All(
                    roles,
                    r => principal.IsInRole(r)));
            var authn = new CookieAuthenticationHttpModule();
            var tastey =
                authn.GenerateCookie(
                    principal.Identity.Name,
                    roles,
                    DateTime.Now.AddHours(3),
                    "209.144.4.100");

            var authorized =
                authn.authenticateRequest(tastey);
            Assert.AreEqual(principal.Identity.Name, authorized.Identity.Name);
            Assert.IsTrue(
                Enumerable.All(
                    roles,
                    r => authorized.IsInRole(r)));
        }

        private System.Security.Principal.WindowsPrincipal getWindowsPrincipal() {
            return
                new System.Security.Principal.WindowsPrincipal(
                    System.Security.Principal.WindowsIdentity.GetCurrent());

        }

        [Test]
        [Ignore("This is needed on MSFT but not Mono; we don't actually rely on the MSFT behavior for our own correctness so let's ignore this until MSFT fixes their implementation or Mono adopts their silly behavior.")]
        public void WindowsPrincipalIsInRoleExpectsDomain() {
            var p = getWindowsPrincipal();
            try {
                p.IsInRole("whatever");
                Assert.Fail("maybe we can simplify MvcUtils.IsInRole after all");
            }
            catch (SystemException) {
                //expected
            }
        }

        [Test]
        public void IsInRoleToleratesAbsenceOfDomains() {
            var p = getWindowsPrincipal();
            MvcUtils.IsInRole(p, @"domain\whatever");
            MvcUtils.IsInRole(p, "whatever@domain");
        }

        public Mock<HttpContextBase> mockContext(String applicationRoot, String requestUrl) {
            var appRoot = new Uri(applicationRoot);
            var ctx = new Mock<HttpContextBase>();
            ctx.Setup(c => c.Request.Url).Returns(new Uri(requestUrl));
            ctx.Setup(c => c.Request.ApplicationPath).Returns(appRoot.AbsolutePath);
            //        ctx.Setup(c => c.Response.ApplyAppPathModifier(It.IsAny<String>())).Returns((string s) => new Uri(appRoot, s).ToString());
            ctx.Setup(c => c.Response.ApplyAppPathModifier(It.IsAny<String>())).Returns((string s) => new Uri(appRoot, s).ToString());
            // Mono's System.Web.Routing.Route.GetRouteData method wants
            // to see String.Empty for PathInfo, else it throws
            // NotImplementedException.
            // PathInfo is "additional path information for a resource"
            // e.g. "/tail" for http://www.contoso.com/virdir/page.html/tail
            // according to MSDN. I guess the idea is the web server takes
            // the longest available prefix URL as the matching resource
            // identifier and leaves whatever remains in PathInfo. In that
            // case, this really is a reasonable default for most of the
            // test cases.
            ctx.Setup(c => c.Request.PathInfo).Returns(String.Empty);
            ctx.
                Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).
                Returns(
                    () =>
                        String.Format(
                            "~/{0}",
                            appRoot.MakeRelativeUri(
                                ((HttpContextBase)ctx.Object).Request.Url).ToString()));
            return ctx;
        }
    }
}
