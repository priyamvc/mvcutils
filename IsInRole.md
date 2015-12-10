# Introduction #

I claim that [IPrincipal.IsInRole](http://msdn.microsoft.com/en-us/library/system.security.principal.iprincipal.isinrole.aspx) [should return false whenever it determines that a user is not in a role](http://seanfoy.blogspot.com/2009/09/trust-relationship-between-primary.html).

# Details #

On some versions of Windows, however, a role name such as "xyz" -- which does not match any of the expected forms for Windows role names ("xyz@bigco.com", "bigco\xyz") -- causes `IsInRole` to throw a `SystemException`: The trust relationship between the primary domain and the trusted domain failed. See my [blog post](http://seanfoy.blogspot.com/2009/09/trust-relationship-between-primary.html) for details.

Others claim to have observed similar behavior [due to insufficient permissions to check role membership](http://social.msdn.microsoft.com/forums/en-US/clr/thread/f7c1a1ec-106c-4fec-b8ad-2e126591099a). Also, Microsoft has a [hotfix](http://support.microsoft.com/kb/976494) for these [symptoms](http://social.msdn.microsoft.com/Forums/en/vblanguage/thread/75f7964b-c90f-487d-ae0c-82678dbe0c6e) on Windows 2008 and Windows 7.

When permissions are insufficient to check role membership, or some `ActiveDirectory` service is unreachable, perhaps `IsInRole` _should_ throw an exception, because it can't prove an answer for the question. On the other hand, I also think a good case can be made to define the method to return true when authorization can be proven and false when it cannot. Either way, I think behavior should be documented.

When `WindowsPrincipal.IsInRole` is given a role name that does not obey the naming rules for Windows role names, isn't that probably a sign of buggy code or incorrect configuration? Shouldn't the app complain loudly (as in raise an exception) rather than silently returning false? Not if you have a diverse population of principals, some of whom are not `WindowsPrincipals` and may have contrary role-naming rules. The behavior of `WindowsPrincipal` breaks the implicit contract of `IPrincipal`, in my opinion.

You might wonder why `MvcUtils.IsInRole` catches only very particular exceptions, converting them to false return values, and allows other exceptions to propagate. The answer is that I've only investigated this one category of exceptions. I'm reluctant to second-guess Microsoft in their judgment about what's "exceptional," so although I think `IsInRole` should always answer true or false, perhaps logging diagnostic data for error conditions when role membership couldn't be determined, I'm willing to see more actual cases before I automate a more general policy.

**As of version 0.0.7.0, `MvcUtils.IsInRole` is intended to return true when the principal has the role, and false when the principal does not have the role, and its behavior is undefined when role membership is indeterminate**. I'm inclined to let the contract shift to "returns true when the principal has proven role membership and false otherwise," in the future.