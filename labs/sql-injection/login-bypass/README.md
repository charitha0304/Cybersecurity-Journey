# SQL Injection — Login Bypass
**Platform:** PortSwigger Web Security Academy  
**Category:** Injection — SQL injection in authentication  
**Official lab:** https://portswigger.net/web-security/sql-injection/lab-login-bypass  
**Status:** Solved

---

## Objective

Determine whether the login query concatenated the username, allowing the authentication condition to be subverted so that login succeeded without a valid password.

## Observation and Methodology

The login form built its query from user input without parameterisation. By injecting input that terminated the intended condition and commented out the password check, the application authenticated the request as the administrator account without a valid password. I confirmed access to the account as administrator.

## Root Cause

The username (and the surrounding authentication logic) was constructed from user input inside the SQL statement. The injected input changed the WHERE clause so the credential check no longer applied.

## Security Impact

Complete authentication bypass: an attacker logs in as any known account, including an administrator, without credentials. This is among the highest-severity outcomes for a web application.

## Remediation

Use parameterised queries for the authentication lookup so the username is bound as data. Do not build authentication logic by string concatenation. Combine with proper password verification (hashed and salted), least-privilege database accounts, and monitoring for injection attempts.

## Relevance to My Development Background

In .NET this maps directly to the difference between string-concatenated SQL and parameterised queries. Using parameterised commands (or an ORM such as Entity Framework Core with LINQ rather than raw interpolated SQL) keeps user input as data, never as part of the query structure.

## Security Lessons

- Injection in an authentication path escalates straight to account takeover.
- The control is parameterisation, not input blacklisting.
- Authentication logic should never be assembled from raw user input.

## Evidence

- `screenshots/01-solved-with-burp.png` — Lab solved; authenticated as administrator, with Burp traffic
- `screenshots/02-burp-repeater.png` — Burp Repeater request/response

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
