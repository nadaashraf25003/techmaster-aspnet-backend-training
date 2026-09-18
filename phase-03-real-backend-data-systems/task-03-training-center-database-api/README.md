# Task 03: Training Center Database API

> **Phase 03: Real Backend Data Systems • TechMaster Academy**  
> **Topic:** ASP.NET Core Web API, EF Core SQL Server Integration, Clean Layered Architecture, DTO Projections, Service Layer, Swagger, and Postman Test Suites.  

---

## 🎯 Executive Summary & Mission

Task 03 delivers the core backend application for **TechMaster Academy**: a production-grade, database-driven RESTful API built on ASP.NET Core and Entity Framework Core with SQL Server.

The API adheres to strict software engineering and architecture principles:
1. **Never Expose EF Core Entities:** All controllers communicate strictly via Request and Response Data Transfer Objects (DTOs).
2. **Service Layer Abstraction:** Business validation, capacity rules, duplicate checks, and status transitions reside in dedicated service classes.
3. **Uniform Response Envelope:** Every API endpoint returns a standardized `ApiResponse<T>` envelope with status codes, messages, and validation errors.
4. **Resilient Database Configuration:** Configured for SQL Server with connection resiliency and automatic In-Memory fallback for testing.
5. **Comprehensive Interactive Documentation:** Fully documented with Swagger / OpenAPI and accompanied by an automated Postman test collection.

---

## 📂 Repository & Project Structure

```text
task-03-training-center-database-api/
├── README.md
├── TrainingCenter.Api/
│   ├── Controllers/
│   │   ├── BaseApiController.cs
│   │   ├── StudentsController.cs
│   │   ├── InstructorsController.cs
│   │   ├── TracksController.cs
│   │   ├── EnrollmentsController.cs
│   │   ├── PaymentsController.cs
│   │   └── ReportsController.cs
│   ├── Data/
│   │   ├── TrainingCenterDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── StudentConfiguration.cs
│   │   │   ├── InstructorConfiguration.cs
│   │   │   ├── TrainingTrackConfiguration.cs
│   │   │   ├── EnrollmentConfiguration.cs
│   │   │   └── PaymentConfiguration.cs
│   │   └── DbInitializer.cs
│   ├── Entities/
│   │   ├── Student.cs
│   │   ├── Instructor.cs
│   │   ├── TrainingTrack.cs
│   │   ├── Enrollment.cs
│   │   └── Payment.cs
│   ├── DTOs/
│   │   ├── Common/
│   │   │   ├── ApiResponse.cs
│   │   │   ├── PagedResult.cs
│   │   │   └── PaginationParams.cs
│   │   ├── Students/
│   │   │   └── StudentDtos.cs
│   │   ├── Instructors/
│   │   │   └── InstructorDtos.cs
│   │   ├── Tracks/
│   │   │   └── TrackDtos.cs
│   │   ├── Enrollments/
│   │   │   └── EnrollmentDtos.cs
│   │   ├── Payments/
│   │   │   └── PaymentDtos.cs
│   │   └── Reports/
│   │       └── ReportDtos.cs
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   └── ServiceInterfaces.cs
│   │   └── Implementations/
│   │       ├── StudentService.cs
│   │       ├── InstructorService.cs
│   │       ├── TrackService.cs
│   │       ├── EnrollmentService.cs
│   │       ├── PaymentService.cs
│   │       └── ReportService.cs
│   ├── Common/
│   │   ├── Enums.cs
│   │   ├── BaseAuditableEntity.cs
│   │   ├── Exceptions/
│   │   │   └── AppExceptions.cs
│   │   └── Middleware/
│   │       └── GlobalExceptionHandlingMiddleware.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   └── TrainingCenter.Api.csproj
├── postman/
│   ├── TechMaster_Training_Center_API.postman_collection.json
│   └── TechMaster_Training_Center_API.postman_environment.json
└── evidence/
    └── README.md
```

---

## 📡 Complete REST API Endpoint Specification

### 1. Students Endpoints (`/api/students`)
| Method | Route | Purpose | Success Code | Failure Codes |
| :--- | :--- | :--- | :---: | :---: |
| **GET** | `/api/students` | Return paginated students with search (`searchTerm`) and active filter (`isActive`). | `200 OK` | `400` |
| **GET** | `/api/students/{id}` | Return single student details with enrollment summary. | `200 OK` | `404` |
| **POST** | `/api/students` | Create new student with unique email validation. | `201 Created` | `400`, `409` |
| **PUT** | `/api/students/{id}` | Update student profile and record `UpdatedAt` timestamp. | `200 OK` | `400`, `404`, `409` |
| **DELETE** | `/api/students/{id}` | Soft delete student (`IsDeleted = 1`). Row is preserved in database. | `200 OK` | `404` |
| **GET** | `/api/students/{id}/enrollments` | Return all enrollments for a student. | `200 OK` | `404` |

---

### 2. Instructors Endpoints (`/api/instructors`)
| Method | Route | Purpose | Success Code | Failure Codes |
| :--- | :--- | :--- | :---: | :---: |
| **GET** | `/api/instructors` | Return all instructors with active filter and assigned track counts. | `200 OK` | `400` |
| **GET** | `/api/instructors/{id}` | Return instructor details and all assigned training tracks. | `200 OK` | `404` |
| **POST** | `/api/instructors` | Create instructor with unique email validation. | `201 Created` | `400`, `409` |
| **PUT** | `/api/instructors/{id}` | Update instructor specialization, bio, or contact info. | `200 OK` | `400`, `404`, `409` |
| **GET** | `/api/instructors/{id}/tracks` | Return all training tracks assigned to the instructor. | `200 OK` | `404` |

---

### 3. Training Tracks Endpoints (`/api/tracks`)
| Method | Route | Purpose | Success Code | Failure Codes |
| :--- | :--- | :--- | :---: | :---: |
| **GET** | `/api/tracks` | Return paginated tracks with filters: `keyword`, `level`, `status`, `instructorId`. | `200 OK` | `400` |
| **GET** | `/api/tracks/{id}` | Return track details with instructor info, capacity, and available seats. | `200 OK` | `404` |
| **POST** | `/api/tracks` | Create track with instructor existence check, capacity validation, and date rules. | `201 Created` | `400`, `409` |
| **PUT** | `/api/tracks/{id}` | Update track syllabus, pricing, capacity, and dates. | `200 OK` | `400`, `404`, `409` |
| **DELETE** | `/api/tracks/{id}` | Soft delete track (Blocked if active enrollments exist). | `200 OK` | `400`, `404` |
| **GET** | `/api/tracks/{id}/students` | Return list of enrolled students in the track. | `200 OK` | `404` |

---

### 4. Enrollments Endpoints (`/api/enrollments`)
| Method | Route | Purpose | Success Code | Failure Codes |
| :--- | :--- | :--- | :---: | :---: |
| **GET** | `/api/enrollments` | Return enrollments with filters: `status`, `trackId`, `studentId`, `paymentStatus`. | `200 OK` | `400` |
| **GET** | `/api/enrollments/{id}` | Return enrollment details with student, track, and payment ledger. | `200 OK` | `404` |
| **POST** | `/api/enrollments` | Enroll student in track (Enforces capacity limits & prevents duplicate enrollments). | `201 Created` | `400`, `404`, `409` |
| **PUT** | `/api/enrollments/{id}/status` | Update enrollment status (`Pending`, `Active`, `Completed`, `Dropped`), progress %, and result. | `200 OK` | `400`, `404` |
| **GET** | `/api/enrollments/{id}/payments` | Return complete payment history for an enrollment. | `200 OK` | `404` |

---

### 5. Payments Endpoints (`/api/payments`)
| Method | Route | Purpose | Success Code | Failure Codes |
| :--- | :--- | :--- | :---: | :---: |
| **GET** | `/api/payments` | Return payments with date range (`startDate`, `endDate`), `status`, and `method` filters. | `200 OK` | `400` |
| **GET** | `/api/payments/{id}` | Return single payment transaction details. | `200 OK` | `404` |
| **POST** | `/api/payments` | Process payment for an enrollment (Validates `Amount > 0` and unique reference number). | `201 Created` | `400`, `404`, `409` |
| **PUT** | `/api/payments/{id}/status` | Update payment settlement status (`Completed`, `Failed`, `Refunded`). | `200 OK` | `400`, `404` |

---

### 6. Analytical & Business Reports Endpoints (`/api/reports`)
| Method | Route | Purpose | Success Code |
| :--- | :--- | :--- | :---: |
| **GET** | `/api/reports/dashboard-summary` | Real-time high-level KPIs (Total students, active enrollments, realized revenue). | `200 OK` |
| **GET** | `/api/reports/unpaid-enrollments` | List of all enrollments with outstanding/unpaid balance. | `200 OK` |
| **GET** | `/api/reports/track-capacity` | Capacity, seat availability, and utilization rate per track. | `200 OK` |
| **GET** | `/api/reports/revenue-summary` | Total realized cash, expected book value, and average ticket size. | `200 OK` |
| **GET** | `/api/reports/revenue-by-track` | Revenue grouped by training track with collection percentage. | `200 OK` |
| **GET** | `/api/reports/instructor-workload` | Active tracks and active students supervised per instructor. | `200 OK` |

---

## 🛡️ Business Rules & Validation Logic

| Rule Domain | Enforcement Mechanism | Failure Response |
| :--- | :--- | :--- |
| **Email Uniqueness** | `IStudentService` / `IInstructorService` + SQL Server Unique Filtered Index | `409 Conflict` ("A student with email '...' already exists.") |
| **Duplicate Enrollment** | `IEnrollmentService` + SQL Server Composite Unique Constraint `(StudentId, TrainingTrackId)` | `409 Conflict` ("Student is already enrolled in this track.") |
| **Track Capacity Protection** | `IEnrollmentService` validates `currentActiveCount < Capacity` before insertion | `400 Bad Request` ("Track has reached its maximum capacity of X students.") |
| **Track Date Constraints** | `ITrackService` + SQL Server Check Constraint `EndDate >= StartDate` | `400 Bad Request` ("EndDate cannot be earlier than StartDate.") |
| **Safe Track Deletion** | `ITrackService` prevents soft-deletion of tracks containing active students | `400 Bad Request` ("Cannot delete track because it has active student enrollments.") |
| **Payment Reference Idempotency**| `IPaymentService` + SQL Server Unique Index on `ReferenceNumber` | `409 Conflict` ("A payment with reference number '...' already exists.") |
| **Soft Delete Handling** | EF Core Global Query Filter `.HasQueryFilter(e => !e.IsDeleted)` | Soft-deleted rows are hidden from general queries automatically. |

---

## 🚀 How to Run Locally

### 1. Start the API
```powershell
dotnet run --project phase-03-real-backend-data-systems/task-03-training-center-database-api/TrainingCenter.Api/TrainingCenter.Api.csproj
```

### 2. Access Interactive Swagger Documentation
Open your browser and navigate to:
```text
http://localhost:5000  (or https://localhost:7001)
```

### 3. Run Automated Postman Test Collection
1. Open **Postman**.
2. Click **Import** and select:
   * [TechMaster_Training_Center_API.postman_collection.json](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-03-training-center-database-api/postman/TechMaster_Training_Center_API.postman_collection.json)
   * [TechMaster_Training_Center_API.postman_environment.json](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-03-training-center-database-api/postman/TechMaster_Training_Center_API.postman_environment.json)
3. Select the **TechMaster Training Center API - Local Environment**.
4. Click **Run Collection** to execute all positive and negative test cases.

---

## ✅ Acceptance Criteria Checklist

- [x] Full ASP.NET Core Web API with clean layered architecture.
- [x] SQL Server database persistence with EF Core configurations and seed data.
- [x] Zero entity leakage in controllers (100% DTO usage).
- [x] Standard `ApiResponse<T>` wrapper on all endpoints.
- [x] Students CRUD + Soft delete + Pagination.
- [x] Instructors CRUD + Assigned tracks.
- [x] Tracks CRUD + Filters + Capacity validation + Safe delete.
- [x] Enrollments with capacity checking and duplicate protection.
- [x] Payments processing with unique reference validation and status tracking.
- [x] 6 Analytical reporting endpoints (Dashboard, Unpaid, Capacity, Revenue, Workload).
- [x] Swagger documentation enabled and served at application root.
- [x] Postman collection and environment files included.
- [x] Testing evidence and payload samples documented.
