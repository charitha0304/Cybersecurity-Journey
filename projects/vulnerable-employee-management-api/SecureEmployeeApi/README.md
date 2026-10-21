# SecureEmployeeApi — Hardened Build

This is the remediated version of `VulnerableEmployeeApi`. It fixes all eight
findings documented in `../findings/`. The two projects sit side by side so the
difference between them *is* the remediation.

## Run

```bash
cd SecureEmployeeApi
dotnet run
```

Starts on `http://localhost:5080` (same as the vulnerable build — run one at a
time), Swagger at `/swagger`. It creates and seeds its own `secure.db` with the
same login accounts, now stored as salted hashes:

| Username | Password    | Role     |
|----------|-------------|----------|
| admin    | `Admin@123` | Admin    |
| alice    | `Password1` | Employee |
| bob      | `Hunter2`   | Employee |
| carol    | `Finance99` | Employee |

Run `../RETEST.md` against this build to confirm each attack is now blocked.

## What changed

| Finding | Fix |
|---------|-----|
| 1 — SQLi login bypass | Parameterised LINQ lookup + salted-hash password verify (`PasswordHasher`) |
| 2 — SQLi UNION | Parameterised `EF.Functions.Like` search |
| 3 — IDOR | Ownership/role check on `GET /api/employees/{id}` |
| 4 — Broken function-level authz | `[Authorize(Roles="Admin")]` on admin report, `PUT`, `DELETE` |
| 5 — Weak JWT | Strong key from config (env/secret), issuer + audience validation |
| 6 — Excessive data exposure | DTOs everywhere; no passwords or sensitive fields leaked |
| 7 — Missing rate limiting | `AddRateLimiter` fixed window on login (5/min → 429) |
| 8 — Insecure config | Dev-only Swagger/errors, HTTPS redirection + HSTS, restricted CORS, key out of source |

## Note on the signing key

For this local lab the JWT key is in `appsettings.json` so the app runs out of the
box. In a real deployment, load it from an environment variable (`Jwt__Key`) or a
secret manager and keep it out of source control — the code already reads it from
configuration, so no code change is needed.
