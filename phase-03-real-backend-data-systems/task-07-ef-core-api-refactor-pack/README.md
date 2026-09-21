# Task 07 — EF Core / API Refactor Pack

## 1. Overview & Refactoring Mission

**Task 07** tackles a critical real-world engineering challenge: **inheriting poorly written EF Core API code and refactoring it into a clean, layered, high-performance, review-ready backend system** without altering business intentions or breaking legitimate consumer capabilities.

In production environments, unoptimized EF Core usage often leads to memory leaks, thread starvation, database corruption, security vulnerabilities, and unexpected HTTP behaviors. This refactoring pack transforms a legacy "fat controller" into clean architecture with strict DTO contracts, asynchronous execution, domain validation, server-side pagination, projection, and soft-delete safeguards.

---

## 2. Architecture Comparison: Before vs. After

### Legacy Anti-Pattern Architecture (Before)
```
[HTTP Client]
      │
      ▼
┌────────────────────────────────────────────────────────┐
│  BadEnrollmentsController (Fat Controller)            │
│  - Directly accepts EF Entities from Request Body       │
│  - Synchronous blocking calls (.ToList, .SaveChanges)  │
│  - Direct AppDbContext injection                       │
│  - Missing duplicate checks & capacity guards          │
│  - Hard deletes data (.Remove)                         │
│  - Inappropriate status codes (200 OK for errors)       │
└──────────────────────────┬─────────────────────────────┘
                           │
                           ▼
                  [(AppDbContext)]
                           │
                           ▼
                 [SQL Server Database]
```

### Refactored Clean Architecture (After)
```
[HTTP Client / Postman / Swagger]
               │
               ▼
┌────────────────────────────────────────────────────────┐
│  GlobalExceptionHandlingMiddleware                     │
│  - Catches domain exceptions (NotFound, Conflict, etc.)│
│  - Returns standardized ApiResponse<T> envelope        │
└──────────────────────────────┬─────────────────────────┘
                               │
                               ▼
┌────────────────────────────────────────────────────────┐
│  EnrollmentsController (Thin Controller)               │
│  - Accepts validated Request DTOs                      │
│  - Returns Response DTOs wrapped in ApiResponse<T>     │
│  - Standard HTTP Status Codes (200, 201, 204, 400, 409)│
└──────────────────────────────┬─────────────────────────┘
                               │
                               ▼
┌────────────────────────────────────────────────────────┐
│  EnrollmentService (Domain & Application Layer)        │
│  - Asynchronous EF Core (ToListAsync, SaveChangesAsync)│
│  - Duplicate Active Enrollment Prevention (Guard)      │
│  - Track Capacity & Status Validation                  │
│  - Positive Payment Validation & Balance Bounds        │
│  - Soft Delete (IsDeleted = true, DeletedAtUtc)        │
│  - Projection (.Select) & Server-Side Pagination       │
└──────────────────────────────┬─────────────────────────┘
                               │
                               ▼
┌────────────────────────────────────────────────────────┐
│  TrainingCenterDbContext (EF Core Data Access Layer)   │
│  - Global Soft-Delete Query Filters (HasQueryFilter)   │
│  - Automatic Audit & Soft-Delete SaveChanges Override  │
│  - Resilient SQL Server Retry Policy                   │
└──────────────────────────────┬─────────────────────────┘
                               │
                               ▼
                 [Microsoft SQL Server Database]
```

---

## 3. Top 10+ Problems Identified in Legacy Code

| # | Problem Name | Legacy Code Snippet | Architectural & Operational Impact |
| :-: | :--- | :--- | :--- |
| **1** | **Direct Entity Leakage in Response** | `public IActionResult GetAll() { var data = _db.Enrollments.Include(...).ToList(); return Ok(data); }` | Exposes internal database schema, triggers circular JSON serialization loops with navigations, and leaks sensitive database internals. |
| **2** | **Mass-Assignment / Over-Posting Vulnerability** | `public IActionResult Create(Enrollment enrollment)` | Allows malicious clients to inject or override database-controlled fields (`Id`, `Status`, `IsDeleted`, `CreatedAtUtc`). |
| **3** | **Synchronous Blocking Operations** | `.ToList()`, `.SaveChanges()`, `.FirstOrDefault()`, `.Find()` | Blocks ASP.NET Core ThreadPool worker threads during I/O. Causes thread starvation and severe latency degradation under concurrency. |
| **4** | **Unbounded Fetching (No Pagination / Filtering)** | `_db.Enrollments.Include(e => e.Student).Include(...).ToList()` | Loads entire tables into server memory. Causes severe Out-Of-Memory (OOM) crashes and High Database I/O on large datasets. |
| **5** | **Absence of LINQ Projection (`.Select()`)** | Entire entity graphs loaded into EF Core Change Tracker. | Loads unneeded columns into memory and tracks entities even when performing read-only queries, wasting RAM and CPU cycles. |
| **6** | **Missing Duplicate Active Enrollment Guard** | `_db.Enrollments.Add(enrollment); _db.SaveChanges();` | Allows the same student to be enrolled in the same track multiple times concurrently, corrupting academic and financial records. |
| **7** | **Ignoring Track Capacity Limits** | No capacity check prior to enrollment insertion. | Allows tracks to exceed physical classroom or instructor limits (e.g. 100 students in a 20-seat track). |
| **8** | **Permanent Data Loss via Hard Delete** | `_db.Enrollments.Remove(item); _db.SaveChanges();` | Permanently destroys audit trails, historic financial payments, and violates data retention and compliance policies. |
| **9** | **Incorrect & Misleading HTTP Status Codes** | `return Ok("not found"); return Ok("missing"); return Ok("deleted");` | Returns `200 OK` for missing resources and errors, breaking standard REST API client error-handling libraries. |
| **10** | **Fat Controller Anti-Pattern** | Direct `_db` manipulation inside Controller action methods. | Violates Single Responsibility Principle (SRP) and Separation of Concerns (SoC). Impossible to unit test without database. |
| **11** | **Missing Payment Validation** | `var payment = new Payment { Amount = amount ... };` | Accepts negative or zero payments, and allows overpayment exceeding the remaining track balance. |
| **12** | **Inconsistent Response Envelope Shape** | Returns raw strings `"missing"` or raw entities interchangeably. | Forces clients to write unpredictable deserialization logic instead of relying on a consistent schema. |

---

## 4. Top 10+ Improvements Implemented in Refactored Code

| # | Improvement Name | Refactored Implementation | Engineering Benefit |
| :-: | :--- | :--- | :--- |
| **1** | **Strict Request & Response DTOs** | `CreateEnrollmentRequestDto`, `EnrollmentResponseDto`, `EnrollmentDetailResponseDto` | Completely encapsulates internal models, eliminates over-posting, and prevents circular reference serialization errors. |
| **2** | **End-to-End Asynchronous Execution** | `await _context.Enrollments.ToListAsync(cancellationToken)`, `await SaveChangesAsync()` | Releases threads back to the thread pool while awaiting I/O, boosting system throughput and scalability. |
| **3** | **Layered Service Abstraction** | `IEnrollmentService` and `EnrollmentService` | Decouples business rules from HTTP controllers, enabling isolated unit testing and clean maintainability. |
| **4** | **Duplicate Active Enrollment Guard** | `await _context.Enrollments.AnyAsync(e => e.StudentId == request.StudentId && e.TrainingTrackId == ... && e.Status == Active)` | Throws `ConflictException` (mapped to `409 Conflict`), maintaining database relationship integrity. |
| **5** | **Track Capacity & Status Enforcement** | `if (activeEnrollments >= track.Capacity) throw new BusinessRuleException(...)` | Protects training tracks from over-enrollment and rejects registrations on inactive/cancelled tracks. |
| **6** | **Payment Bounds & Positive Validation** | `if (request.Amount > remainingBalance) throw new BusinessRuleException(...)` | Guarantees payments are strictly positive, updates remaining balances accurately, and prevents overpayments. |
| **7** | **Soft-Delete Implementation** | `enrollment.IsDeleted = true; enrollment.DeletedAtUtc = DateTime.UtcNow;` with `HasQueryFilter(e => !e.IsDeleted)` | Preserves historical audit records and financial transactions while automatically hiding deleted records from active queries. |
| **8** | **Server-Side Pagination & Projection** | `query.Skip(...).Take(...).Select(e => new EnrollmentResponseDto { ... })` with `.AsNoTracking()` | Executes direct, optimized SQL `SELECT` statements, dramatically lowering memory footprint and database load. |
| **9** | **Standard RESTful HTTP Status Codes** | `201 Created` with `CreatedAtAction`, `204 No Content` for delete, `404 Not Found`, `409 Conflict`, `400 Bad Request` | Adheres strictly to RFC 9110 HTTP standards for seamless API consumer integration. |
| **10** | **Global Exception Handling & Envelope** | `GlobalExceptionHandlingMiddleware` + `ApiResponse<T>` | Standardizes error responses across all endpoints and prevents sensitive server stack traces from leaking to clients. |

---

## 5. Side-by-Side Code Refactoring Examples

### Example 1: Retrieval Endpoint (`GetAll`)

#### Before (Bad Code)
```csharp
[HttpGet]
public IActionResult GetAll()
{
    // Problem: returns full EF entities with navigation properties.
    // Problem: no pagination, no filtering, no projection.
    var data = _db.Enrollments
        .Include(e => e.Student)
        .Include(e => e.TrainingTrack)
        .Include(e => e.Payments)
        .ToList();
    return Ok(data);
}
```

#### After (Refactored Clean Code)
```csharp
// Controller
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponseDto>>), StatusCodes.Status200OK)]
public async Task<ActionResult<ApiResponse<PagedResult<EnrollmentResponseDto>>>> GetAll(
    [FromQuery] EnrollmentQueryParameters parameters,
    CancellationToken cancellationToken)
{
    var result = await _enrollmentService.GetAllPagedAsync(parameters, cancellationToken);
    return Ok(ApiResponse<PagedResult<EnrollmentResponseDto>>.SuccessResponse(result, "Enrollments retrieved successfully"));
}

// Service (Optimized SQL Projection with Server-Side Pagination)
public async Task<PagedResult<EnrollmentResponseDto>> GetAllPagedAsync(EnrollmentQueryParameters parameters, CancellationToken cancellationToken)
{
    var query = _context.Enrollments.AsNoTracking().AsQueryable();

    if (parameters.StudentId.HasValue) query = query.Where(e => e.StudentId == parameters.StudentId.Value);
    if (parameters.TrackId.HasValue) query = query.Where(e => e.TrainingTrackId == parameters.TrackId.Value);

    var totalCount = await query.CountAsync(cancellationToken);

    var items = await query
        .OrderByDescending(e => e.EnrollmentDate)
        .Skip((parameters.PageNumber - 1) * parameters.PageSize)
        .Take(parameters.PageSize)
        .Select(e => new EnrollmentResponseDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentName = e.Student != null ? e.Student.FullName : "Unknown",
            TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
            TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0,
            EnrollmentDate = e.EnrollmentDate,
            Status = e.Status,
            TotalPaidAmount = e.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => (decimal?)p.Amount) ?? 0m
        })
        .ToListAsync(cancellationToken);

    return new PagedResult<EnrollmentResponseDto>(items, totalCount, parameters.PageNumber, parameters.PageSize);
}
```

---

### Example 2: Deletion Endpoint (`Delete`)

#### Before (Bad Code)
```csharp
[HttpDelete("{id}")]
public IActionResult Delete(int id)
{
    var item = _db.Enrollments.Find(id);
    if (item == null)
    {
        return Ok("missing"); // wrong status code
    }
    // Problem: hard delete loses historical data.
    _db.Enrollments.Remove(item);
    _db.SaveChanges();
    return Ok("deleted");
}
```

#### After (Refactored Clean Code)
```csharp
// Controller
[HttpDelete("{id:int}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
{
    await _enrollmentService.SoftDeleteAsync(id, cancellationToken);
    return NoContent(); // HTTP 204 No Content
}

// Service (Soft Delete preserving audit & financial records)
public async Task SoftDeleteAsync(int id, CancellationToken cancellationToken)
{
    var enrollment = await _context.Enrollments.FindAsync(new object[] { id }, cancellationToken);
    if (enrollment == null)
    {
        throw new NotFoundException($"Enrollment with ID {id} was not found.");
    }

    enrollment.IsDeleted = true;
    enrollment.DeletedAtUtc = DateTime.UtcNow;
    enrollment.UpdatedAtUtc = DateTime.UtcNow;
    enrollment.Status = EnrollmentStatus.Cancelled;

    await _context.SaveChangesAsync(cancellationToken);
}
```

---

## 6. Meaningful Commits Log (Refactoring Progression)

To demonstrate a structured, professional refactoring workflow, the changes follow these 8 progressive stages:

1. **`feat(legacy): add original bad enrollment controller and initial entities`**
   - Preserves [`BadEnrollmentsController.cs`](TrainingCenter.Api/Controllers/Legacy/BadEnrollmentsController.cs) for baseline grading.
2. **`feat(dtos): create enrollment and payment request/response dtos`**
   - Implements [`CreateEnrollmentRequestDto.cs`](TrainingCenter.Api/DTOs/Enrollments/EnrollmentDtos.cs), [`ProcessPaymentRequestDto.cs`](TrainingCenter.Api/DTOs/Payments/PaymentDtos.cs), and [`ApiResponse.cs`](TrainingCenter.Api/DTOs/Common/ApiResponse.cs).
3. **`feat(services): introduce layered service abstraction for enrollments`**
   - Implements [`IEnrollmentService`](TrainingCenter.Api/Services/Interfaces/ServiceInterfaces.cs) and [`EnrollmentService`](TrainingCenter.Api/Services/Implementations/EnrollmentService.cs).
4. **`feat(validation): add duplicate enrollment guard and track capacity validation`**
   - Adds pre-condition guards and custom exception mappings.
5. **`refactor(payments): enforce positive amount bounds and overpayment checks`**
   - Enforces remaining balance calculations and audit references.
6. **`feat(soft-delete): replace hard deletes with EF Core global query filters`**
   - Configures `HasQueryFilter` and soft-delete interceptor in [`TrainingCenterDbContext.cs`](TrainingCenter.Api/Data/TrainingCenterDbContext.cs).
7. **`perf(queries): introduce projection and server-side pagination to list endpoints`**
   - Adds `.Select()` projections and `PagedResult<T>` handling.
8. **`docs(evidence): add comprehensive problems/improvements analysis and tests`**
   - Adds 10 unit/integration tests and complete verification documentation.

---

## 7. Automated Test Verification Results

All 10 unit and integration tests executed in `TrainingCenter.Tests`:

```
Passed! - Failed: 0, Passed: 10, Skipped: 0, Total: 10
Duration: ~3 seconds
```

| Test Name | Validated Behavior | Status |
| :--- | :--- | :---: |
| `GetAllPagedAsync_ShouldReturnProjectedDtos_WithoutCircularReferences` | Validates DTO projection and pagination | ✅ PASS |
| `CreateAsync_ShouldEnforceDuplicateActiveEnrollmentGuard_ThrowsConflictException` | Prevents duplicate student registrations | ✅ PASS |
| `CreateAsync_ShouldEnforceTrackCapacityLimit_ThrowsBusinessRuleException` | Rejects registrations when capacity is exceeded | ✅ PASS |
| `CreateAsync_ShouldRejectInactiveStudent_ThrowsBusinessRuleException` | Rejects inactive students | ✅ PASS |
| `CreateAsync_ShouldRejectNonActiveTrack_ThrowsBusinessRuleException` | Rejects cancelled/completed tracks | ✅ PASS |
| `CreateAsync_ShouldCreateEnrollment_WhenValid` | Successfully registers valid enrollment | ✅ PASS |
| `ProcessPaymentAsync_ShouldValidatePositiveAmount_ThrowsValidationException` | Rejects negative or zero amounts | ✅ PASS |
| `ProcessPaymentAsync_ShouldPreventOverpayment_ThrowsBusinessRuleException` | Blocks payments exceeding outstanding balance | ✅ PASS |
| `ProcessPaymentAsync_ShouldProcessValidPayment_AndCalculateRemainingBalance` | Computes accurate remaining balance | ✅ PASS |
| `SoftDeleteAsync_ShouldMarkIsDeleted_AndPreserveAuditTrail` | Verifies soft delete flag and query exclusion | ✅ PASS |

---

## 8. Artifacts Index

- **Preserved Bad Code**: [`TrainingCenter.Api/Controllers/Legacy/BadEnrollmentsController.cs`](TrainingCenter.Api/Controllers/Legacy/BadEnrollmentsController.cs)
- **Refactored Controller**: [`TrainingCenter.Api/Controllers/EnrollmentsController.cs`](TrainingCenter.Api/Controllers/EnrollmentsController.cs)
- **Service Implementation**: [`TrainingCenter.Api/Services/Implementations/EnrollmentService.cs`](TrainingCenter.Api/Services/Implementations/EnrollmentService.cs)
- **DTOs**: [`TrainingCenter.Api/DTOs/Enrollments/EnrollmentDtos.cs`](TrainingCenter.Api/DTOs/Enrollments/EnrollmentDtos.cs)
- **Automated Tests**: [`TrainingCenter.Tests/EnrollmentRefactorTests.cs`](../TrainingCenter.Tests/EnrollmentRefactorTests.cs)
- **Postman Collection**: [`postman/TechMaster_Task07_Refactor_Pack.postman_collection.json`](postman/TechMaster_Task07_Refactor_Pack.postman_collection.json)
