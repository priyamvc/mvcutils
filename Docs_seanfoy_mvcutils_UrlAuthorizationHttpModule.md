
```
public class UrlAuthorizationHttpModule : seanfoy.mvcutils.HttpModuleBase
```

# Remarks #
To be added.

**Assembly:** seanfoy.mvcutils 0.0.10.0

# Members #
_List of all members,_

## Constructors ##
| `public UrlAuthorizationHttpModule ();`  | To be added. |
|:-----------------------------------------|:-------------|
| `public UrlAuthorizationHttpModule (seanfoy.mvcutils.UrlAuthorizationHttpModule.UrlAuthorizationConfigurationSection config);`  | To be added. |

## Fields ##
| `public System.Collections.Generic.KeyValuePair&lt;System.Text.RegularExpressions.Regex,System.Collections.Generic.List&lt;seanfoy.mvcutils.UrlAuthorizationHttpModule.ACLEntry&gt;&gt; defaultRule;`  | To be added. |
|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|:-------------|
| `public System.Collections.Generic.Dictionary&lt;System.Text.RegularExpressions.Regex,System.Collections.Generic.List&lt;seanfoy.mvcutils.UrlAuthorizationHttpModule.ACLEntry&gt;&gt; rules;`          | To be added. |

## Methods ##
| `public static string AppSpecificUrlStem (System.Web.HttpContextBase ctx, string canonicalUrl);`  | To be added. |
|:--------------------------------------------------------------------------------------------------|:-------------|
| `public seanfoy.mvcutils.UrlAuthorizationHttpModule.ACLEntry authorize (System.Web.HttpContextBase ctx, System.Web.Routing.RouteCollection routes);`  | To be added. |
| `public void authorizeRequest (object sender, EventArgs args);`                                   | To be added. |
| `public static string CanonicalRequestUrl (System.Web.HttpContextBase ctx, System.Web.Routing.RouteCollection routes);`  | To be added. |
| `public void Configure (seanfoy.mvcutils.UrlAuthorizationHttpModule.UrlAuthorizationConfigurationSection config);`  | To be added. |
| `public static seanfoy.mvcutils.UrlAuthorizationHttpModule.UrlAuthorizationConfigurationSection GetSection (string name);`  | To be added. |
| `public override void Init (System.Web.HttpApplication app);`                                     | To be added. |


# Member Details #
_A detailed description of each member_

## Constructors ##
_A detrailed description of constructors._

### UrlAuthorizationHttpModule Constructor ###
_To be added._
```
public UrlAuthorizationHttpModule ();
```

#### Remarks ####
To be added.

#### Parameters ####
### UrlAuthorizationHttpModule Constructor ###
_To be added._
```
public UrlAuthorizationHttpModule (seanfoy.mvcutils.UrlAuthorizationHttpModule.UrlAuthorizationConfigurationSection config);
```

#### Remarks ####
To be added.

#### Parameters ####
  * seanfoy.mvcutils.UrlAuthorizationHttpModule+UrlAuthorizationConfigurationSection `config`  To be added.

## Fields ##
_A detailed description of fields._

### defaultRule ###
_To be added._
```
public System.Collections.Generic.KeyValuePair&lt;System.Text.RegularExpressions.Regex,System.Collections.Generic.List&lt;seanfoy.mvcutils.UrlAuthorizationHttpModule.ACLEntry&gt;&gt; defaultRule;
```
#### Remarks ####
To be added.

### rules ###
_To be added._
```
public System.Collections.Generic.Dictionary&lt;System.Text.RegularExpressions.Regex,System.Collections.Generic.List&lt;seanfoy.mvcutils.UrlAuthorizationHttpModule.ACLEntry&gt;&gt; rules;
```
#### Remarks ####
To be added.

## Methods ##
_A detailed description of methods._

### AppSpecificUrlStem ###
_To be added._
```
public static string AppSpecificUrlStem (System.Web.HttpContextBase ctx, string canonicalUrl);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpContextBase `ctx`  To be added.
  * System.String `canonicalUrl`  To be added.

### authorize ###
_To be added._
```
public seanfoy.mvcutils.UrlAuthorizationHttpModule.ACLEntry authorize (System.Web.HttpContextBase ctx, System.Web.Routing.RouteCollection routes);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpContextBase `ctx`  To be added.
  * System.Web.Routing.RouteCollection `routes`  To be added.

### authorizeRequest ###
_To be added._
```
public void authorizeRequest (object sender, EventArgs args);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Object `sender`  To be added.
  * System.EventArgs `args`  To be added.

### CanonicalRequestUrl ###
_To be added._
```
public static string CanonicalRequestUrl (System.Web.HttpContextBase ctx, System.Web.Routing.RouteCollection routes);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpContextBase `ctx`  To be added.
  * System.Web.Routing.RouteCollection `routes`  To be added.

### Configure ###
_To be added._
```
public void Configure (seanfoy.mvcutils.UrlAuthorizationHttpModule.UrlAuthorizationConfigurationSection config);
```
#### Remarks ####
To be added.

#### Parameters ####
  * seanfoy.mvcutils.UrlAuthorizationHttpModule+UrlAuthorizationConfigurationSection `config`  To be added.

### GetSection ###
_To be added._
```
public static seanfoy.mvcutils.UrlAuthorizationHttpModule.UrlAuthorizationConfigurationSection GetSection (string name);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.String `name`  To be added.

### Init ###
_To be added._
```
public override void Init (System.Web.HttpApplication app);
```
#### Remarks ####
To be added.

#### Parameters ####
  * System.Web.HttpApplication `app`  To be added.