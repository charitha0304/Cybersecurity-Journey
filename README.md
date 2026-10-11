# Cybersecurity Journey

A working record of my transition from software development into application security.

I am a software developer with around a year of professional experience building web
applications with Angular and .NET. I am now moving into cybersecurity, focusing on
application security. This repository documents what I learn, what I test, what I observe,
and how I reason about it.

I want to be clear about where I am: I am at the beginning of this transition. I have not
worked professionally as a penetration tester or a security engineer. What this repository
demonstrates is methodology and progression, not seniority. All testing documented here is
performed against authorised lab environments — see [SECURITY.md](SECURITY.md).

---

## Target Direction

**Primary:** Junior Application Security / AppSec
**Secondary:** API Security, Vulnerability Management, Cloud Security

---

## Background

I currently work as a developer. My day-to-day stack:

| Area | Technologies |
|---|---|
| Frontend | Angular, TypeScript, JavaScript, HTML, CSS |
| Backend | .NET, C#, REST APIs |
| Data | SQL |
| Tooling | Git, GitHub |

### Why this background matters for AppSec

Application security is largely a code and design discipline, and my development experience
maps onto it directly:

- **I can read the code that contains the bug.** Most vulnerabilities in web applications are
  reachable only by understanding what the application was intended to do. Being able to open
  a C# controller or an Angular service and follow the logic is the core skill, not an extra.
- **I know where authorisation checks are supposed to live.** Having built API endpoints, I
  understand middleware ordering, route handlers, and where a privilege check gets missed.
- **I have written the queries.** Parameterised queries, ORM behaviour, and raw SQL are
  familiar to me, which makes injection classes concrete rather than theoretical.
- **I have designed APIs.** Object-level authorisation, over-posting, and excessive data
  exposure are failures of API design, and I have made the design decisions they come from.
- **I understand the SDLC.** Security findings only matter if a development team can act on
  them. I have been on the receiving end of that conversation.

---

## Skills I Am Currently Developing

- HTTP/HTTPS protocol fundamentals — requests, responses, methods, headers, cookies, status codes
- Authentication vs authorisation, and where the distinction breaks in practice
- Web proxy interception and request manipulation with Burp Suite
- Broken access control identification and reasoning
- Structured vulnerability documentation: methodology, root cause, impact, remediation
- OWASP Top 10 concepts

---

## Hands-On Platforms

| Platform | Use |
|---|---|
| PortSwigger Web Security Academy | Primary lab environment for web vulnerability classes |
| TryHackMe | Foundational web and networking concepts |
| Burp Suite Community Edition | Intercepting proxy for hands-on request analysis |

---

## Write-Ups

Each write-up follows the same structure: what the application was doing, what I observed,
how I tested it, why the flaw exists, the impact, the root cause, and how I would fix it.

| Day | Topic | Write-up |
|---|---|---|
| 01 | Web fundamentals | [web-fundamentals.md](day-01/web-fundamentals.md) |
| 02 | Intercepting HTTP traffic | [intercepting-http-traffic.md](day-02/burp-suite/intercepting-http-traffic.md) |
| 02 | Modifying HTTP requests | [modifying-http-requests.md](day-02/burp-suite/modifying-http-requests.md) |
| 02 | Unprotected administrative functionality | [unprotected-admin-functionality.md](day-02/burp-suite/unprotected-admin-functionality.md) |

Longer write-ups are organised by vulnerability class under [`writeups/`](writeups/):
web security, API security, authentication, authorisation, injection, and cloud security.

---

## Projects

Planned hands-on builds. These are not yet complete and are listed here so the intended
direction is visible.

| Project | Description | Status |
|---|---|---|
| [`secure-api/`](projects/secure-api/) | A .NET REST API built with authorisation, input validation and security headers applied deliberately, documented decision by decision | Not started |
| [`web-security-lab/`](projects/web-security-lab/) | A deliberately vulnerable Angular + .NET application, where I introduce vulnerabilities, exploit them, then remediate them and document each cycle | Not started |
| [`azure-security-lab/`](projects/azure-security-lab/) | Cloud security configuration exercises | Not started |

---

## Learning Roadmap

**Phase 1 — Web and protocol fundamentals**
HTTP/HTTPS, DNS, TLS, the request lifecycle, same-origin policy, proxy interception.

**Phase 2 — Core web vulnerability classes**
Access control, authentication flaws, SSRF, injection, XSS, business logic — worked through
the PortSwigger Web Security Academy labs with a write-up for each class.

**Phase 3 — Language-specific application security**
.NET and Angular specific issues: insecure deserialisation, model binding and over-posting,
middleware ordering, sanitiser bypasses in Angular templates.

**Phase 4 — API security**
Object-level authorisation, token handling, OAuth and JWT failure modes.

**Phase 5 — Secure code review and automation**
Static analysis, dependency scanning, and integrating security checks into CI/CD.

**Phase 6 — Threat modelling and secure design**
STRIDE, OWASP ASVS, and documenting a threat model for my own applications.

---

## Professional Goals

- Move into a junior application security or security engineering role
- Build a demonstrable record of finding, explaining and remediating vulnerabilities rather
  than only completing labs
- Develop depth in .NET and API security specifically, rather than breadth across every
  security domain at once
- Contribute security fixes to open-source projects

---

## Contact

[Add LinkedIn URL]
[Add email if you want to make it public]

---

*This repository is a learning record. Nothing here should be taken as professional security
advice, and every finding documented is from an authorised lab environment.*
