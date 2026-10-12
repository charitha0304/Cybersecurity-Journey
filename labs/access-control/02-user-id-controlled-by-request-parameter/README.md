# User ID Controlled by Request Parameter (IDOR)
**Platform:** PortSwigger Web Security Academy  
**Category:** Broken access control — Insecure Direct Object Reference (IDOR)  
**Official lab:** https://portswigger.net/web-security/access-control/lab-user-id-controlled-by-request-parameter  
**Status:** Solved

---

## Objective

Determine whether the account page enforced that a user can only view their own account, or whether the record returned was selected purely from a user-supplied identifier.

## Observation and Methodology

The account page retrieved the displayed account from an identifier carried in the request rather than from the authenticated session. By changing that identifier to another user's value, the application returned a different user's account page, including that user's API key. I confirmed access to another user's data and did not reuse or retain the exposed key. The API key is redacted in the committed screenshot.

## Root Cause

The server used a client-supplied identifier to select the account record and did not verify that the record belonged to the authenticated user. Identity (the session) and the object being requested (the account id) were never checked against each other.

## Security Impact

Any authenticated user could read another user's account data by supplying that user's identifier. Where such a page also exposes secrets — here an API key — the exposure extends to credentials that could be used elsewhere. The impact scales with how sensitive the returned record is.

## Remediation

Enforce an ownership check on the server for every record access: the authenticated user's identity must match (or be explicitly authorised for) the object being requested, rather than trusting an identifier from the request. Prefer deriving the account from the session wherever possible. Where a direct reference is unavoidable, apply a per-object authorisation check, and consider non-sequential, unguessable identifiers as defence in depth (not as the control itself).

## Relevance to My Development Background

This is the most common class of bug I expect to meet in a .NET API. It maps directly to a controller action such as `GET /api/account/{id}` that loads `_context.Accounts.Find(id)` and returns it with only `[Authorize]` applied — authentication without an ownership predicate. The fix in ASP.NET is a resource-based authorisation check (for example `IAuthorizationService.AuthorizeAsync` against the resource) or scoping the query to the current user, e.g. filtering on the user id taken from the validated token rather than from the route.

## Security Lessons

- Access control decisions must bind the session identity to the specific object requested.
- A value in the request is attacker-controlled; selecting a record from it without an ownership check is broken access control.
- Unguessable IDs reduce discoverability but are not a substitute for an authorisation check.

## Evidence

- `screenshots/01-solved-apikey-redacted.png` — Lab solved; another user's account page returned (API key redacted before commit)

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
