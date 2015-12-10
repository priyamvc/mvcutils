
```
public static class MvcUtils
```

# Remarks #
To be added.

**Assembly:** seanfoy.mvcutils 0.0.10.0

# Members #
_List of all members,_


## Fields ##
| `public static System.Text.RegularExpressions.Regex modreq(System.Runtime.CompilerServices.IsVolatile) _aliasPattern;`  | To be added. |
|:------------------------------------------------------------------------------------------------------------------------|:-------------|
| `public static readonly object _aliasPatternLock;`                                                                      | To be added. |
| `public static string[] aliases;`                                                                                       | To be added. |

## Properties ##
| `public static System.Text.RegularExpressions.Regex aliasPattern { get; }`  | To be added. |
|:----------------------------------------------------------------------------|:-------------|

## Methods ##
| `public static string action (this System.Web.Mvc.UrlHelper helper, string actionName);`  | To be added. |
|:------------------------------------------------------------------------------------------|:-------------|
| `public static string action (this System.Web.Mvc.UrlHelper helper, string actionName, string controllerName);`  | To be added. |
| `public static string action (this System.Web.Mvc.UrlHelper helper, string actionName, string controllerName, object routeValues);`  | To be added. |
| `public static void addMvcRoute (string url, object defaults);`                           | To be added. |
| `public static void addMvcRoute (string url, object defaults, object constraints);`       | To be added. |
| `public static void addMvcRoute (System.Web.Routing.RouteCollection routes, string url, object defaults, object constraints);`  | To be added. |
| `public static string appRelativeUrl (System.Web.HttpRequest request, string relativeToAppRoot);`  | To be added. |
| `public static string appRelativeUrl (System.Web.HttpRequestBase request, string relativeToAppRoot);`  | To be added. |
| `public static string idFromCDATA (string cdata);`                                        | To be added. |
| `public static bool IsInRole (string role);`                                              | To be added. |
| `public static bool IsInRole (System.Security.Principal.IPrincipal p, string role);`      | To be added. |
| `public static string MvcMangle (Uri to, Uri from);`                                      | To be added. |


# Member Details #
_A detailed description of each member_


## Fields ##
_A detailed description of fields._

### _aliasPattern ###_To be added._```
public static System.Text.RegularExpressions.Regex modreq(System.Runtime.CompilerServices.IsVolatile) _aliasPattern;
```
#### Remarks ####
To be added._

### _aliasPatternLock ###_To be added._```
public static readonly object _aliasPatternLock;
```
#### Remarks ####
To be added._

### aliases ###
_To be added._
```
public static string[] aliases;
```
#### Remarks ####
To be added.
## Properties ##
_A detailed description of properties._

### aliasPattern ###
_To be added._
```
public static System.Text.RegularExpressions.Regex aliasPattern { get; }
```
#### Value ####
To be added.

## Methods ##
_A detailed description of methods._

### action ###
_To be added._
```
public static string action (this System.Web.Mvc.UrlHelper helper, string actionName);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.Mvc.UrlHelper `helper`  To be added.
  * System.String `actionName`  To be added.

### action ###
_To be added._
```
public static string action (this System.Web.Mvc.UrlHelper helper, string actionName, string controllerName);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.Mvc.UrlHelper `helper`  To be added.
  * System.String `actionName`  To be added.
  * System.String `controllerName`  To be added.

### action ###
_To be added._
```
public static string action (this System.Web.Mvc.UrlHelper helper, string actionName, string controllerName, object routeValues);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.Mvc.UrlHelper `helper`  To be added.
  * System.String `actionName`  To be added.
  * System.String `controllerName`  To be added.
  * System.Object `routeValues`  To be added.

### addMvcRoute ###
_To be added._
```
public static void addMvcRoute (string url, object defaults);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `url`  To be added.
  * System.Object `defaults`  To be added.

### addMvcRoute ###
_To be added._
```
public static void addMvcRoute (string url, object defaults, object constraints);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `url`  To be added.
  * System.Object `defaults`  To be added.
  * System.Object `constraints`  To be added.

### addMvcRoute ###
_To be added._
```
public static void addMvcRoute (System.Web.Routing.RouteCollection routes, string url, object defaults, object constraints);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.Routing.RouteCollection `routes`  To be added.
  * System.String `url`  To be added.
  * System.Object `defaults`  To be added.
  * System.Object `constraints`  To be added.

### appRelativeUrl ###
_To be added._
```
public static string appRelativeUrl (System.Web.HttpRequest request, string relativeToAppRoot);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpRequest `request`  To be added.
  * System.String `relativeToAppRoot`  To be added.

### appRelativeUrl ###
_To be added._
```
public static string appRelativeUrl (System.Web.HttpRequestBase request, string relativeToAppRoot);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpRequestBase `request`  To be added.
  * System.String `relativeToAppRoot`  To be added.

### idFromCDATA ###
_To be added._
```
public static string idFromCDATA (string cdata);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `cdata`  To be added.

### IsInRole ###
_To be added._
```
public static bool IsInRole (string role);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `role`  To be added.

### IsInRole ###
_To be added._
```
public static bool IsInRole (System.Security.Principal.IPrincipal p, string role);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Security.Principal.IPrincipal `p`  To be added.
  * System.String `role`  To be added.

### MvcMangle ###
_To be added._
```
public static string MvcMangle (Uri to, Uri from);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Uri `to`  To be added.
  * System.Uri `from`  To be added.