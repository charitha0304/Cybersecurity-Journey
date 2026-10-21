# Phase 3 — Retest Checklist

Re-run each original attack against the **hardened** build (`SecureEmployeeApi`)
and confirm it is now blocked. This completes the `Fix → Retest` step of every
finding.

## Setup

1. Stop the vulnerable app if it's running.
2. Run the hardened build:
   ```bash
   cd SecureEmployeeApi
   dotnet run
   ```
   It serves on `http://localhost:5080` and seeds its own `secure.db` with the
   same accounts (`admin/Admin@123`, `alice/Password1`, `bob/Hunter2`,
   `carol/Finance99`) — now stored as salted hashes.
3. Use the same tools as before (Postman, Burp, jwt.io). Capture a screenshot of
   each result and drop it into the matching finding's **Verification (retest)**
   section (suggested names below).

## Summary

| # | Finding | Retest request | Before | After (expected) |
|---|---------|----------------|--------|------------------|
| 1 | SQLi login bypass | `POST /api/auth/login` with `' OR '1'='1' --` | 200 + admin token | **401 Unauthorized** |
| 2 | SQLi UNION | `GET /api/employees/search?name=zzz' UNION SELECT …` | user table dumped | **200 with `[]`** (no data leaked) |
| 3 | IDOR | alice → `GET /api/employees/5` | 200 (another record) | **403 Forbidden** |
| 4 | Broken authz | alice → `GET /api/admin/users`, `DELETE /api/employees/4` | 200 / 204 | **403 Forbidden** |
| 5 | Weak JWT | send the old forged admin token | 200 (accepted) | **401 Unauthorized** |
| 6 | Excessive exposure | `POST /login`, `GET /employees`, `GET /admin/users` | passwords / salary / NI returned | **no sensitive fields** |
| 7 | Rate limiting | Intruder 10+ logins in a minute | all processed | **429 Too Many Requests** |
| 8 | Insecure config | error in Production, `/swagger`, HTTP | stack trace / open docs / HTTP | **generic error / 404 / HTTPS redirect** |

---

## Per-finding detail

### 1 — SQL injection (login bypass)
```json
POST /api/auth/login
{ "username": "' OR '1'='1' --", "password": "x" }
```
**Expected: `401 Unauthorized`** — the quote is treated as literal text, the
parameterised lookup finds no user, and the hash check fails. Also confirm a real
login still works: `{ "username": "alice", "password": "Password1" }` → `200`.
*Screenshot:* `finding-01-retest.png`

### 2 — SQL injection (UNION)
Login as alice, then (with the bearer token):
```
GET /api/employees/search?name=zzz' UNION SELECT Id, Username, PasswordHash, Role, 0, '', '' FROM Users --
```
**Expected: `200 OK` with `[]`.** The whole value is a literal search term, so it
matches no employee and returns nothing from `Users`.
*Screenshot:* `finding-02-retest.png`

### 3 — IDOR
Login as **alice** (owns employee 1):
```
GET /api/employees/5     → 403 Forbidden
GET /api/employees/1     → 200 OK (her own record)
```
*Screenshot:* `finding-03-retest.png`

### 4 — Broken function-level authorization
As **alice** (non-admin):
```
GET    /api/admin/users   → 403 Forbidden
DELETE /api/employees/4   → 403 Forbidden
PUT    /api/employees/2   → 403 Forbidden
```
Then confirm an **admin** token still gets `200` / `204` on the same calls.
*Screenshot:* `finding-04-retest.png`

### 5 — Weak JWT
Send the **old forged admin token** from Finding 05 (the one re-signed with
`insecure-demo-signing-key-do-not-use-in-production`) to any protected endpoint:
```
GET /api/employees/1
Authorization: Bearer <old forged token>
```
**Expected: `401 Unauthorized`** — it was signed with the old key, which the
hardened build no longer trusts (new strong key + issuer/audience validation).
Also confirm a fresh token from the hardened build's login works normally.
*Screenshot:* `finding-05-retest.png`

### 6 — Excessive data exposure
- `POST /api/auth/login` (alice) → response contains a token and `{ username, role }` only — **no password**.
- `GET /api/employees` → each item has `id, fullName, email, department` — **no salary / NI / notes**.
- `GET /api/admin/users` (as admin) → `id, username, role` — **no password/hash**.
*Screenshot:* `finding-06-retest.png`

### 7 — Missing rate limiting
Repeat the Burp Intruder login run (10+ attempts in under a minute — null payloads
or a wordlist).
**Expected:** the first 5 return `401`, then further attempts return
**`429 Too Many Requests`**.
*Screenshot:* `finding-07-retest.png`

### 8 — Insecure configuration
- **Verbose errors:** run in Production and trigger an error —
  ```bash
  # bash/macOS
  ASPNETCORE_ENVIRONMENT=Production dotnet run
  # PowerShell
  $env:ASPNETCORE_ENVIRONMENT="Production"; dotnet run
  ```
  A server error now returns a generic Problem response, **no stack trace**.
- **Swagger:** in Production, `GET /swagger` → **404** (not served).
- **HTTPS:** run the `https` profile (`dotnet run --launch-profile https`);
  `http://localhost:5080` now **redirects (307) to `https://localhost:7080`**.
  You may need `dotnet dev-certs https --trust` once.
- **CORS:** the policy only allows `http://localhost:4200`; a request from any
  other origin is not granted CORS access.
*Screenshot:* `finding-08-retest.png`

---

When the retests are captured, each finding's report tells the full story:
discovery → evidence → impact → severity → root cause → fix → **retest (blocked)**.
