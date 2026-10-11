# Unprotected Administrative Functionality

## Lab

PortSwigger Web Security Academy — `[Add exact lab name]`

**Vulnerability class:** Broken Access Control
**Date tested:** `[Add date]`

---

## Objective

Determine whether administrative functionality within the application was properly restricted
to administrative users, or whether it could be reached by a user without administrative
privileges.

---

## Reconnaissance

I began by looking at what the application exposed about its own structure rather than at the
interface it presented.

I inspected the application's publicly accessible resources and found `/robots.txt`.

`robots.txt` is intended to tell search engine crawlers which paths not to index. It is a
convention for crawler behaviour and carries no access-control meaning. Because its purpose is
to name paths that the application owner would prefer not to be indexed, it frequently names
exactly the paths that are most interesting to a tester.

In this case, `robots.txt` disclosed a path associated with administrative functionality:

```
[Add the relevant robots.txt entry as observed]
```

`[Add screenshot of /robots.txt]`

---

## Initial Observation

I navigated directly to the disclosed administrative path while authenticated as a normal,
non-administrative user.

The administrative functionality was reachable. The application did not redirect the request,
did not return an access-denied response, and did not otherwise prevent the page from being
served.

`[Add screenshot of administrator panel]`

---

## Testing Methodology

1. **Enumerate exposed resources.** I reviewed publicly accessible files served by the
   application, which is how `/robots.txt` was identified.
2. **Extract candidate paths.** I noted the path relating to administrative functionality
   disclosed in `robots.txt`.
3. **Establish a non-privileged context.** I was authenticated as a standard user account with
   no administrative role.
4. **Request the privileged resource directly.** I requested the administrative path from that
   non-privileged context, bypassing the application's navigation entirely — the interface did
   not present a link to it.
5. **Observe the response.** I recorded whether the application served the administrative
   functionality or denied the request.

`[Add Burp request and response for the administrative path request, if captured]`

I did not attempt to escalate further or exercise administrative actions beyond confirming
that the functionality was reachable.

---

## Result

The administrative panel was accessible to a user who did not hold administrative privileges.

The application served the privileged functionality on request. Access to it depended entirely
on knowing the path — not on holding the role that the functionality was intended for.

---

## Vulnerability Classification

**Broken Access Control — Unprotected Administrative Functionality**

The application relied on the administrative path being undisclosed rather than on a
server-side check of the requesting user's privileges.

---

## Authentication vs Authorisation

This finding is an **authorisation** failure, not an authentication failure. The distinction
is central to the finding.

**Authentication asks: "Who are you?"**
The application performed this correctly. I was logged in, the application identified my
session, and it knew which account the request came from.

**Authorisation asks: "Are you allowed to access this resource or perform this action?"**
The application did not perform this check on the administrative endpoint.

The user in this case was **authenticated but not authorised**. Identity had been established
and was never questioned. What was missing was the second decision — whether that established
identity carried the privilege required for this particular resource.

This matters because the two are often conflated. An application can have a completely sound
login mechanism, strong password policy, and multi-factor authentication, and still expose
administrative functionality to every logged-in user. Proving who someone is says nothing
about what they may do.

---

## Security Impact

The immediate impact is that administrative functionality was exposed to a user without
administrative privileges.

The severity of that exposure depends entirely on what the administrative functionality
permits. Where administrative functions allow account management, data modification, privilege
changes or configuration changes, the impact of unrestricted access would be significant.
Where they are limited to read-only information, the impact would be correspondingly lower.

I am not overstating this: I confirmed that the panel was reachable, and I did not enumerate
or exercise the full set of administrative actions available. **The exact impact depends on
what administrative capability the application exposes behind that panel.**

The structural point stands regardless of severity. Any user able to guess, discover, or be
told the path had the same level of access, and the application had no mechanism to
distinguish between them.

---

## Root Cause

Server-side authorisation checks were not enforced on the administrative functionality.

The application appears to have treated the administrative path as protected because it was
not linked from the user interface and was not intended to be discovered. That is an
assumption about user behaviour, not a control. The server received a request for the path and
served it without evaluating whether the requesting user's privilege level permitted it.

Where the request originated — a navigation link, a typed URL, or a manually constructed
request — is not something the server can determine and not something it should depend on.

---

## Remediation

**Enforce server-side authorisation on every privileged endpoint and function.**

Specifically:

1. **Check privileges on the server, per request.** Every request to administrative
   functionality must independently verify that the authenticated user holds the required
   role or permission before any privileged content or action is served.
2. **Deny by default.** Access should be refused unless a check explicitly grants it, rather
   than permitted unless a check explicitly refuses it. A missing check should fail closed.
3. **Apply the check to the function, not the navigation.** Removing a link from the interface
   changes what users are shown, not what the server will do.
4. **Return an appropriate response to unauthorised requests.** A `403` denying the action,
   without the privileged content appearing anywhere in the response body.
5. **Centralise the enforcement.** Applying authorisation consistently at a framework or
   middleware level is more reliable than remembering to add a check to each new endpoint
   individually.

Access control must **not** be based on:

- `robots.txt` entries
- URL obscurity or unlinked paths
- Client-side controls — hidden menu items, disabled buttons, or front-end route guards
- The assumption that users will only reach pages through the interface provided

Each of these describes what a user is likely to do. None of them constrains what a user is
able to do.

---

## Security Lessons Learned

- **Discoverability is not authorisation.** Whether a resource is easy to find and whether a
  user is permitted to access it are unrelated questions. The application answered the first
  and never asked the second.
- **`robots.txt` is not an access-control mechanism.** It is a crawler directive. Listing a
  path there publishes its existence to anyone who reads the file, which includes anyone
  testing the application.
- **Authentication and authorisation are distinct controls.** Verifying identity is not
  verifying permission, and an application can perform the first flawlessly while omitting the
  second entirely.
- **Authorisation must be enforced server-side.** Any check that runs only in the browser is a
  presentation decision.
- **Sensitive functionality requires an explicit privilege check.** Absence of a link is not a
  control. The check has to exist, run on the server, and run on every request.
- **Reconnaissance before interaction is productive.** The finding came from reading what the
  application published about itself, before touching the interface at all.

---

## Evidence

`[Add screenshot of /robots.txt]`

`[Add screenshot of administrator panel]`

`[Add Burp request/response if available]`

---

## Ethical Scope

This testing was performed exclusively against the authorised PortSwigger Web Security Academy
lab environment, which is provided expressly for security testing practice.

No testing was performed against any system I do not own or have explicit authorisation to
test. No lab solutions have been reproduced here — the reasoning and methodology documented
above are my own.
