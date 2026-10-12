# CSRF Vulnerability with No Defenses
**Platform:** PortSwigger Web Security Academy  
**Category:** Cross-site request forgery (CSRF)  
**Official lab:** https://portswigger.net/web-security/csrf/lab-no-defenses  
**Status:** Solved

---

## Objective

Determine whether a state-changing request could be forged from another site because it relied only on the automatically-sent session cookie and had no anti-CSRF protection.

## Observation and Methodology

The state-changing request (an email change) was authenticated purely by the session cookie, which the browser attaches automatically, and carried no anti-CSRF token or equivalent check. I built a proof-of-concept page on the exploit server that submits the same request; when the authenticated victim loads it, the action is performed as them. I confirmed the action via the lab's exploit delivery.

## Root Cause

The request had no unpredictable, request-scoped value tying it to the user's session, and the server accepted it based solely on the ambient cookie. Because cookies are sent automatically cross-site, the request could be forged.

## Security Impact

An attacker can cause an authenticated user to perform unwanted state-changing actions (here, changing their email, which can lead to account takeover) simply by getting them to visit a page.

## Remediation

Use an anti-CSRF token (synchroniser pattern) that is unpredictable and validated server-side on every state-changing request. Set `SameSite` on session cookies (Lax or Strict) as strong defence in depth, and consider re-authentication for sensitive actions. Framework-provided CSRF protection should be enabled rather than hand-rolled.

## Relevance to My Development Background

ASP.NET provides antiforgery tokens (`[ValidateAntiForgeryToken]` / the antiforgery middleware). This lab is a reminder to keep that enabled for cookie-authenticated state-changing endpoints and to set SameSite on the auth cookie. Token-in-header APIs are structurally less exposed because the token is not sent automatically.

## Security Lessons

- CSRF exists because browsers attach cookies automatically to cross-site requests.
- The control is a per-request unpredictable token, and/or SameSite cookies.
- Only state-changing requests need CSRF protection, but all of them do.

## Evidence

- `screenshots/01-solved.png` — Lab solved
- `screenshots/02-exploit-server-poc.png` — Proof-of-concept form hosted on the exploit server

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
