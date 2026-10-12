# URL-Based Access Control Can Be Circumvented
**Platform:** PortSwigger Web Security Academy  
**Category:** Broken access control — front-end URL matching  
**Official lab:** https://portswigger.net/web-security/access-control/lab-url-based-access-control-can-be-circumvented  
**Status:** In progress — not yet confirmed solved

---

## Objective

Examine whether access control enforced at a front-end layer on the request path could be bypassed using a header that overrides the path the back-end sees.

## Observation and Methodology

Access to the administrative path was blocked at the front end, returning `HTTP 403 Access denied`. I was investigating whether a header that rewrites the effective URL for the back-end (`X-Original-URL`) could reach administrative functionality that the front-end filter was protecting. The captured evidence currently shows the lab in a **Not solved** state: a 403 on the blocked path, and a request using the override header. I have not captured a confirmed successful administrative action or a solved banner, so this lab is documented honestly as in progress and will be updated once I reproduce and capture a solved result. The session cookie in the request screenshot is redacted.

## Root Cause

The access control decision was made at a front-end component based on the requested URL, while the back-end honoured a header that overrode the effective path. When two layers disagree about what the requested resource is, a control enforced on one layer can be bypassed via the other.

## Security Impact

If exploitable, this allows an unauthenticated or unprivileged user to reach administrative functionality that the front-end believed it was protecting. As I have not confirmed the successful action, I am not claiming impact beyond the observed 403 and the override-header behaviour under test.

## Remediation

Do not enforce access control solely at a front-end layer on the request path. Enforce authorisation at the application/back-end layer where the action is actually performed, based on the authenticated user's privileges, not on the URL string. Ensure all layers agree on the effective path, and ignore or strip untrusted path-override headers (`X-Original-URL`, `X-Rewrite-URL`) unless they are explicitly trusted and sanitised.

## Security Lessons

- Access control enforced only on the front end can be bypassed if the back-end resolves the resource differently.
- Path-override headers are attacker-controlled input and must not silently change authorisation outcomes.
- Document honestly: an unconfirmed lab stays in progress until the successful result is captured.

## Evidence

- `screenshots/01-not-solved.png` — Lab shown Not solved (status recorded honestly)
- `screenshots/02-burp-repeater.png` — Burp Repeater during testing
- `screenshots/03-response-403-access-denied.png` — HTTP 403 Access denied on the protected path
- `screenshots/04-request-x-original-url-redacted.png` — Request using X-Original-URL override header (session cookie redacted)
- `screenshots/05-response-302-location-admin.png` — HTTP 302 Location: /admin observed during testing

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
