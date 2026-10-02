# Task 08: Bad Auth Refactor Pack

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  

---

## 🎯 Task Overview

Inherit, diagnose, and refactor poorly designed/vulnerable legacy authentication and authorization code:
1. **Flaw 1: Hardcoded Secrets & Weak Tokens:** Eliminate hardcoded signing keys and expired/unsigned token acceptance.
2. **Flaw 2: Plaintext / MD5 Password Storage:** Migrate vulnerable hashing to secure salted hashing mechanisms.
3. **Flaw 3: Insecure Direct Object References (IDOR):** Fix endpoints where user IDs in routes are trusted without verifying the authenticated token identity.
4. **Flaw 4: Missing Role Enforcement:** Secure endpoints that lack role checks or allow privilege escalation.
5. **Automated Unit & Integration Tests:** Prove all vulnerabilities are eliminated without breaking legitimate consumer workflows.
