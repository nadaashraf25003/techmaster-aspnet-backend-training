# Task 01: Authentication Foundation • Identity • Hashing • JWT

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  

---

## 🎯 Task Overview

Implement the core authentication foundation for the TechMaster Secure Training Platform API:
1. **User Identity Model:** `User` entity with `Email`, `PasswordHash`, `Salt`, `Role` (Admin, Instructor, Student), `IsActive`, and timestamps.
2. **Cryptographic Password Hashing:** Secure password hashing service implementing PBKDF2/ASP.NET Identity hasher with unique cryptographic salt per user.
3. **JWT Token Generation:** `IJwtTokenService` generating cryptographically signed JSON Web Tokens containing user claims (`sub`, `email`, `role`, `name`).
4. **Authentication Endpoints:**
   - `POST /api/auth/register` — Register a new student/user.
   - `POST /api/auth/login` — Validate credentials and return JWT bearer token.
   - `GET /api/auth/me` — Protected endpoint returning current authenticated user profile and claims.
