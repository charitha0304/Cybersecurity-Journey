# Security Policy and Testing Scope

## Purpose of This Repository

This repository documents my personal learning in application security. It contains
write-ups, notes and analysis of security testing I have carried out as part of that learning.

## Authorised Testing Only

All security testing documented in this repository was performed exclusively against:

- **Authorised training platforms** that provide lab environments expressly for security
  testing practice, including the PortSwigger Web Security Academy and TryHackMe
- **Intentionally vulnerable applications** published for the purpose of security education
- **Systems I own or control**, including applications I have built myself for testing

I have not performed, and do not perform, security testing against any system without explicit
authorisation from its owner. Nothing in this repository is the result of unauthorised access
to any third-party system.

## Content of Write-Ups

- Write-ups document my own methodology, observations and reasoning
- Lab solutions from training platforms are not reproduced
- Where evidence is incomplete, that is marked as a placeholder rather than filled in with
  invented data
- No real credentials, keys, tokens or session identifiers are committed to this repository

## Purpose of Published Findings

Findings are documented to demonstrate methodology and understanding, and to record what I
have learned. They are not published to enable exploitation of any live system.

## Reporting an Issue

If you believe anything in this repository is inaccurate, inappropriate, or discloses
something it should not, please open an issue or contact me directly.

`[Add contact email]`

## Redaction of Captured Secrets

Some screenshots captured during lab testing contained values that should not be published.
These have been redacted in the committed images:

- An API key displayed on another user's account page (IDOR lab) — covered with a solid bar.
- A persistent "stay logged in" cookie value (cookie brute-force lab) — covered.
- A session cookie in a request (URL-based access control lab) — covered.

The redacted values belonged to ephemeral PortSwigger lab instances and are not reusable, but
they are removed as a matter of good practice. The URL-based access control lab is kept in an
"In Progress" state and is not presented as solved.
