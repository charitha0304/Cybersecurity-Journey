# Day 02 — Intercepting HTTP Traffic with Burp Suite

**Source:** Official PortSwigger Burp Suite documentation

---

## Objective

Move from reading about HTTP requests to observing them directly. I wanted to see the actual
traffic my browser generates, rather than the rendered result of it, and to be able to pause a
request before it reaches the server.

---

## Tool

**Burp Suite** — [Add edition used: Community / Professional]

Burp Suite operates as an intercepting proxy. It sits between the browser and the target
application: the browser is configured to send its traffic to Burp, Burp forwards it to the
target, and the response returns along the same path. Because Burp holds each request in the
middle of that path, an authorised tester can inspect, pause, modify, or reissue it before it
continues.

For HTTPS traffic this requires Burp's CA certificate to be trusted by the browser. Without
it, the browser correctly rejects Burp's connection, because Burp is functioning as a
deliberate man-in-the-middle. Installing the certificate is an explicit decision to trust that
interception on my own machine, for my own traffic.

---

## What I Practiced

1. Downloading and installing Burp Suite
2. Configuring the browser and proxy so traffic flows through Burp
3. Intercepting HTTP traffic
4. Modifying intercepted requests
5. Setting the target scope
6. Reissuing requests

---

## Intercepting HTTP Traffic

With interception enabled, requests are held in the Proxy tab rather than being sent straight
on. Each held request can be forwarded, dropped, or edited before forwarding.

Setting the **target scope** turned out to be more important than I expected. Without it, Burp
captures every request the browser makes, including background traffic from unrelated sites,
which quickly buries the traffic that actually matters. Restricting scope to the target
application makes the request history readable and also keeps testing confined to the systems
I am authorised to test.

Alongside live interception, the **HTTP history** records traffic that has already passed
through the proxy, which is useful for reviewing a flow after completing it rather than
pausing at every step.

`[Add screenshot of intercepted request]`

---

## Request Analysis

Working with real intercepted requests, the parts I focused on:

- The request line — method, path, and any parameters carried in the path or query string
- The `Host` header
- The `Cookie` header, and which values appear to carry session state
- Any `Authorization` header
- The request body on state-changing requests
- The corresponding response status code and headers

`[Add annotated example of a request captured during this session]`

---

## Security Relevance

Seeing the raw traffic changed how concrete the fundamentals felt.

The browser interface presents a curated version of what an application allows. The proxy
shows what the application actually accepts. A control that hides a button, disables a field,
or blocks a route in the client is entirely absent at this layer — the request can still be
constructed and sent.

This is the practical foundation for testing access control: if a control is enforced only in
the interface, it is not enforced at all, and a proxy is how that gets demonstrated rather
than asserted.

---

## Key Takeaways

- Burp Suite acts as an intermediary between browser and application, allowing an authorised
  tester to inspect and modify traffic in transit
- Intercepting HTTPS requires deliberately trusting Burp's CA certificate, because Burp is
  acting as a man-in-the-middle by design
- Defining the target scope early keeps the history usable and keeps testing within authorised
  boundaries
- The proxy view shows what the server will accept, not what the interface offers
- Client-side restrictions are not visible at the protocol layer, which is exactly why they
  cannot be relied on as security controls

---

## Ethical Scope

All interception was performed against authorised lab environments and my own browser traffic.
