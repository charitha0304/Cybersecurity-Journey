# SQL Injection — Retrieval of Hidden Data
**Platform:** PortSwigger Web Security Academy  
**Category:** Injection — SQL injection (in a WHERE clause)  
**Official lab:** https://portswigger.net/web-security/sql-injection/lab-retrieve-hidden-data  
**Status:** Solved

---

## Objective

Determine whether a product-filter parameter was concatenated into a SQL WHERE clause, allowing the query's logic to be altered so that records normally hidden by the filter were returned.

## Observation and Methodology

The category filter parameter was placed into a SQL query without being treated strictly as data. By injecting input that altered the WHERE condition so it always evaluated true (and commenting out the remainder of the clause), the application returned products that the intended filter would have excluded. I confirmed that additional, normally-hidden records were displayed.

## Root Cause

User input from the category parameter was incorporated into the SQL statement as part of the query rather than as a bound parameter. This let the input change the query's logic instead of only its data.

## Security Impact

An attacker can change the meaning of the query. Retrieving hidden records is the mildest outcome; the same flaw class commonly leads to reading arbitrary data from other tables and, depending on the database and context, further compromise.

## Remediation

Use parameterised queries / prepared statements for every database call so user input is always bound as data. Prefer an ORM or query builder that parameterises by default, validate input against an allow-list where the set of valid values is known (e.g. categories), and apply least-privilege database accounts.

## Relevance to My Development Background

In .NET this maps directly to the difference between string-concatenated SQL and parameterised queries. Using parameterised commands (or an ORM such as Entity Framework Core with LINQ rather than raw interpolated SQL) keeps user input as data, never as part of the query structure.

## Security Lessons

- Injection is a data-vs-code confusion: the fix is to keep input as data, not to filter 'bad characters'.
- Parameterisation closes the whole class, not one payload.
- Allow-listing known-valid values is strong defence in depth for constrained inputs.

## Evidence

- `screenshots/01-solved.png` — Lab solved; filtered view returning normally-hidden products
- `screenshots/02-burp.png` — Burp request/response evidence
- `screenshots/03-burp.png` — Burp request/response evidence

## Ethical Scope

All testing was performed exclusively against the authorised PortSwigger Web Security Academy lab environment, which is provided expressly for security testing practice. No testing was performed against any system I do not own or have explicit authorisation to test, and no official lab solutions are reproduced here.
