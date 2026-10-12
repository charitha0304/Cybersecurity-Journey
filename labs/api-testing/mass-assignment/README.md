# Exploiting a Mass Assignment Vulnerability
**Platform:** PortSwigger Web Security Academy  
**Category:** API testing — mass assignment / over-posting  
**Official lab:** https://portswigger.net/web-security/api-testing/lab-exploiting-mass-assignment-vulnerability  
**Status:** Solved

---

## Objective

Determine whether the API bound request fields directly to internal model properties, allowing a hidden property to be set by adding it to the request.

## Observation and Methodology

By comparing the API's GET response schema with the POST request body, I identified a property that the client was not meant to control. Adding that property to the request and setting its value caused the server to accept it, altering the outcome in the attacker's favour (a discount applied to a purchase). I verified the request in Burp before documenting it.

## Root Cause

The API bound incoming request fields directly onto an internal object without restricting which properties a client may set. A sensitive, server-controlled property was therefore settable from the request.

## Security Impact

Mass assignment lets a client set fields they should not control — prices, discounts, roles, account flags — leading to business-logic abuse or privilege escalation depending on the exposed property.

## Remediation

Bind requests to an explicit allow-list of client-settable fields using a dedicated input DTO, rather than binding directly to the domain/entity model. Never expose server-controlled properties to model binding. Validate and authorise sensitive fields server-side regardless of what the request contains.

## Relevance to My Development Background

This is directly my stack. In ASP.NET, binding a request straight onto an EF Core entity is the classic over-posting bug; the fix is a view-model/DTO with only the fields the client may set, or `[Bind]`/`BindNever` restrictions. Having designed API models, this is the failure mode I am best placed to catch in review.

## Security Lessons

- Mass assignment is an input-binding design flaw, not a single payload.
- Bind to explicit DTOs; never bind requests straight onto entities.
- Comparing GET schema to POST body is a reliable way to find controllable hidden fields.

## Evidence

- `screenshots/01-solved-with-burp.png` — Lab solved; hidden property accepted by the API, with Burp traffic

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
