# Task 00: Phase 04 Sprint Setup & Baseline Analysis

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  
> **Target:** 100/100 Delivery Standard  

---

## 📌 1. Mission & Objectives

The goal of **Task 00** is to prepare the Phase 04 15-day sprint like a senior backend engineering team:
1. Establish the **Phase 04** folder structure within the existing training repository without creating extraneous repositories.
2. Port the latest stable Phase 03 project (`TrainingCenter.Api`) into `task-00-sprint-setup` as the official, verified **Baseline Solution**.
3. Thoroughly audit and document the **current architectural and security limitations** of the Phase 03 system before implementing authentication and authorization.
4. Define the **Sprint Backlog**, **RBAC Matrix**, and **Target Architecture** for Phase 04.
5. Ensure the baseline project compiles and runs with **0 errors and 0 warnings**.

---

## 🏗️ 2. Baseline Overview (Phase 03 System)

The baseline project is the **TechMaster Training Center Registration API** built in Phase 03.

### Baseline Specifications:
- **Framework:** .NET 10.0 (ASP.NET Core Web API)
- **Database:** Microsoft SQL Server with Entity Framework Core (Code-First)
- **Entities:**
  - `Student` (Id, FullName, Email, PhoneNumber, NationalId, Status, CreatedAtUtc)
  - `Instructor` (Id, FullName, Email, Specialization, HourlyRate, IsActive, CreatedAtUtc)
  - `Track` (Id, Name, Code, Description, Price, Capacity, DurationHours, Status, InstructorId)
  - `Enrollment` (Id, StudentId, TrackId, EnrollmentDateUtc, Status, TotalAmount, DiscountAmount, FinalAmount, BalanceDue, Notes)
  - `Payment` (Id, EnrollmentId, Amount, PaymentDateUtc, Method, TransactionReference, Status, Notes)
- **Controllers & Endpoints (Phase 03 Baseline):**
  - `StudentsController` — CRUD, filtering by status/search, pagination.
  - `InstructorsController` — CRUD, active status toggling, assigned tracks.
  - `TracksController` — CRUD, enrollment roster, capacity management.
  - `EnrollmentsController` — Enrollment creation, cancellation, status updates.
  - `PaymentsController` — Payment processing against enrollment balance.
  - `ReportsController` — Financial summaries, instructor workloads, top tracks, overdue balances.
  - `HealthController` — Database connectivity and status check.

---

## ⚠️ 3. Current Phase 03 Limitations & Security Gaps

Before starting Phase 04, a security and architecture audit identified the following critical vulnerabilities and design gaps in the Phase 03 baseline:

| Gap / Vulnerability | Description in Phase 03 | Security Risk / Business Impact | Phase 04 Remediation |
| :--- | :--- | :--- | :--- |
| **1. Anonymous CRUD** | All endpoints are open (`AllowAnonymous`). Anyone with network access can query, create, edit, or delete any record. | Total lack of confidentiality and data integrity. | Require JWT Authentication (`[Authorize]`) across all domain endpoints. |
| **2. Lack of Identity & User Accounts** | No `User`, `Role`, or `Credential` entities exist. The system only tracks `Student` and `Instructor` domain profiles without login credentials. | No user authentication or accountability. | Implement `User` entity, `Role` enum/claims, password hashing with cryptographic salt, and login flow. |
| **3. No Role-Based Access Control (RBAC)** | A student or unauthenticated guest can access financial reports, delete tracks, or approve payments. | Privilege escalation; any caller has full administrative privileges. | Enforce `Admin`, `Instructor`, and `Student` role policies across all endpoints. |
| **4. Lack of Resource Ownership Checks** | Endpoints query by raw IDs without checking who owns the record (e.g. `GET /api/enrollments/5`). | **IDOR (Insecure Direct Object Reference)** — Student A can view/cancel Student B's enrollments or payments. | Implement ownership validation in services based on `ClaimsPrincipal` (`User.GetStudentId()`). |
| **5. Missing Audit Log / History** | When a payment is recorded or an enrollment is cancelled, only the entity state is updated. There is no historical record of *who* performed the action. | Zero compliance, no forensic trail for disputes or fraud investigation. | Create `ActivityLog` / `AuditTrail` subsystem recording Actor, Action, Entity, TimestampUtc, and Old/New values. |
| **6. Hardcoded Secret Risks** | JWT configuration is not yet structured into environment-aware options (`JwtOptions`). | Risk of token forgery if secrets are misconfigured. | Use strongly typed `IOptions<JwtOptions>` with signing keys configured via environment variables / User Secrets. |
| **7. Error Response Inconsistencies** | While Phase 03 added initial middleware, some endpoints bypass `ApiResponse<T>` envelope or leak stack details. | Client integration fragility and potential server fingerprinting. | Standardize `ApiResponse<T>`, RFC 7807 problem details, and correlation IDs. |

---

## 🎯 4. Target Phase 04 Security & Role Architecture

```
                                   ┌─────────────────────────────────┐
                                   │       Incoming HTTP Request     │
                                   └────────────────┬────────────────┘
                                                    │
                                                    ▼
                                   ┌─────────────────────────────────┐
                                   │  Global Exception Middleware    │
                                   └────────────────┬────────────────┘
                                                    │
                                                    ▼
                                   ┌─────────────────────────────────┐
                                   │  Authentication Middleware (JWT)│
                                   └────────────────┬────────────────┘
                                                    │
                                                    ▼
                                   ┌─────────────────────────────────┐
                                   │  Authorization Middleware (RBAC)│
                                   └────────────────┬────────────────┘
                                                    │
                   ┌────────────────────────────────┼────────────────────────────────┐
                   │                                │                                │
                   ▼                                ▼                                ▼
       ┌───────────────────────┐        ┌───────────────────────┐        ┌───────────────────────┐
       │     Admin Portal      │        │   Instructor Portal   │        │     Student Portal    │
       │  [Authorize(Roles=    │        │  [Authorize(Roles=    │        │  [Authorize(Roles=    │
       │     "Admin")]         │        │    "Instructor")]     │        │     "Student")]       │
       ├───────────────────────┤        ├───────────────────────┤        ├───────────────────────┤
       │ - Manage Users & Roles│        │ - View Assigned Tracks│        │ - View My Enrollments │
       │ - Manage Tracks/Prices│        │ - View My Students    │        │ - Make My Payment     │
       │ - Financial Reports   │        │ - Session Rosters     │        │ - View My Balance     │
       │ - View Audit Timeline │        │ - Update Profile      │        │ - Update My Profile   │
       └───────────┬───────────┘        └───────────┬───────────┘        └───────────┬───────────┘
                   │                                │                                │
                   └────────────────────────────────┼────────────────────────────────┘
                                                    │
                                                    ▼
                                   ┌─────────────────────────────────┐
                                   │     Application Service Layer   │
                                   │  - Business Rules & Ownership   │
                                   │  - Audit Trail Generation       │
                                   └────────────────┬────────────────┘
                                                    │
                                                    ▼
                                   ┌─────────────────────────────────┐
                                   │    EF Core DbContext & SQL DB   │
                                   └─────────────────────────────────┘
```



## 🧪 6. Baseline Verification & Compilation Proof

The Phase 03 baseline project has been copied into `task-00-sprint-setup` and validated:

```powershell
dotnet build "phase-04-secure-professional-backend/task-00-sprint-setup/TrainingCenter.sln"
```

**Build Verification Result:**
- **Status:** Succeeded
- **Warnings:** 0
- **Errors:** 0
- **Target Framework:** .NET 10.0 (`net10.0`)
- **Database Engine:** Microsoft SQL Server (with in-memory fallback for local unit tests)
- **Health Check Endpoint:** `GET /health` (`{"status": "Healthy", "databaseConnected": true}`)
