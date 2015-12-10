# Introduction #

Microsoft provides [authentication](http://msdn.microsoft.com/en-us/library/system.web.security.formsauthentication.aspx) and [authorization](http://msdn.microsoft.com/en-us/library/system.web.security.urlauthorizationmodule.aspx) modules for ASP.NET, but they're not ideal for every project.

`FormsAuthenticationModule` doesn't compose well with other authentication methods, because it combines cookie- and url-based authentication with an expected Web Forms UI. Specifically, it redirects unauthenticated users to the login page rather than simply allowing other modules to try other modes of authentication.

`UrlAuthorizationModule` allows you to specify permissions for groups of urls. Users can be identified by name, by role-membership, or by the special designations `?` and `*` for anonymous and anyone respectively. Urls can be specified exactly or by matching prefixes at path-separator boundaries. These `UrlAuthorizationModule` url patterns are weaker than we might like for MVC applications, particularly in the presence of [IIS6 url mangling](routing.md).

# `CookieAuthenticationHttpModule` #

This module issues and validates cookies, and handles the [AuthenticateRequest](http://msdn.microsoft.com/en-us/library/system.web.httpapplication.authenticaterequest.aspx) event by setting [HttpContext.User](http://msdn.microsoft.com/en-us/library/system.web.httpcontext.user.aspx) appropriately.

Unlike `FormsAuthenticationModule`, it does not address human user interface concerns at all. It also does not integrate with the standard Role/Membership provider infrastructure. **It has not been reviewed by cryptology or other security experts**.

This module is appropriate for use in a testing environment. For example, it combines nicely with `WindowsAuthenticationModule` to allow selenium tests to authenticate via html forms and cookies, while human users use HTTP [Basic](http://tools.ietf.org/html/rfc2617) or [NTLM](http://tools.ietf.org/html/rfc4559). Even if you don't need to support rfc-2617 or rfc-4559 simultaneously with cookies, the separation of UI from authentication concerns allows you to test against `CookieAuthenticationHttpModule` with the intention of deploying to production with only e.g., `WindowsAuthenticationModule` enabled.

It's easy to use:
```
public class Global : HttpApplication {
    override public void Init() {
        base.Init();
        Find<CookieAuthenticationHttpModule>().Configure(appSettings);
    }
    public static System.Collections.Specialized.NameValueCollection appSettings {
        get {
            return ConfigurationManager.AppSettings;
        }
    }
```

But be very careful to provide your own private signing key. If you fail to provide a key, the module will generate its own, which can lead to confusing results in clustered environments or in the presence of normal ASP.NET worker process recycling.
```
<configuration>
  <appSettings>
    <!-- be sure to generate your own random key -->
    <add key="seanfoy.mvcutils.CookieAuthenticationHttpModule.signingKey" value="FB-6A-A1-2E-A8-01-66-A3-29-94-7A-EF-C3-79-6E-DC-C9-E0-CC-A2-35-A6-D3-A4-82-DF-61-4D-99-B8-67-D3-6C-01-D2-E4-98-4C-75-BD-48-F9-95-BF-BB-DA-15-74-8A-E3-BD-71-C6-09-5F-2A-5A-F2-53-D6-18-E3-CB-21" />
  </appSettings>
  <system.web>
    <httpModules>
      <add name="CookieAuthentication" type="seanfoy.mvcutils.CookieAuthenticationHttpModule,seanfoy.mvcutils" />
      <!-- ... -->
    </httpModules>

    <!-- Windows Authentication isn't required for CookieAuthenticationHttpModule, but it is permitted here: -->
    <authentication mode="Windows" />
  </system.web>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true">
      <add name="CookieAuthentication" type="seanfoy.mvcutils.CookieAuthenticationHttpModule,seanfoy.mvcutils" precondition="managedHandler" />
      <!-- ... -->
    </modules>
  </system.webServer>
</configuration>
```

## Optional Configuration ##

| appSettings key | meaning |
|:----------------|:--------|
| `seanfoy.mvcutils.CookieAuthenticationHttpModule.cookieName` | the name of the cookie issued and verified by the module. The default value is `seanfoy.mvcutils.CookieAuthenticationHttpModule-token`. |
| `seanfoy.mvcutils.CookieAuthenticationHttpModule.signingKey` | a sequence of 64 bytes represented as string images of hexadecimal octets separated by `-` |
| `seanfoy.mvcutils.CookieAuthenticationHttpModule.recessive` | A boolean value written as `true` or `false` indicating whether this module should defer to any other authentication modules that happen to process a request first. The default is `false`. |
| `seanfoy.mvcutils.CookieAuthenticationHttpModule.transferrable` | A boolean value written as `true` or `false` indicating whether cookies are valid only for the IP address to which they were issued. |

## Cookie validity ##

Cookies issued by this module include an expiration date, a client ip address, and a hash-based message authentication code. A cookie is valid if all of the following conditions are met:
  * its cryptographic signature is valid
  * it has not yet expired
  * it is presented by the client ip address to which it was issued, or the module is configured for transferrable cookies.

# `UrlAuthorizationHttpModule` #

This module restricts access to resources according to acls that associate permitted users (and roles) with allowed urls. Compared to the stock `System.Web.Security.UrlAuthorizationModule`, it offers greater flexibility in matching users against patterns and much greater flexibility in matching urls against patterns (`UrlAuthorizationHttpModule` matches urls with regular expressions). Compared to `System.Web.Mvc.AuthorizeAttribute`, it allows separate declaration of authorization rules from program logic: admins can adjust permissions by editing Web.config rather than by editing and recompiling your source code.

Use the module this way:
```
public class Global : HttpApplication {
    override public void Init() {
        base.Init();
        var urlauthzn =
            Find<CookieAuthenticationHttpModule>();
        urlauthzn.Configure(
            UrlAuthorizationHttpModule.GetSection(
                typeof(UrlAuthorizationHttpModule).Name));        
    }
}
```

Configuration is similar to the stock module, but more involved:
```
<configuration>
  <configSections>
    <section name="UrlAuthorizationHttpModule" type="seanfoy.mvcutils.UrlAuthorizationHttpModule+UrlAuthorizationConfigurationSection,seanfoy.mvcutils" />
  </configSections>
  <UrlAuthorizationHttpModule>
    <rules>
      <add urlPattern="admin">
        <acl>
          <add polarity="Deny" principalPattern="?" />
          <add polarity="Allow" principalEquality="seanfoy.mvcutils.UrlAuthorizationHttpModule+PrincipalRoleEquality,seanfoy.mvcutils" principalPattern="admins@domain.tld" />
        </acl>
      </add>
      <add urlPattern="(profile|inbox/.+)">
        <acl>
          <add polarity="Deny" principalPattern="?" />
          <add polarity="Allow" principalEquality="seanfoy.mvcutils.UrlAuthorizationHttpModule+PrincipalRoleEquality,seanfoy.mvcutils" principalPattern="users@domain.tld" />
          <add polarity="Allow" principalEquality="seanfoy.mvcutils.UrlAuthorizationHttpModule+PrincipalRoleEquality,seanfoy.mvcutils" principalPattern="admins@domain.tld" />
        </acl>
      </add>
    </rules>
  </UrlAuthorizationHttpModule>

  <system.web>
    <httpModules>
      <add name="seanfoy.mvcutils.UrlAuthorization" type="seanfoy.mvcutils.UrlAuthorizationHttpModule,seanfoy.mvcutils" />
      <!-- ... -->
    </httpModules>
  </system.web>
  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true">
      <add name="UrlAuthorization" type="seanfoy.mvcutils.UrlAuthorizationHttpModule,seanfoy.mvcutils" precondition="managedHandler" />
      <!-- ... -->
    </modules>
  </system.webServer>
</configuration>
```

## `PrincipalEquality` ##

The built-in `PrincipalEquality` relations are:
| `PrincipalEquality` | `*` matches any request; `?` matches only the anonymous user; behavior is undefined for other patterns |
|:--------------------|:-------------------------------------------------------------------------------------------------------|
| `PrincipalNameICEquality` | The specified pattern is compared to the authenticated `IIdentity.Name` using `StringComparison.OrdinalIgnoreCase` |
| `PrincipalNameEquality` | The specified pattern is compared to the authenticated `IIdentity.Name` using `StringComparison.Ordinal` (exact matching) |
| `PrincipalRoleEquality` | The authenticated user is tested for membership in the specified role                                  |