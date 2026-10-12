# Reflected XSS into HTML Context with Nothing Encoded
**Platform:** PortSwigger Web Security Academy  
**Category:** Cross-site scripting (XSS) — reflected  
**Official lab:** https://portswigger.net/web-security/cross-site-scripting/reflected/lab-html-context-nothing-encoded  
**Status:** Solved

---

## Objective

Determine whether a search parameter was reflected into the HTML response without encoding, allowing injected markup to execute as script in the victim's browser.

## Observation and Methodology

The search term was reflected back into the page's HTML without output encoding. Submitting a script payload through the parameter caused the browser to execute it, which I confirmed by the resulting alert. Because the input is reflected from the request, delivery would be via a crafted link.

## Root Cause

User input was written into the HTML response without context-appropriate output encoding, so the browser parsed attacker input as markup and script rather than as text.

## Security Impact

Reflected XSS runs attacker script in the victim's browser within the application's origin. That allows session/cookie theft (where not HttpOnly), actions performed as the victim, and credential capture. XSS matters because it executes inside the application's origin — a same-origin-policy violation in effect.

## Remediation

Apply context-aware output encoding wherever user input is rendered (HTML, attribute, JavaScript and URL contexts each differ). Validate input where feasible, set a strong Content-Security-Policy as defence in depth, and mark session cookies `HttpOnly` so script cannot read them.

## Relevance to My Development Background

On the Angular/.NET stack I work in, the defensive points are output encoding in the server templates and not bypassing Angular's built-in sanitisation. Angular escapes interpolated values by default; the risk appears when that protection is deliberately bypassed (for example `bypassSecurityTrustHtml` or `[innerHTML]` with unsanitised input). A Content-Security-Policy adds defence in depth.

## Security Lessons

- XSS is an output-encoding problem: encode on output, per context.
- The alert box is only a marker; the real risk is code running in your origin.
- CSP and HttpOnly reduce impact but do not replace encoding.

## Evidence

- `screenshots/01-solved.png` — Lab solved
- `screenshots/02-burp.png` — Burp request/response evidence
- `screenshots/03-alert-fired.png` — Injected script executing in the browser

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
