# Introduction #

For historical reasons related to a [file system](routing.md) interface to MS-DOS devices, IIS rejects requests whose urls include a colon (other than in the authority portion). Of course, literal colons would be problematic [anyway](http://tools.ietf.org/html/rfc2396#section-2.2), but IIS also rejects properly escaped URLs.

So what if you're trying to [build an Internet-scale notification system](http://roy.gbiv.com/untangled/2008/paper-tigers-and-hidden-dragons)? Suppose you're photo-sharing site, and you want to enable more general social-networking sites to notice when any of their 45k users uploads something new to you. Then you might want to create resources that summarize activity by time-slice. Sensibly enough, you might decide to use times in the corresponding URIs.

Then, you'd better not use IIS. Or, you could use a `DateTime` encoding that avoids the use of colons. Such as [ISO-8601 basic date and time of day representation](http://www.phys.uu.nl/~vgent/calendar/downloads/iso_8601_2004.pdf).

# Details #

The `ColectomyDateTimeBinder` helps you to remove the colons from your `DateTime` strings. It has a function `ToString`: `DateTime` → `String`, and a function `parse`: `String` → `DateTime`. A handy `Regex` (`ISO8601_Basic_DateTime`) is provided to help you recognize encoded `DateTime`s on your own. As you'd expect from its name, `ColectomyDateTimeBinder` implements `IModelBinder`, so it works nicely with ASP.NET MVC.

You can use the usual technique to make the binder generally available to your app:
```
ModelBinders.Binders.Add(typeof(DateTime), new ColectomyDateTimeBinder());
```

If you're using the [routing](routing.md) helpers with IIS6 or IIS6+IIRF, then you might need to use extra care in defining your routes. System.Web.Routing patterns are greedy, so to keep the .aspx or .mvc filename extension out of the [parameter value](http://msdn.microsoft.com/en-us/library/system.web.routing.route.aspx), you will need to add a constraint:
```
MVCUtils.addMVCRoute(
    "uploads/{timeslice}",
    new {
        controller = "Sawzall",
        action = "report"},
    //HACK: for greediness
    new {
        timeslice = ColectomyDateTimeBinder.ISO8601_Basic_DateTime.ToString()});
```

And when you want to link to an activity summary:
```
Url.action(
    "report",
    "Sawzall",
    new {
        timeslice = ColectomyDateTimeBinder.ToString(quantize(DateTime.Now)) });
```