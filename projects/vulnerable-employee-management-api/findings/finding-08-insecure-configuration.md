# Finding 08 — Insecure Configuration

> Part of a self-built Application Security lab: a deliberately vulnerable
> .NET 8 Web API (Employee Management) that I attack, document, and fix.
> This finding covers the application's deployment and framework configuration.

| | |
|---|---|
| **Severity** | 🟡 Medium |
| **CVSS 3.1** | 5.3 — `AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N` |
| **Category** | OWASP Top 10 — A05:2021 Security Misconfiguration · OWASP API Security Top 10 — API8:2023 Security Misconfiguration · CWE-16, CWE-209, CWE-319, CWE-942 |
| **Affected** | Application-wide (`Program.cs`, `appsettings.json`) |
| **Tools used** | Browser, Postman, source review |
| **Status** | Reported — remediation below |

---

## Summary

The application ships with several insecure default settings that increase its
attack surface and leak internal information: detailed error pages and Swagger are
enabled unconditionally, CORS accepts any origin, HTTPS is not enforced, and the
JWT signing secret is committed to source control.

## Description

This is a collection of configuration weaknesses rather than a single exploit.
Each one is low-effort for an attacker to take advantage of and together they make
every other finding easier to discover and exploit.

### 1. Verbose errors leak internals (CWE-209)
A malformed request returns a full stack trace and the underlying database error.
When I added a single quote during the SQL injection testing (Findings 01 and 02),
the server responded with a SQLite error message and stack trace instead of a
generic error. That tells an attacker the database engine, the query structure,
and framework internals — a direct aid to exploitation.

### 2. Swagger UI exposed unconditionally
`/swagger` serves the complete API documentation — every endpoint, parameter, and
schema — to anyone, in any environment, with no authentication.

### 3. Permissive CORS (CWE-942)
The default CORS policy is `AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()`, so
any website can call the API from a victim's browser.

### 4. No HTTPS enforcement (CWE-319)
There is no HTTPS redirection, so tokens and credentials can travel in clear text
and be intercepted on the network.

### 5. Secret committed to source control
The JWT signing key is hard-coded in `appsettings.json` (see Finding 05), so anyone
with repository access can forge tokens.

## Steps to reproduce

- **Verbose errors:** send `GET /api/employees/search?name=test'` (a single quote)
  and observe the `500` response containing a SQLite error and stack trace
  (captured in Findings 01/02).
- **Swagger:** browse to `http://localhost:5080/swagger` — the full API is
  documented with no login.
- **CORS / HTTPS / secret:** review `Program.cs` and `appsettings.json` (below).

## Proof of concept

The verbose-error evidence is the `500` stack trace from the SQL injection probes
in Findings 01 and 02. The remaining items are confirmed by configuration review:

```csharp
// Program.cs

// 3. CORS — any origin, header, and method
builder.Services.AddCors(o =>
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// 1 & 2. Developer exception page and Swagger enabled unconditionally
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

// 4. No app.UseHttpsRedirection() — HTTP is served as-is
```

```jsonc
// appsettings.json — 5. secret in source control
"Jwt": { "Key": "insecure-demo-signing-key-do-not-use-in-production", ... }
```

*(Optional: add `screenshots/finding-08-swagger.png` showing the open Swagger UI and
`screenshots/finding-08-verbose-error.png` showing a 500 stack trace, if you want
inline screenshots.)*

## Impact

- Error messages hand an attacker the database type, query shapes, and stack
  traces, speeding up every other attack.
- The open API documentation removes any need to guess endpoints.
- Permissive CORS allows malicious sites to make authenticated calls from a
  victim's browser.
- Without HTTPS, credentials and tokens can be sniffed in transit.
- The committed signing key enables token forgery (Finding 05).

## Severity

**Medium.** No single item is a direct compromise, but together they leak
sensitive internal detail and widen the attack surface, and the missing HTTPS and
committed secret raise the ceiling on impact.

CVSS 3.1 base score **5.3** — `AV:N/AC:L/PR:N/UI:N/S:U/C:L/I:N/A:N` (scored on the
information disclosure from verbose errors; the other items add risk).

## Root cause

Development-time conveniences and permissive defaults were left in place for all
environments, and a secret was stored in configuration that ships with the code.

## Remediation

```csharp
// Only expose developer error pages and Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");   // generic ProblemDetails, no stack trace
    app.UseHsts();
}

app.UseHttpsRedirection();

// Restrict CORS to known origins and only the methods/headers actually needed
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins("https://app.example.com")
    .WithMethods("GET", "POST", "PUT", "DELETE")
    .WithHeaders("Authorization", "Content-Type")));
```

- **Never commit secrets.** Load the JWT key from an environment variable or secret
  manager, and rotate the committed one (Finding 05).
- **Don't expose Swagger in production** (or put it behind authentication).
- Set secure response headers (HSTS, `X-Content-Type-Options`, a content security
  policy) as part of hardening.

## Verification (retest)

In a production-like run (`ASPNETCORE_ENVIRONMENT=Production`):
- A malformed request returns a generic error with no stack trace.
- `/swagger` is not served.
- HTTP requests redirect to HTTPS.
- The signing key is read from the environment, not from `appsettings.json`.

(Add the retest screenshots here once the fix is applied.)

## References

- OWASP Top 10 — A05:2021 Security Misconfiguration: https://owasp.org/Top10/A05_2021-Security_Misconfiguration/
- OWASP API Security Top 10 (2023) — API8 Security Misconfiguration: https://owasp.org/API-Security/editions/2023/en/0xa8-security-misconfiguration/
- Microsoft Learn — Error handling in ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling
- CWE-209 — Generation of Error Message Containing Sensitive Information: https://cwe.mitre.org/data/definitions/209.html
