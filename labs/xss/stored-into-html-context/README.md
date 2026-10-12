# Stored XSS into HTML Context with Nothing Encoded
**Platform:** PortSwigger Web Security Academy  
**Category:** Cross-site scripting (XSS) — stored  
**Official lab:** https://portswigger.net/web-security/cross-site-scripting/stored/lab-html-context-nothing-encoded  
**Status:** Solved

---

## Objective

Determine whether user-submitted content was stored and later rendered to other users without encoding, allowing persistent script execution.

## Observation and Methodology

I submitted a script payload into a persistent input field. When the page that renders stored content was viewed, the payload executed, confirming stored XSS. Unlike reflected XSS, no crafted link is needed — the payload persists and runs for every viewer of the affected content.

## Root Cause

Stored user input was rendered into the HTML context without output encoding, so saved attacker input was served as executable markup to subsequent viewers.

## Security Impact

Stored XSS is typically higher impact than reflected: it executes for every user who views the content, with no social-engineering step, and can be used to attack administrators or spread between users.

## Remediation

Encode output on render, per context, including content that was previously stored. Do not rely on input-time filtering alone. Add a Content-Security-Policy and `HttpOnly` session cookies as defence in depth.

## Relevance to My Development Background

On the Angular/.NET stack I work in, the defensive points are output encoding in the server templates and not bypassing Angular's built-in sanitisation. Angular escapes interpolated values by default; the risk appears when that protection is deliberately bypassed (for example `bypassSecurityTrustHtml` or `[innerHTML]` with unsanitised input). A Content-Security-Policy adds defence in depth.

## Security Lessons

- Stored XSS needs no victim interaction beyond viewing the page.
- Encoding must happen on output even for data already in the database.
- Persisted content is still untrusted input at render time.

## Evidence

- `screenshots/01-solved.png` — Lab solved; stored payload executing on render

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
