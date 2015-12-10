
```
public class CookieAuthenticationHttpModule : seanfoy.mvcutils.HttpModuleBase
```

# Remarks #
To be added.

**Assembly:** seanfoy.mvcutils 0.0.10.0

# Members #
_List of all members,_

## Constructors ##
| `public CookieAuthenticationHttpModule ();`  | To be added. |
|:---------------------------------------------|:-------------|
| `public CookieAuthenticationHttpModule (System.Collections.Specialized.NameValueCollection config);`  | To be added. |

## Fields ##
| `public string cookieName;`  | To be added. |
|:-----------------------------|:-------------|
| `public bool recessive;`     | To be added. |
| `public byte[] signingKey;`  | To be added. |
| `public bool transferrable;`  | To be added. |

## Methods ##
| `public bool authenticatedThisWay (System.Security.Principal.IPrincipal p);`  | To be added. |
|:------------------------------------------------------------------------------|:-------------|
| `public void authenticateRequest (object sender, EventArgs args);`            | To be added. |
| `public System.Security.Principal.IPrincipal authenticateRequest (System.Web.HttpCookie cookie, DateTime now, string clientIP);`  | To be added. |
| `public string configKey (string setting);`                                   | To be added. |
| `public void Configure (System.Collections.Specialized.NameValueCollection config);`  | To be added. |
| `public void deauthenticateRequest ();`                                       | To be added. |
| `public System.Web.HttpCookie GenerateCookie (string username, string[] roles, DateTime expiry, string clientIP);`  | To be added. |
| `public System.Web.HttpCookie GenerateLogoutCookie ();`                       | To be added. |
| `public string getClientIP ();`                                               | To be added. |
| `public override void Init (System.Web.HttpApplication app);`                 | To be added. |
| `public static string keyForRole (int i);`                                    | To be added. |
| `public static byte[] readKey (string image);`                                | To be added. |
| `public static int rolesInCookie (System.Web.HttpCookie c);`                  | To be added. |
| `public string sign (System.Collections.Specialized.NameValueCollection cookieValues);`  | To be added. |
| `public bool ValidP (System.Web.HttpCookie cookie, DateTime now, string clientIP);`  | To be added. |
| `public static string writeKey (byte[] key);`                                 | To be added. |


# Member Details #
_A detailed description of each member_

## Constructors ##
_A detrailed description of constructors._

### CookieAuthenticationHttpModule Constructor ###
_To be added._
```
public CookieAuthenticationHttpModule ();
```

#### Remarks ####
To be added.

#### Parameters ####
### CookieAuthenticationHttpModule Constructor ###
_To be added._
```
public CookieAuthenticationHttpModule (System.Collections.Specialized.NameValueCollection config);
```

#### Remarks ####
To be added.

#### Parameters ####
  * System.Collections.Specialized.NameValueCollection `config`  To be added.

## Fields ##
_A detailed description of fields._

### cookieName ###
_To be added._
```
public string cookieName;
```
#### Remarks ####
To be added.

### recessive ###
_To be added._
```
public bool recessive;
```
#### Remarks ####
To be added.

### signingKey ###
_To be added._
```
public byte[] signingKey;
```
#### Remarks ####
To be added.

### transferrable ###
_To be added._
```
public bool transferrable;
```
#### Remarks ####
To be added.

## Methods ##
_A detailed description of methods._

### authenticatedThisWay ###
_To be added._
```
public bool authenticatedThisWay (System.Security.Principal.IPrincipal p);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Security.Principal.IPrincipal `p`  To be added.

### authenticateRequest ###
_To be added._
```
public void authenticateRequest (object sender, EventArgs args);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Object `sender`  To be added.
  * System.EventArgs `args`  To be added.

### authenticateRequest ###
_To be added._
```
public System.Security.Principal.IPrincipal authenticateRequest (System.Web.HttpCookie cookie, DateTime now, string clientIP);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpCookie `cookie`  To be added.
  * System.DateTime `now`  To be added.
  * System.String `clientIP`  To be added.

### configKey ###
_To be added._
```
public string configKey (string setting);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `setting`  To be added.

### Configure ###
_To be added._
```
public void Configure (System.Collections.Specialized.NameValueCollection config);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Collections.Specialized.NameValueCollection `config`  To be added.

### deauthenticateRequest ###
_To be added._
```
public void deauthenticateRequest ();
```
#### Remarks ####
To be added.

#### Parameters ####

### GenerateCookie ###
_To be added._
```
public System.Web.HttpCookie GenerateCookie (string username, string[] roles, DateTime expiry, string clientIP);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `username`  To be added.
  * System.String[.md](.md) `roles`  To be added.
  * System.DateTime `expiry`  To be added.
  * System.String `clientIP`  To be added.

### GenerateLogoutCookie ###
_To be added._
```
public System.Web.HttpCookie GenerateLogoutCookie ();
```
#### Remarks ####
To be added.

#### Parameters ####

### getClientIP ###
_To be added._
```
public string getClientIP ();
```
#### Remarks ####
To be added.

#### Parameters ####

### Init ###
_To be added._
```
public override void Init (System.Web.HttpApplication app);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpApplication `app`  To be added.

### keyForRole ###
_To be added._
```
public static string keyForRole (int i);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Int32 `i`  To be added.

### readKey ###
_To be added._
```
public static byte[] readKey (string image);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `image`  To be added.

### rolesInCookie ###
_To be added._
```
public static int rolesInCookie (System.Web.HttpCookie c);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpCookie `c`  To be added.

### sign ###
_To be added._
```
public string sign (System.Collections.Specialized.NameValueCollection cookieValues);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Collections.Specialized.NameValueCollection `cookieValues`  To be added.

### ValidP ###
_To be added._
```
public bool ValidP (System.Web.HttpCookie cookie, DateTime now, string clientIP);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpCookie `cookie`  To be added.
  * System.DateTime `now`  To be added.
  * System.String `clientIP`  To be added.

### writeKey ###
_To be added._
```
public static string writeKey (byte[] key);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Byte[.md](.md) `key`  To be added.