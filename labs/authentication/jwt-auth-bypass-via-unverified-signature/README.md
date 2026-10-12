# JWT Authentication Bypass via Unverified Signature
**Platform:** PortSwigger Web Security Academy  
**Category:** Authentication — JSON Web Token (JWT) handling  
**Official lab:** https://portswigger.net/web-security/jwt/lab-jwt-authentication-bypass-via-unverified-signature  
**Status:** Solved

---

## Objective

Determine whether the application verified the JWT signature, or whether a modified token payload would be accepted.

## Observation and Methodology

I captured the JWT from an authenticated request and inspected its payload. I modified the claim that identifies the user and submitted the altered token to a privileged path. The application accepted the modified token, which showed that the signature was not being verified. This is the unverified-signature case, not the `alg=none` case.

## Root Cause

The application read and trusted the JWT payload without cryptographically verifying its signature. A JWT's integrity depends entirely on signature verification; without it, the payload is simply attacker-controlled input.

## Security Impact

Any user can change their identity or role claim and be treated as another user — here, as an administrator — achieving full authentication/authorisation bypass.

## Remediation

Always verify the JWT signature server-side with the expected algorithm and key before trusting any claim. Pin the accepted algorithm (reject `none` and unexpected algorithms), validate standard claims (issuer, audience, expiry), and use a vetted JWT library with verification enabled rather than decoding the token manually.

## Relevance to My Development Background

In .NET this is the difference between calling a validation routine that actually checks the signing key (`TokenValidationParameters` with `ValidateIssuerSigningKey = true` and a pinned algorithm) and merely reading claims from a decoded token. Manual base64 decoding of the payload for authorisation decisions is the anti-pattern this lab demonstrates.

## Security Lessons

- Decoding a JWT is not validating it; the signature check is the security control.
- Claims inside a token are untrusted until the signature is verified.
- Algorithm confusion and 'no verification' are distinct failure modes — both must be closed.

## Evidence

- `screenshots/01-solved-with-burp.png` — Lab solved; privileged access via a modified token, with Burp traffic

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
