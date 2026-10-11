# Day 01 — Web Fundamentals

**Platform:** TryHackMe
**Module:** [Add exact room name]

---

## Objective

Establish a working understanding of how web applications communicate, so that later
vulnerability testing rests on an understanding of the protocol rather than on memorised
attack steps. My goal was not to learn definitions, but to be able to look at a raw HTTP
request and say what each part does and what an attacker controls.

---

## Topics Covered

- HTTP and HTTPS
- The request/response model
- HTTP methods
- URL structure and parameters
- Request and response headers
- Cookies and session handling
- HTTP status codes
- Authentication vs authorisation
- General web application structure

---

## HTTP Request and Response

Web applications communicate over a request/response model. The client sends a request; the
server returns a response. HTTP is **stateless** — the server retains no memory of previous
requests. Every request must carry whatever identity or context the server needs.

This single property explains most of session management. Cookies, tokens and session
identifiers all exist to work around statelessness, and most authentication weaknesses are
failures in that workaround rather than failures of HTTP itself.

A request consists of a request line (method, path, version), headers, and an optional body.
A response consists of a status line, headers, and usually a body.

HTTPS is HTTP carried inside a TLS-encrypted connection. It protects the request in transit —
it does not make the application logic inside it any more secure. An application with broken
authorisation is equally broken over HTTPS.

---

## HTTP Methods

| Method | Intended use |
|---|---|
| GET | Retrieve a resource; should not change server state |
| POST | Submit data; typically changes state |
| PUT | Replace a resource |
| PATCH | Partially update a resource |
| DELETE | Remove a resource |
| HEAD | Same as GET but returns headers only |
| OPTIONS | Ask which methods the server permits |

The security-relevant point is that these are **conventions, not enforcement**. A server can
be implemented to change state on a GET request, or to accept a POST body on an endpoint the
developer assumed was read-only. Whether a method is permitted, and what it is allowed to do,
is decided by server-side code — not by the method name.

---

## URL and Parameters

A URL breaks down into scheme, host, port, path, query string, and fragment.

Two points I want to remember:

- Every part of the URL is **client-controlled**. The path, the query parameters and any
  identifiers inside them can be modified freely by the user. Any assumption the server makes
  about their values is an assumption an attacker can break.
- The **fragment** (the part after `#`) is never sent to the server. It is processed only by
  the browser. This means anything that happens as a result of a fragment value will not
  appear in server logs.

---

## Headers

Headers carry metadata about the request or response.

Request headers I paid attention to:

- `Host` — tells the server which site is being requested. Attacker-controllable.
- `Cookie` — carries session state, attached automatically by the browser.
- `Authorization` — carries credentials or tokens; not attached automatically.
- `User-Agent` — client identification; frequently trusted more than it should be.
- `Referer` — the originating page; sometimes used for access decisions, which is unreliable.

Response headers I paid attention to:

- `Set-Cookie` — issues a cookie and its attributes.
- `Content-Type` — how the browser should interpret the body.
- `Location` — redirect target.

Headers are part of the request, which means they are part of the attack surface. Anything the
server reads from a header is user input.

---

## Cookies

Cookies are set by the server with `Set-Cookie` and returned by the browser on subsequent
requests to a matching domain. They are the most common mechanism for maintaining a session
across an otherwise stateless protocol.

Attributes that matter:

| Attribute | Effect |
|---|---|
| `HttpOnly` | JavaScript cannot read the cookie |
| `Secure` | Cookie is only sent over HTTPS |
| `SameSite` | Controls whether the cookie is sent on cross-site requests |
| `Domain` / `Path` | Scope of the cookie |
| `Expires` / `Max-Age` | Lifetime |

The behaviour that stood out to me: the browser attaches cookies **automatically**, without
the application asking. A request originating from anywhere the browser considers valid will
carry the session cookie with it. That automatic attachment is the mechanism behind
cross-site request forgery, and `SameSite` exists to constrain it.

---

## HTTP Status Codes

| Range | Meaning |
|---|---|
| 1xx | Informational |
| 2xx | Success |
| 3xx | Redirection |
| 4xx | Client error |
| 5xx | Server error |

Specific codes worth knowing for security testing:

- **200** — succeeded. If a request I expected to be blocked returns 200, that is a finding.
- **301 / 302** — redirect. An application that redirects an unauthorised user but still
  returns the sensitive content in the response body has not actually blocked anything.
- **401 Unauthorized** — despite the name, this means *unauthenticated*: the server does not
  know who you are.
- **403 Forbidden** — the server knows who you are and is refusing the action. This is the
  authorisation failure response.
- **404** — not found, though applications sometimes return 404 to hide resources rather than
  to deny access.
- **500** — server error, often revealing that unexpected input reached code that could not
  handle it.

The distinction between 401 and 403 is a useful shorthand for the authentication/authorisation
distinction below.

---

## Authentication vs Authorisation

These are separate controls and the difference matters:

**Authentication — "Who are you?"**
Verifying identity. Logging in with credentials, presenting a token, completing MFA.

**Authorisation — "Are you allowed to do this?"**
Deciding whether the identified user is permitted to perform this action or access this
resource.

They fail independently. A user can be perfectly authenticated and still perform actions they
have no right to perform, because the application verified identity and then never checked
permission. Passing the login screen proves who you are; it does not decide what you may do.

Both checks must be enforced server-side. Anything enforced only in the browser — a hidden
menu item, a disabled button, a client-side route guard — is a user-interface decision, not a
security control.

---

## Security Perspective

What I took from this beyond the definitions:

1. **Everything the client sends is untrusted input.** Not just form fields — the URL, the
   headers, the cookies, the method. All of it can be modified before it reaches the server.
2. **The browser is a convenience, not a boundary.** The browser presents an interface, but
   requests can be constructed without it. Any control that only exists in the interface does
   not exist.
3. **Statelessness explains session weaknesses.** Because HTTP remembers nothing, identity has
   to be re-asserted every request, and the mechanisms for doing that are where things break.
4. **Access control is decided per request, not per session.** Every single request needs its
   own authorisation decision, because every single request can be replayed or modified.

---

## Key Takeaways

- HTTP is stateless; all session handling is a workaround for that
- Every component of a request is client-controlled and therefore untrusted
- Method conventions are not enforcement
- 401 means unauthenticated, 403 means unauthorised
- Authentication and authorisation are different controls that fail independently
- Security controls must be enforced server-side to exist at all

---
