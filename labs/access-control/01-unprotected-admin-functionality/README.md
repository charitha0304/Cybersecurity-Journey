# Unprotected Administrative Functionality
**Platform:** PortSwigger Web Security Academy  
**Category:** Broken access control — unprotected admin functionality  
**Official lab:** https://portswigger.net/web-security/access-control/lab-unprotected-admin-functionality  
**Status:** Solved

---

## Objective

Determine whether administrative functionality was restricted to administrators, or whether it could be reached by a user without administrative privileges.

## Observation and Methodology

I began with reconnaissance of the application's exposed resources rather than the interface. I inspected `/robots.txt`, which is a crawler directive with no access-control meaning, and it disclosed a path associated with administrative functionality. I requested that path directly while authenticated as a standard, non-administrative user. The application served the administrative panel: it did not redirect, did not return an access-denied response, and did not otherwise prevent access. I confirmed the panel was reachable and did not exercise destructive administrative actions beyond that confirmation.

## Root Cause

Server-side authorisation was not enforced on the administrative functionality. The application relied on the path being undisclosed rather than on a privilege check. The server received the request and served the resource without evaluating whether the authenticated user held the required role.

## Security Impact

Administrative functionality was exposed to a user without administrative privileges. The severity depends on what the panel permits — account management, privilege changes or data modification would be high impact; read-only functions lower. I confirmed reachability and did not enumerate the full set of administrative actions, so the exact impact depends on the panel's capability.

## Remediation

Enforce a server-side authorisation check on every privileged endpoint and function, evaluated per request. Deny by default, so a missing check fails closed. Apply the check to the function, not the navigation — removing a link changes what users are shown, not what the server does. Return a 403 without the privileged content appearing in the body. Do **not** base access control on robots.txt entries, unlinked or obscure URLs, or client-side controls.

## Relevance to My Development Background

In ASP.NET this is the difference between `[Authorize]` and `[Authorize(Roles="Admin")]`, and between an action that assumes it is unreachable because no link points to it and one that actually checks the caller's role. Centralising the check (policy-based authorisation applied at a middleware or convention level) is more reliable than remembering to annotate each new admin endpoint.

## Security Lessons

- Discoverability is not authorisation: whether a resource is easy to find and whether a user may access it are unrelated questions.
- `robots.txt` is not an access-control mechanism; it publishes paths to anyone who reads it.
- Authentication and authorisation are distinct controls; this is an authorisation failure — the user was authenticated but not authorised.
- Authorisation must be enforced server-side, on every request.

## Evidence

- `screenshots/01-solved.png` — Lab solved; administrative functionality reached as a non-admin user

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
