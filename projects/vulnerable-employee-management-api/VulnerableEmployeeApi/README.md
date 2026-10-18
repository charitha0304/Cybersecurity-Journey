# Vulnerable Employee Management API — AppSec Lab

A small .NET 8 Web API for an employee-management system, built **deliberately
insecure** so it can be attacked, fixed, and documented as an Application
Security portfolio project.

> ⚠️ **Local lab only. Never deploy this or expose it to a network you don't
> control.** It stores plain-text passwords, ships a committed secret, and is
> intentionally exploitable. Run it on `localhost`, break it, fix it, then keep
> the write-up — not the running app.

---

## What's inside

- .NET 8 Web API, EF Core + SQLite (database file is created and seeded on first run)
- JWT bearer authentication
- Seven planted vulnerability classes, each marked in code with a `// VULN` comment
- `requests.http` with a ready-made request for every attack

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` should print 8.x; newer SDKs also work)
- Optional attacker tooling: Burp Suite Community, Postman, a browser, `sqlmap`, and `jwt_tool` (or [jwt.io](https://jwt.io))

## Run it

```bash
cd VulnerableEmployeeApi
dotnet restore
dotnet run
```

It starts on **http://localhost:5080**. Swagger UI is at
**http://localhost:5080/swagger**. The SQLite file `employees.db` is created and
seeded automatically; delete it to reset the data.

## Seeded accounts

| Username | Password    | Role     | Owns employee |
|----------|-------------|----------|---------------|
| admin    | `Admin@123` | Admin    | 5 (Priya)     |
| alice    | `Password1` | Employee | 1             |
| bob      | `Hunter2`   | Employee | 2             |
| carol    | `Finance99` | Employee | 3             |

Use a low-privilege account (alice) for most attacks — the whole point is
showing what a normal user can reach that they shouldn't.

---

## Vulnerability map

| # | Vulnerability | Where (endpoint) | Code location |
|---|---------------|------------------|---------------|
| 1 | SQL injection (auth bypass) | `POST /api/auth/login` | `Controllers/AuthController.cs` |
| 2 | SQL injection (UNION extraction) | `GET /api/employees/search` | `Controllers/EmployeesController.cs` |
| 3 | IDOR / broken object-level authz | `GET /api/employees/{id}` | `Controllers/EmployeesController.cs` |
| 4 | Broken authorization (edit / delete / admin report) | `PUT`/`DELETE /api/employees/{id}`, `GET /api/admin/users` | `EmployeesController.cs`, `AdminController.cs` |
| 5 | Weak JWT validation | JWT setup | `Program.cs`, `Services/TokenService.cs` |
| 6 | Excessive data exposure | `GET /api/employees`, `/api/admin/users`, login response | controllers (no DTOs) |
| 7 | Missing rate limiting | `POST /api/auth/login` | `Program.cs` (no limiter registered) |
| 8 | Insecure configuration | CORS, verbose errors, Swagger, no HTTPS, committed secret | `Program.cs`, `appsettings.json` |

Full attack requests for each are in **`requests.http`**.

---

## Attacker setup (quick pointers)

- **Browser + Burp** — set the browser proxy to `127.0.0.1:8080`, trust the Burp
  CA, and send traffic through Proxy → HTTP history. Tamper requests in Repeater.
- **Postman** — import the endpoints or hand-build them; set an `Authorization:
  Bearer <token>` header after logging in.
- **SQL injection** — reproduce by hand with the payloads in `requests.http`, or
  point `sqlmap` at the search endpoint:
  `sqlmap -u "http://localhost:5080/api/employees/search?name=a" --headers="Authorization: Bearer <token>" --batch`
- **JWT manipulation** — paste a token into [jwt.io](https://jwt.io) to read the
  claims. The signing secret is committed in `appsettings.json`
  (`insecure-demo-signing-key-do-not-use-in-production`), so the primary attack
  is *source disclosure*: read the key from the repo, then on jwt.io paste it
  into the "verify signature" box, change the payload to `"role": "Admin"`, and
  copy the re-signed token. No cracking required — that is the realistic
  weak-JWT finding on a modern library.
  - *Optional crack demo:* a low-entropy secret also falls to a dictionary
    attack. A tiny wordlist containing this key is shipped as `jwt-wordlist.txt`;
    run `jwt_tool <token> -C -d jwt-wordlist.txt` (or `hashcat -m 16500`) to show
    the methodology succeeding.
  - *Second weak-validation flaw (independent of the key):* the API sets
    `ValidateIssuer = false` and `ValidateAudience = false`, so a token minted for
    any issuer/audience is accepted.

---

## How to document each finding

Use this structure for every vulnerability (this is your report format):

```
Vulnerability   — what it is, one line
↓
How I discovered it — tool + what tipped you off
↓
HTTP request    — the exact request that triggers it
↓
Evidence        — the response / screenshot proving it worked
↓
Impact          — what an attacker gains
↓
Severity        — Critical / High / Medium / Low (say why)
↓
Root cause      — the specific coding/config mistake
↓
Fix             — the change that closes it (with the corrected code)
↓
Retest          — the same request, now blocked
```

### Worked example — SQL injection (login bypass)

**Vulnerability** — `POST /api/auth/login` builds SQL by string concatenation, so
input is interpreted as SQL.

**How I discovered it** — Noticed the login response changed when a single quote
`'` was added to the username (server behaviour shifted), then tested a boolean
payload.

**HTTP request**
```http
POST /api/auth/login HTTP/1.1
Host: localhost:5080
Content-Type: application/json

{ "username": "' OR '1'='1' --", "password": "x" }
```

**Evidence** — Returns `200 OK` with a valid JWT for `admin` despite no correct
password. (Attach the Burp/Postman response screenshot.)

**Impact** — Full authentication bypass; attacker logs in as the first user
(admin) and, via UNION injection on the search endpoint, dumps every user's
plain-text password.

**Severity** — Critical. Unauthenticated, trivial, leads to full account takeover.

**Root cause** — Untrusted input concatenated into a raw SQL string
(`FromSqlRaw($"... '{req.Username}' ...")`), plus passwords stored and compared in
plain text.

**Fix** — Look the user up with a parameterised query and verify a hashed
password, never interpolate input into SQL:
```csharp
var user = _db.Users.FirstOrDefault(u => u.Username == req.Username);
if (user == null || !PasswordHasher.Verify(req.Password, user.PasswordHash))
    return Unauthorized(new { message = "Invalid credentials" });
```

**Retest** — The same `' OR '1'='1' --` payload now returns `401 Unauthorized`;
the quote is treated as literal data. (Attach the retest screenshot.)

---

## Project phases

1. **Build** — this repo. ✅
2. **Attack** — work through `requests.http`, capturing evidence for each finding
   (fill the template up to *Root cause*).
3. **Fix + retest** — remediate each vulnerability, re-run the same attack, and
   capture the "now blocked" evidence (completes *Fix* and *Retest*).
4. **(Optional) Angular front-end** — a thin UI over the API once the security
   work is done.

Keep the vulnerable build and the fixed build as two commits (or two branches) so
the diff *is* your remediation evidence.
