# Employee Management API — Application Security Lab

A small, deliberately vulnerable .NET 8 Web API that I built, attacked, documented,
and fixed as an Application Security portfolio project. The goal is to show the full
loop an AppSec engineer works through: find a flaw, prove it with evidence, explain
the root cause, and remediate it.

> ⚠️ **Intentionally vulnerable — local lab only.** This project stores passwords in
> clear text, ships a committed secret, and is exploitable on purpose. Do not deploy
> it or expose it to any network you don't control. Run it on `localhost`, break it,
> fix it, and keep the write-ups.

---

## What this demonstrates

For each vulnerability I worked through one cycle:

```
Vulnerability → How I discovered it → HTTP request → Evidence
            → Impact → Severity → Root cause → Fix → Retest
```

The findings below are documented through **root cause**, with the recommended fix
written up. Applying the fixes and capturing the "now blocked" retests is Phase 3.

## Stack

- .NET 8 Web API
- Entity Framework Core + SQLite (database is created and seeded on first run)
- JWT bearer authentication

## Running it

```bash
cd VulnerableEmployeeApi
dotnet restore
dotnet run
```

The API starts on `http://localhost:5080`, with Swagger at `/swagger`. The SQLite
file `employees.db` is created and seeded automatically — delete it to reset.

**Seed accounts:** usernames and roles are below; the default passwords are defined
in `VulnerableEmployeeApi/Data/DbSeeder.cs`.

| Username | Role     | Owns employee |
|----------|----------|---------------|
| admin    | Admin    | 5             |
| alice    | Employee | 1             |
| bob      | Employee | 2             |
| carol    | Employee | 3             |

## Findings

Eight vulnerability classes, each with its own report in `findings/`.

| # | Vulnerability | Severity | OWASP / CWE | Report |
|---|---------------|----------|-------------|--------|
| 1 | SQL injection — authentication bypass | 🔴 Critical | A03:2021 Injection · CWE-89 | [finding 01](findings/finding-01-sql-injection-auth-bypass.md) |
| 2 | SQL injection (UNION) — credential disclosure | 🟠 High | A03:2021 Injection · CWE-89 | [finding 02](findings/finding-02-sql-injection-union-data-exposure.md) |
| 3 | IDOR / Broken Object Level Authorization | 🟡 Medium | API1:2023 BOLA · CWE-639 | [finding 03](findings/finding-03-idor-broken-object-level-authorization.md) |
| 4 | Broken Function Level Authorization | 🟠 High | API5:2023 BFLA · CWE-862 | [finding 04](findings/finding-04-broken-function-level-authorization.md) |
| 5 | Weak JWT — forgeable tokens | 🔴 Critical | API2:2023 Broken Auth · CWE-321 | [finding 05](findings/finding-05-weak-jwt-forgeable-tokens.md) |
| 6 | Excessive data exposure | 🟡 Medium | API3:2023 · CWE-213 | [finding 06](findings/finding-06-excessive-data-exposure.md) |
| 7 | Missing rate limiting | 🟡 Medium | API4:2023 · CWE-307 | [finding 07](findings/finding-07-missing-rate-limiting.md) |
| 8 | Insecure configuration | 🟡 Medium | A05:2021 Misconfiguration · CWE-16 | [finding 08](findings/finding-08-insecure-configuration.md) |

**Severity spread:** 2 Critical, 2 High, 4 Medium. Severities are scored on evidence
(CVSS 3.1 vectors are given in each report), not stamped uniformly — for example the
unauthenticated login bypass is Critical, while the authenticated IDOR is Medium.

A thread worth noting across the set: the weak-JWT flaw (Finding 05) undermines the
fixes for Findings 03 and 04, because those rely on the `employeeId` and `role`
claims that a forged token can set freely.

## How the attacks were run

- **Burp Suite (Community)** — intercept proxy, Repeater for tampering requests,
  Intruder for the rate-limiting test.
- **Postman** — building and replaying API requests.
- **Browser + Swagger** — driving the API and reading responses.
- **jwt.io** — decoding and forging tokens.
- **Manual SQL payloads** — injection discovery and UNION extraction.

## Project layout

```
.
├── README.md                     ← this file
├── VulnerableEmployeeApi/        ← the intentionally vulnerable .NET 8 API
└── findings/
    ├── finding-01-....md ... finding-08-....md
    └── screenshots/                   ← evidence screenshots (sensitive values redacted)
```

Screenshots in the reports have sensitive values redacted — bearer tokens, passwords,
and National Insurance numbers are blacked out while enough is left visible to prove
each finding.

## Status

- **Phase 1 — build the vulnerable app:** done.
- **Phase 2 — attack and document (through root cause):** done — all 8 findings.
- **Phase 3 — apply fixes and capture retests:** next.
