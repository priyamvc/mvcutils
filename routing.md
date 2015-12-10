# Problem #
Historically, DOS and Windows have strong conventions for file naming wherein the file extension indicates the type of data in the file. IIS6 and below map the url space to file system paths, using file extensions to decide how to respond to each request. The mapping is very simple; urls for IIS6 and worse normally include file extensions. Note that the file extension may not relate at all to the data type of the response; it relates to the data type of the the file that tells IIS how to generate the response.

The [architecture of the web](http://www.w3.org/TR/webarch/) suggests drawing a boundary between responses and the mechanisms by which responses are generated. URIs can identify resources with shared representations (the responses). The mechanism by which responses are produced is of little interest outside the server that produces them, and by hiding the implementation detail we gain useful flexibility.

Consider a web site implemented in ASP "classic." Although users don't care that the site is implemented with VBScript or TCL/TK, the urls in a typical asp application indicate the use of asp. Of course, from a web architecture perspective, there's nothing wrong with using file extensions in the identification of a resource. The problem is that the file extensions have meaning apart from the identification of resources. When the site owner decides to switch to ASP.NET, all the urls change and the users' bookmarks break.

# Routing and MVC #
ASP.NET MVC highlights the capabilities of System.Web.Routing, which decouples urls from the filesystem. This potentially eases migration from ASP.NET Web Forms and ASP "classic", as well as to Django or whatever Microsoft comes up with next.

Unfortunately, although System.Web.Routing provides a suitably flexible mapping between URLs and the .NET programs that correspond to them, IIS6 still relies on file extensions to decide whether to let ASP.NET respond to a request in the first place. This is fixed in IIS7.

Not everyone will upgrade to IIS7 immediately, and in the meantime it would be good to have a fix for IIS6 or at least a means by which we could write applications that work on both IIS6 and IIS7, without perpetuating the exposure of file extensions in URLs.

[IIRF](http://www.codeplex.com/IIRF) is a fix for IIS6. It rewrites URLs so that an incoming "cool URI" appears to IIS as a file-extension-containing URL.

This solves the architectural problem but it doesn't address the implementation of a particular hosted application: the app still sees (synthetic) file extensions in incoming URLs, and the MVC framework will naturally generate file-extension-containing URLs.

So we have three cases:
  1. IIS7 + System.Web.Routing: cool URIs
  1. IIS6 + System.Web.Routing: uncool URIs with some behind-the-scenes improvements compared to older Microsoft web frameworks
  1. IIS6 + IIRF + System.Web.Routing: cool URIs if you're willing to do some extra work.

Wouldn't it be great if we could deploy the same application code in all three cases, and have the coolest possible URIs in each case? We could accomplish this by:
  1. Adding routes for both cool and uncool URIs
  1. Generating actions (URIs) that match the coolness of the current request

`MvcUtils.addMvcRoute` makes route replication easy. Given a cool URI pattern, it adds a route for that (by delegating to Microsoft's `MapRoute` extension method for `RouteCollection`) and generates additional routes to support IIRF and IIS6-sans-IIRF. IIRF's synthetic file extension for MVC is conventionally .mvc, and for those who are not free to edit IIS application mappings, `addMvcRoute` also adds a route with .aspx.

```
MVCUtils.addMVCRoute(
    RouteTable.Routes,
    "register",
    new {
        controller = "Registration",
        action = "form"});
```

The `MvcUtils.action` extension method works just like Microsoft's `Action` extension method, except that it is aware of the `addMvcRoute` approach. It senses the coolness of incoming URIs and generates outbound URIs to match.



&lt;form method="post" action="${Url.action('book', 'Registration')}"&gt;

