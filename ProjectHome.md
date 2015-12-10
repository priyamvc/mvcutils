This project adds additional functionality on top of the ASP.NET MVC Framework. These enhancements can increase your productivity using the MVC Framework.

Major features include:
  * [convenience methods](routing.md) for adding and using routes that facilitate transition from IIS6-style file-extension-containing urls to IIS7 (or IIS6+[IIRF](http://www.codeplex.com/IIRF)) [cool uris](http://www.w3.org/Provider/Style/URI).
  * an [IsInRole](IsInRole.md) method that works as advertised, even when used with `WindowsPrincipal`s.
  * a [binder](binders.md) for ISO8601 Basic `DateTime` strings, for working around IIS's aversion to colons
  * a `FileStreamResult` implementation that reliably transmits all the data in the underlying stream.
  * a `Stream` implementation for general `IEnumerable`s, which is handy for serving up LINQ query results.
  * an `IHttpModule` for url-based [authorization](authnz#UrlAuthorizationHttpModule.md) using regular expression matching on System.Web.Routing-canonicalized urls.
  * an `IHttpModule` for cookie-based [authentication](authnz#CookieAuthenticationHttpModule.md), which is suitable for e.g., [selenium](http://seleniumhq.org/) UI testing.