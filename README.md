# Cybersecurity Journey

A working record of my transition from software development into application security.

I am a software developer with around a year of professional experience building web
applications with Angular and .NET/C#. I am now moving into cybersecurity, focused on
application security. This repository documents the vulnerability classes I study, the labs
I complete, what I observe, and how I reason about root cause and remediation.

Where I am being clear about: I am at the start of this transition. I have not worked
professionally as a penetration tester or security engineer. What this repository
demonstrates is methodology and progression — learn, practise, test, observe, explain,
remediate, document — not seniority. All testing is performed against authorised lab
environments; see [SECURITY.md](SECURITY.md).

---

## Why my background is relevant to AppSec

Application security is largely a code and design discipline, and my development experience
maps onto it directly:

- I can read the code that contains the bug — a C# controller or an Angular service.
- I know where authorisation checks are supposed to live (middleware order, route handlers).
- I have written the database queries, so injection is concrete rather than theoretical.
- I have designed REST APIs, so object-level authorisation, mass assignment and excessive
  data exposure are failures of decisions I have actually made.

| Area | Technologies |
|---|---|
| Frontend | Angular, TypeScript, JavaScript, HTML, CSS |
| Backend | .NET, C#, REST APIs |
| Data | SQL |
| Tooling | Git, GitHub, Burp Suite |

---

## Lab write-ups

Thirteen labs completed and documented, plus one in progress. Each write-up covers the
objective, observation and methodology, root cause, security impact, remediation, lessons,
and redacted evidence. Several include a note on how the issue maps to the .NET/Angular stack.

### Broken access control
| Lab | Status | Write-up |
|---|---|---|
| Unprotected admin functionality | Solved | [read](labs/access-control/01-unprotected-admin-functionality/) |
| User ID controlled by request parameter (IDOR) | Solved | [read](labs/access-control/02-user-id-controlled-by-request-parameter/) |
| URL-based access control can be circumvented | In progress | [read](labs/access-control/03-url-based-access-control-circumvented/) |

### Authentication
| Lab | Status | Write-up |
|---|---|---|
| Username enumeration via different responses | Solved | [read](labs/authentication/username-enumeration-via-different-responses/) |
| Brute-forcing a stay-logged-in cookie | Solved | [read](labs/authentication/brute-forcing-a-stay-logged-in-cookie/) |
| JWT authentication bypass via unverified signature | Solved | [read](labs/authentication/jwt-auth-bypass-via-unverified-signature/) |

### SQL injection
| Lab | Status | Write-up |
|---|---|---|
| Retrieval of hidden data | Solved | [read](labs/sql-injection/retrieval-of-hidden-data/) |
| Login bypass | Solved | [read](labs/sql-injection/login-bypass/) |

### Cross-site scripting (XSS)
| Lab | Status | Write-up |
|---|---|---|
| Reflected XSS into HTML context | Solved | [read](labs/xss/reflected-into-html-context/) |
| Stored XSS into HTML context | Solved | [read](labs/xss/stored-into-html-context/) |
| DOM XSS in document.write sink | Solved | [read](labs/xss/dom-xss-document-write-sink/) |

### Cross-site request forgery (CSRF)
| Lab | Status | Write-up |
|---|---|---|
| CSRF with no defenses | Solved | [read](labs/csrf/no-defenses/) |

### API testing
| Lab | Status | Write-up |
|---|---|---|
| Exploiting an API endpoint using documentation | Solved | [read](labs/api-testing/exploiting-api-endpoint-using-documentation/) |
| Exploiting a mass assignment vulnerability | Solved | [read](labs/api-testing/mass-assignment/) |

---

## Vulnerability assessment reports (OWASP ZAP)

A small vulnerability assessment of a local, self-hosted OWASP Juice Shop instance using OWASP
ZAP 2.17.0 — passive and active scanning, with manual validation of the key finding. These are
tool-driven assessment reports, distinct from the manual labs above.

| Finding | Scan type | Severity | Report |
|---|---|---|---|
| SQL Injection — product search `q` parameter | Active + manual validation | High | [read](vulnerability-assessment-reports/owasp-zap-juice-shop/active-scan-sql-injection/) |
| Content Security Policy (CSP) header not set | Passive | Misconfiguration | [read](vulnerability-assessment-reports/owasp-zap-juice-shop/passive-scan-csp-header/) |

Overview: [vulnerability-assessment-reports/owasp-zap-juice-shop/](vulnerability-assessment-reports/owasp-zap-juice-shop/)

---

## Projects

Self-built projects that go beyond individual labs — building an application, testing its security, and remediating vulnerabilities end to end.

| Project | Stack | Scope | Write-up |
|---------|-------|-------|----------|
| Vulnerable Employee Management API | .NET 8 Web API, EF Core, SQLite, JWT | SQL injection, IDOR/BOLA, authorization flaws, JWT security, excessive data exposure, rate limiting, and insecure configuration | [Read the project](projects/vulnerable-employee-management-api/README.md) |

Built as an intentionally vulnerable application in a local-only lab and tested using Burp Suite, Postman, and jwt.io. Findings include discovery, evidence, impact, remediation, and retesting.


## Fundamentals

Foundational notes written while building up the basics — how the web works underneath, and
getting hands-on with an intercepting proxy.

| Topic | Notes |
|---|---|
| Web fundamentals — HTTP/HTTPS, methods, headers, cookies, status codes, authN vs authZ | [read](fundamentals/web-fundamentals.md) |
| Intercepting HTTP traffic with Burp Suite | [read](fundamentals/burp-suite/intercepting-http-traffic.md) |
| Modifying HTTP requests with Burp Suite | [read](fundamentals/burp-suite/modifying-http-requests.md) |

---

## Platforms and resources

- **PortSwigger Web Security Academy** — primary lab environment for web vulnerability classes
- **Burp Suite (Community)** — intercepting proxy used across the labs
- **OWASP ZAP** — passive and active scanning of a local OWASP Juice Shop instance
- **OWASP Juice Shop** — deliberately vulnerable app, self-hosted locally for assessment practice
- **TryHackMe** — foundational web and networking concepts
- **OWASP** — Top 10, API Security Top 10, ASVS, WSTG, Cheat Sheet Series

See [resources/learning-resources.md](resources/learning-resources.md).

---

## Evidence and redaction

Screenshots are stored in a `screenshots/` folder inside each lab. Secrets captured during
testing — an API key, session and persistent-login cookies — have been redacted in the
committed images. The URL-based access control lab is recorded honestly as **in progress**
until I reproduce and capture a confirmed solved result. See [SECURITY.md](SECURITY.md).

---

## Roadmap

1. Core web vulnerability classes via the Web Security Academy (in progress).
2. .NET and Angular specific issues — insecure deserialisation, over-posting, middleware
   ordering, sanitiser bypasses.
3. API security depth — object-level authorisation, token and JWT handling.
4. Secure code review and automation — SAST, dependency scanning, CI integration.
5. Threat modelling and secure design — STRIDE, OWASP ASVS.

---

## Contact

- GitHub: https://github.com/charitha0304
- LinkedIn: [Add LinkedIn URL]

---

*This repository is a learning record. Every finding documented here is from an authorised
lab environment, and no official lab solutions are reproduced.*
