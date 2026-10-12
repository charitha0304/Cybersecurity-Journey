# DOM XSS in document.write Sink Using Source location.search
**Platform:** PortSwigger Web Security Academy  
**Category:** Cross-site scripting (XSS) — DOM-based  
**Official lab:** https://portswigger.net/web-security/cross-site-scripting/dom-based/lab-document-write-sink  
**Status:** Solved

---

## Objective

Determine whether client-side JavaScript passed a value from the URL (`location.search`) into a dangerous sink (`document.write`) without sanitisation, allowing script execution entirely in the browser.

## Observation and Methodology

I traced the search value from the URL source (`location.search`) into the `document.write` sink in the page's own JavaScript. Supplying input that broke out of the surrounding context caused script to execute. The flaw is entirely client-side — the vulnerable data flow is in the browser, which is why it does not appear in server logs.

## Root Cause

Client-side code took attacker-controllable input from a URL source and wrote it into a sink that parses HTML/script, with no sanitisation along that data flow.

## Security Impact

DOM-based XSS executes in the victim's browser within the application's origin, with the same consequences as other XSS. It can be missed by purely server-side testing because the vulnerable flow never reaches the server.

## Remediation

Avoid dangerous sinks (`document.write`, `innerHTML`, etc.) with untrusted data; prefer safe DOM APIs (`textContent`) and framework-provided escaping. Sanitise and encode untrusted URL data before it reaches a sink, and apply a Content-Security-Policy. Review client-side source-to-sink flows, not just server code.

## Relevance to My Development Background

On the Angular/.NET stack I work in, the defensive points are output encoding in the server templates and not bypassing Angular's built-in sanitisation. Angular escapes interpolated values by default; the risk appears when that protection is deliberately bypassed (for example `bypassSecurityTrustHtml` or `[innerHTML]` with unsanitised input). A Content-Security-Policy adds defence in depth.

## Security Lessons

- DOM XSS lives in client-side data flows, invisible to server logs.
- The fix is avoiding dangerous sinks and encoding before the sink, not server-side filtering.
- Source-to-sink review is a distinct skill from server-side testing.

## Evidence

- `screenshots/01-solved-with-devtools.png` — Lab solved; source-to-sink flow confirmed with developer tools

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
