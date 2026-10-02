# Task 01: Authentication Foundation • Identity • Password Hashing • JWT • Claims

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  
> **Score Standard:** 100/100 (Top Student Performance Standard)  
> **Core Theme:** Building a production-grade authentication and identity platform for the TechMaster Training Center API.

---

## 📌 1. Executive Summary & Mission

In **Phase 03**, the API operated with anonymous CRUD access. In real-world enterprise engineering, every single transaction and query must be bound to an authenticated identity, an explicit role, and cryptographically verified claims.

**Task 01 delivers the foundational Identity & Security Layer:**
1. **ApplicationUser Entity & RBAC Enums:** Modeled with audit timestamps, profile linkages, and active status checks.
2. **Cryptographically Secure Password Hashing:** RFC 2898 PBKDF2 with HMAC-SHA512, 100,000 iterations, 128-bit random salt, and constant-time verification against timing attacks.
3. **JWT Access Token Issuance:** RFC 7519 standard JWTs signed with HMAC-SHA256, carrying standard and custom claims (`sub`, `name`, `email`, `role`, `studentId`, `instructorId`).
4. **Current User Extraction (`/api/auth/me`):** Claim-driven profile retrieval with `[Authorize]` enforcement.
5. **Advanced Session Management:** Refresh Token rotation (`/api/auth/refresh-token`) and explicit revocation (`/api/auth/logout`).
6. **Swagger UI Security Integration:** Direct interactive token testing via standard OpenAPI Bearer security scheme.

---

## 🏗️ 2. Identity Architecture & System Topology

```mermaid
flowchart TD
    Client["Client / Postman / Swagger UI"]
    
    subgraph IdentityLayer ["Authentication & Identity System"]
        AuthController["AuthController\n(/api/auth/*)"]
        AuthService["AuthService\n(Registration, Login, Refresh, Claims)"]
        PwdHasher["PasswordHasherService\n(PBKDF2-SHA512, 100k iters, Salt)"]
        JwtService["JwtTokenService\n(Access Tokens, Claims, Refresh Tokens)"]
    end
    
    subgraph DataStore ["EF Core DbContext & SQL Database"]
        UsersTable[("Users Table\n(ApplicationUser)")]
        RefreshTable[("RefreshTokens Table")]
        StudentsTable[("Students Table")]
        InstructorsTable[("Instructors Table")]
    end
    
    Client -->|"POST /api/auth/register"| AuthController
    Client -->|"POST /api/auth/login"| AuthController
    Client -->|"GET /api/auth/me (Bearer JWT)"| AuthController
    Client -->|"POST /api/auth/refresh-token"| AuthController
    
    AuthController --> AuthService
    AuthService --> PwdHasher
    AuthService --> JwtService
    AuthService --> UsersTable
    AuthService --> RefreshTable
    AuthService --> StudentsTable
    AuthService --> InstructorsTable
```

---

## 🗄️ 3. Entity & Data Model Specifications

### `ApplicationUser` Entity
| Field | Type | Constraint | Description |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | PK, Identity | Unique user identifier. |
| `FullName` | `string` | Max 100, Required | User's legal name. |
| `Email` | `string` | Max 150, Unique Index | Unique login email (normalized). |
| `PasswordHash` | `string` | Max 500, Required | Formatted `{hash}:{salt}:{iterations}:{algo}` string. |
| `Role` | `UserRole` | Enum (`Admin`, `Instructor`, `Student`) | Stored as string (`MaxLength(20)`). |
| `IsActive` | `bool` | Default `true` | Account active toggle. Deactivated accounts cannot login. |
| `StudentId` | `int?` | Nullable FK -> `Students` | Optional 1-to-1 link to domain Student profile. |
| `InstructorId` | `int?` | Nullable FK -> `Instructors` | Optional 1-to-1 link to domain Instructor profile. |
| `LastLoginAtUtc` | `DateTime?` | Nullable | Audit tracking for login timestamps. |
| `CreatedAt` | `DateTime` | UTC, Required | Creation timestamp from `BaseAuditableEntity`. |
| `LastModifiedAt`| `DateTime?` | UTC, Nullable | Update timestamp from `BaseAuditableEntity`. |

---

## 🔒 4. Password Security & Hashing Standard

TechMaster enforces strict cryptographic standards for user passwords:

- **Algorithm:** RFC 2898 PBKDF2 (Password-Based Key Derivation Function 2)
- **Hash Function:** `HMAC-SHA512`
- **Salt Length:** 128 bits (16 bytes) generated via `RandomNumberGenerator.GetBytes()`
- **Key Length:** 256 bits (32 bytes)
- **Iteration Count:** 100,000 iterations (protects against GPU-accelerated brute force)
- **Timing Attack Mitigation:** Verification utilizes `CryptographicOperations.FixedTimeEquals()` to guarantee constant-time evaluation regardless of character match positions.

---

## 🛡️ 5. JWT Claims Architecture & Security Rules

### Claims Encoded in Access Tokens:
| Claim Name | Type | Value Source | Purpose |
| :--- | :--- | :--- | :--- |
| `sub` / `nameid` | `ClaimTypes.NameIdentifier` | `user.Id.ToString()` | Unique subject identifier for the token. |
| `unique_name` | `ClaimTypes.Name` | `user.FullName` | User display name. |
| `email` | `ClaimTypes.Email` | `user.Email` | User email address. |
| `role` | `ClaimTypes.Role` | `user.Role.ToString()` | Used by ASP.NET Core RBAC middleware (`[Authorize(Roles = ...)]`). |
| `jti` | `JwtRegisteredClaimNames.Jti` | `Guid.NewGuid().ToString()` | Unique JWT ID to prevent replay attacks. |
| `studentId` | Custom Claim | `user.StudentId.ToString()` | Direct access to linked student domain ID. |
| `instructorId`| Custom Claim | `user.InstructorId.ToString()`| Direct access to linked instructor domain ID. |

### ⛔ What Must NEVER Go Inside a JWT:
1. ❌ Passwords or Password Hashes.
2. ❌ Database connection strings or master secret keys.
3. ❌ Personally Identifiable Information (PII) like National ID or full credit card numbers.
4. ❌ Ephemeral business state or rapidly changing balances.

---

## 🚀 6. API Endpoints Reference

### 1. `POST /api/auth/register` (Public)
Registers a new user (Student or Instructor) and automatically creates and links their corresponding domain profile.

```http
POST /api/auth/register HTTP/1.1
Content-Type: application/json

{
  "fullName": "Mohamed Ayman",
  "email": "mohamed.ayman@example.com",
  "password": "Password@123456",
  "confirmPassword": "Password@123456",
  "role": "Student",
  "phoneNumber": "+201012345678",
  "address": "Nasr City, Cairo"
}
```

**Response (`201 Created`):**
```json
{
  "success": true,
  "message": "User registered successfully.",
  "data": {
    "userId": 5,
    "fullName": "Mohamed Ayman",
    "email": "mohamed.ayman@example.com",
    "role": "Student",
    "linkedStudentId": 8,
    "linkedInstructorId": null,
    "createdAtUtc": "2026-10-02T19:07:07.0318164Z"
  },
  "errors": [],
  "statusCode": 201,
  "timestamp": "2026-10-02T19:07:07.0472319Z"
}
```

---

### 2. `POST /api/auth/login` (Public)
Authenticates credentials, verifies active status, and returns a signed JWT and Refresh Token.

```http
POST /api/auth/login HTTP/1.1
Content-Type: application/json

{
  "email": "admin@techmaster.com",
  "password": "Admin@123456"
}
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "message": "Authentication successful.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "G6siqKvnQq5VldcuZJXH5PmTz3mc7ZEk...",
    "expiresAt": "2026-10-02T20:06:48.0805242Z",
    "userId": 1,
    "fullName": "TechMaster System Administrator",
    "email": "admin@techmaster.com",
    "role": "Admin",
    "linkedStudentId": null,
    "linkedInstructorId": null
  },
  "errors": [],
  "statusCode": 200,
  "timestamp": "2026-10-02T19:06:48.4015336Z"
}
```

---

### 3. `GET /api/auth/me` (`[Authorize]`)
Retrieves the identity, claims, and profile bindings for the caller using the Bearer token.

```http
GET /api/auth/me HTTP/1.1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (`200 OK`):**
```json
{
  "success": true,
  "message": "Current user retrieved successfully.",
  "data": {
    "userId": 1,
    "fullName": "TechMaster System Administrator",
    "email": "admin@techmaster.com",
    "role": "Admin",
    "isActive": true,
    "linkedStudentId": null,
    "linkedInstructorId": null,
    "lastLoginAt": "2026-10-02T19:06:53.7090547Z"
  },
  "errors": [],
  "statusCode": 200,
  "timestamp": "2026-10-02T19:06:53.9630139Z"
}
```

---

### 4. `POST /api/auth/refresh-token` (Public)
Rotates a valid refresh token, invalidating the old token and generating a new JWT + refresh token pair.

```http
POST /api/auth/refresh-token HTTP/1.1
Content-Type: application/json

{
  "refreshToken": "uEqLWGik6e4rhhNhTBTRGln1EjLyAXUXBcuCzvh2DRpRLh6AfNsxw4tt33VqoyPzRqo608CH3nUAN5VJomg"
}
```

---

### 5. `POST /api/auth/change-password` (`[Authorize]`)
Allows an authenticated user to change their password by confirming their old password.

```http
POST /api/auth/change-password HTTP/1.1
Authorization: Bearer <JWT>
Content-Type: application/json

{
  "currentPassword": "Student@123456",
  "newPassword": "NewStudent@123456",
  "confirmNewPassword": "NewStudent@123456"
}
```

---

### 6. `POST /api/auth/logout` (`[Authorize]`)
Revokes the caller's active refresh tokens to terminate the session.

```http
POST /api/auth/logout HTTP/1.1
Authorization: Bearer <JWT>
Content-Type: application/json

{
  "refreshToken": "<OptionalSpecificToken>"
}
```

---

## 🧪 7. Seeded Test Credentials

The database automatically seeds default users with pre-hashed passwords on startup for testing:

| Email | Password | Role | Description |
| :--- | :--- | :---: | :--- |
| `admin@techmaster.com` | `Admin@123456` | **Admin** | Superuser with full administrative access. |
| `instructor@techmaster.com` | `Instructor@123456` | **Instructor** | Linked to Instructor profile (`Eng. Mohamed Ali`). |
| `student@techmaster.com` | `Student@123456` | **Student** | Linked to Student profile (`Nada Ashraf`). |
| `inactive@techmaster.com` | `Inactive@123456` | **Student** | Deactivated account for testing `403 Forbidden` login rejection. |

---

## 📦 8. Postman Collection & Automated Tests

A complete Postman test collection is located at:
📁 `phase-04-secure-professional-backend/task-01-auth-foundation/postman/TechMaster_Phase04_Task01_Auth.postman_collection.json`

### Test Suites Included:
1. **01. Positive Auth Flows:**
   - Register New Student (saves `new_user_id`).
   - Login as Admin (saves `admin_token`, `admin_refresh_token`).
   - Login as Instructor (saves `instructor_token`).
   - Login as Student (saves `student_token`, `student_refresh_token`).
   - `GET /api/auth/me` with Admin Token.
   - `GET /api/auth/me` with Student Token.
   - Refresh Token Rotation.
   - Logout & Token Invalidation.
2. **02. Negative & Security Test Cases:**
   - Reject unauthenticated request to `/api/auth/me` (`401 Unauthorized`).
   - Reject self-registration with Admin role (`400 Bad Request`).
   - Reject duplicate email registration (`409 Conflict`).
   - Reject login for deactivated account (`403 Forbidden`).
   - Reject login with incorrect password (`401 Unauthorized`).

---

## ✅ 9. Task 01 Acceptance Criteria Checklist

- [x] `ApplicationUser` and `RefreshToken` entities configured with Fluent API.
- [x] `IPasswordHasherService` using PBKDF2 with SHA512, 100k iterations, and constant-time verification.
- [x] `IJwtTokenService` generating cryptographically signed JWTs with sub, email, role, and custom domain IDs.
- [x] `POST /api/auth/register` with role guard (blocking Admin registration) and email uniqueness check.
- [x] `POST /api/auth/login` with deactivated account rejection (`403`) and invalid credential handling (`401`).
- [x] `GET /api/auth/me` with `[Authorize]` returning current identity and profile linkages.
- [x] `POST /api/auth/change-password`, `POST /api/auth/refresh-token`, and `POST /api/auth/logout` implemented.
- [x] Swagger UI equipped with JWT Bearer authorization scheme.
- [x] Zero hardcoded secrets; configuration bound via strongly-typed `JwtOptions`.
- [x] Postman collection created with automated test scripts.
- [x] Solution builds with 0 Errors and 0 Warnings.
