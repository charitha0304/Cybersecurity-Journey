# Username Enumeration via Different Responses
**Platform:** PortSwigger Web Security Academy  
**Category:** Authentication — username enumeration  
**Official lab:** https://portswigger.net/web-security/authentication/password-based/lab-username-enumeration-via-different-responses  
**Status:** Solved

---

## Objective

Determine whether the login responded differently to valid and invalid usernames, allowing valid usernames to be distinguished before a password is known.

## Observation and Methodology

I sent the login request to Burp Intruder and varied the username across a candidate list, comparing the responses. One username produced a response that differed from the others (in message text and/or length), distinguishing it as valid. I then tested candidate passwords for that single username and confirmed access to the account page. I have not published any credentials.

## Root Cause

The authentication flow returned observably different responses depending on whether the submitted username existed. This difference — wording, response length, or status — leaks account existence.

## Security Impact

An attacker can build a list of valid usernames without any password, narrowing a later password-guessing attack and enabling targeted phishing or credential-stuffing. It converts an open guessing problem into a focused one.

## Remediation

Return a single, identical response for all failed logins regardless of whether the username exists — same wording, same status, and comparable timing. Apply rate limiting and account lockout/backoff on the login endpoint, and monitor for enumeration patterns. Treat response length and timing as part of the response: they must not vary with account existence.

## Security Lessons

- Authentication responses must not reveal whether an account exists.
- Side channels count: message text, response length and timing can all leak the same fact.
- Enumeration is a precursor, not the end goal — it makes the next attack cheaper.

## Evidence

- `screenshots/01-solved-with-burp.png` — Account page reached, with Burp traffic
- `screenshots/02-intruder-results.png` — Burp Intruder results — comparing responses across usernames
- `screenshots/03-intruder-results.png` — Burp Intruder results — password stage
- `screenshots/04-solved.png` — Lab solved; My Account page

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
