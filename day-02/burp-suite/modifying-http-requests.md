# Day 02 — Modifying HTTP Requests

**Source:** Official PortSwigger Burp Suite documentation and beginner practical exercises

---

## Objective

Confirm in practice that a request can be altered after it leaves the browser and before it
reaches the server, and observe how the application responds to input it did not expect to
receive.

---

## What I Practiced

- Intercepting a request in the Proxy tab
- Editing values within the held request before forwarding it
- Forwarding the modified request and reviewing the response
- Reissuing a request with further modifications to compare responses

Reissuing was the part I found most useful. Rather than repeating a browser action for every
variation, the same request can be sent repeatedly with a single value changed each time,
which makes it possible to isolate what actually caused a change in the response.

---

## Original Request

```http
[Paste actual request from Burp]
```

---

## Modification

`[Describe exactly what value was modified — the parameter or header name, the original value,
and the value it was changed to]`

---

## Result

`[Describe observed response — status code, notable headers, and what changed in the response
body compared to the unmodified request]`

`[Add screenshot of modified request and response]`

---

## Security Relevance

The browser applies constraints — field lengths, input types, dropdown options, disabled
controls, client-side validation. None of those constraints exist once the request is in
transit. The server receives a sequence of bytes and has no way to know whether they came from
the interface it served or from a proxy.

This means:

- **Client-side validation is a usability feature, not a security control.** It improves the
  experience for legitimate users and stops nothing else.
- **Every parameter is an input the server must validate independently**, including values the
  interface never allowed the user to change.
- **Hidden fields and read-only fields are neither hidden nor read-only** at the protocol
  layer.

Coming from development, this is the point that reframes things for me. Validation logic I
would have written in an Angular form is not a barrier to anything — it has to be duplicated
and enforced server-side, because the server-side check is the only one that exists as far as
an attacker is concerned.

---

## Key Takeaways

- Requests can be freely modified in transit; the browser is not a boundary
- Client-side validation must be treated as unenforced
- Every parameter reaching the server requires independent server-side validation
- Reissuing requests with single-value changes is how a specific cause is isolated from a
  general observation

---

## Ethical Scope

All request modification was performed against authorised lab environments only.
