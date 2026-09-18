# Task 04: Querying, Filtering, and Reporting

> **Phase 03: Real Backend Data Systems • TechMaster Academy**  
> **Topic:** Advanced EF Core Querying, Dynamic Conditional Filtering, Aggregations, Projections, Server-Side Pagination, REST Reporting Endpoints, and SQL Execution Mechanics.

---

## 🎯 Executive Summary & Mission

Task 04 establishes the enterprise querying, reporting, and analytics layer for **TechMaster Academy**. It implements high-performance relational database queries in ASP.NET Core with Entity Framework Core (EF Core 8) and Microsoft SQL Server.

The query engine is engineered with strict production standards:
1. **Deferred Execution & Dynamic Composition:** All query filters are composed conditionally on `IQueryable<T>` and translated to optimized SQL in a single database round-trip.
2. **Strict Financial Precision:** All monetary amounts (`Revenue`, `Price`, `Amount`, `OutstandingBalance`) use `decimal(18,2)` to eliminate floating-point rounding inaccuracies.
3. **No Over-Fetching:** Data is projected directly to strongly-typed DTOs via `.Select()`, avoiding the N+1 query problem and preventing internal EF Core entity leakage.
4. **Resilient Validation:** Strict validation rules (e.g. `from <= to` on date ranges, enum validation, capacity protection) return uniform `ApiResponse<T>` envelopes.
5. **Swagger & Postman Ready:** Interactive OpenAPI/Swagger documentation at application root (`/`) with a comprehensive automated Postman test collection.

---

## 📂 Repository & Project Structure

```text
task-04-querying-filtering-reporting/
├── README.md
├── queries/
│   ├── Query13_PaymentsByDateRange.sql
│   ├── Query14_RevenueSummary.sql
│   ├── Query15_RevenuePerTrack.sql
│   ├── Query16_TopTracksByEnrollment.sql
│   ├── Query17_InstructorWorkload.sql
│   ├── Query18_StudentsWithoutPayments.sql
│   ├── Query19_AdvancedEnrollmentFilter.sql
│   ├── Query20_DashboardSummary.sql
│   ├── Query_Supporting_TrackCapacity.sql
│   └── Query_Supporting_UnpaidEnrollments.sql
├── TrainingCenter.Api/
│   ├── Controllers/
│   │   ├── BaseApiController.cs
│   │   ├── PaymentsController.cs          # Query 13 (Date Range + Validation)
│   │   ├── ReportsController.cs           # Queries 14, 15, 16, 17, 18, 20 + Supporting Reports
│   │   ├── EnrollmentsController.cs       # Query 19 (Advanced Conditional Filtering)
│   │   ├── StudentsController.cs
│   │   ├── InstructorsController.cs
│   │   └── TracksController.cs
│   ├── Data/
│   │   ├── TrainingCenterDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── StudentConfiguration.cs
│   │   │   ├── InstructorConfiguration.cs
│   │   │   ├── TrainingTrackConfiguration.cs
│   │   │   ├── EnrollmentConfiguration.cs
│   │   │   └── PaymentConfiguration.cs
│   │   └── DbInitializer.cs               # Rich seed data verifying Queries 13-20
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
│   │   ├── Payments/
│   │   │   └── PaymentDtos.cs             # Query 13 Parameters & DTOs
│   │   ├── Enrollments/
│   │   │   └── EnrollmentDtos.cs          # Query 19 Dynamic Parameters & DTOs
│   │   ├── Reports/
│   │   │   └── ReportDtos.cs              # Queries 14, 15, 16, 17, 18, 20 DTOs
│   │   ├── Students/
│   │   ├── Instructors/
│   │   └── Tracks/
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   └── ServiceInterfaces.cs
│   │   └── Implementations/
│   │       ├── ReportService.cs           # Core BI Reporting logic
│   │       ├── PaymentService.cs          # Date range validation & payment queries
│   │       ├── EnrollmentService.cs       # Dynamic IQueryable composition
│   │       ├── StudentService.cs
│   │       ├── InstructorService.cs
│   │       └── TrackService.cs
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
│   ├── TechMaster_Query_Pack_API.postman_collection.json
│   └── TechMaster_Query_Pack_API.postman_environment.json
└── evidence/
    └── README.md
```

---

## 📡 Query Specification Matrix (Queries 13 – 20)

| Query ID | Title | Route & Method | Key EF Core Concept | Expected Behavior & Validation |
| :--- | :--- | :--- | :--- | :--- |
| **Query 13** | **Payments By Date Range** | `GET /api/payments?from=2026-07-01&to=2026-07-31` | `.Where(p => p.PaymentDate >= from && p.PaymentDate <= to)` | Validates `from <= to` (returns 400 Bad Request if invalid); server-side pagination. |
| **Query 14** | **Revenue Summary** | `GET /api/reports/revenue-summary` | `SumAsync` + `CountAsync` with decimal money | Returns total revenue, realized cash, expected revenue, paid count, pending count, failed count. |
| **Query 15** | **Revenue Per Track** | `GET /api/reports/revenue-by-track` | `.GroupBy()` / navigation collection aggregates | Groups completed payments by track; returns Track Title, Unit Price, Total Paid, Total Enrolled. |
| **Query 16** | **Top Tracks By Enrollment** | `GET /api/reports/top-tracks?count=5` | `.OrderByDescending(activeCount).Take(5)` | Defaults to top 5 tracks; computes active enrollments, capacity, available seats, utilization percentage. |
| **Query 17** | **Instructor Workload** | `GET /api/reports/instructor-workload` | Navigation aggregation across Tracks & Students | Returns Instructor name, specialization, active tracks count, supervised active students, revenue generated. |
| **Query 18** | **Students Without Payments** | `GET /api/reports/students-without-payments` | `.Any()` / `.All()` condition on related payments | Returns active/pending enrolled students with 0 completed payments; **strictly excludes cancelled enrollments**. |
| **Query 19** | **Advanced Enrollment Filter** | `GET /api/enrollments?trackId=1&status=Active&paymentStatus=Paid` | Conditional `IQueryable` composition | Dynamically applies `.Where()` predicates only when parameters have values; supports multi-filter & pagination. |
| **Query 20** | **Dashboard Summary** | `GET /api/reports/dashboard-summary` | Multiple aggregate queries | Consolidated metrics: `studentsCount`, `tracksCount`, `activeEnrollments`, `revenue`, `unpaidCount`. |

---

## 🔬 Deep Dive: 5 Selected In-Depth Query Explanations

### 1. Query 13: Payments By Date Range
* **Scenario:** Retrieve all payments processed within a date interval with validation that `from <= to`.
* **C# LINQ Implementation ([`PaymentService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-04-querying-filtering-reporting/TrainingCenter.Api/Services/Implementations/PaymentService.cs)):**
```csharp
if (filters.From.HasValue && filters.To.HasValue && filters.From.Value > filters.To.Value)
{
    throw new BadRequestException("Invalid date range: 'from' date must be less than or equal to 'to' date.");
}

var query = _context.Payments.AsNoTracking().AsQueryable();

if (filters.From.HasValue)
    query = query.Where(p => p.PaymentDate >= filters.From.Value.Date);

if (filters.To.HasValue)
{
    var toDate = filters.To.Value.Date.AddDays(1).AddTicks(-1);
    query = query.Where(p => p.PaymentDate <= toDate);
}

var items = await query
    .OrderByDescending(p => p.PaymentDate)
    .Skip((filters.PageNumber - 1) * filters.PageSize)
    .Take(filters.PageSize)
    .Select(p => new PaymentResponse { ... })
    .ToListAsync();
```
* **Generated T-SQL:**
```sql
SELECT p.[PaymentId], p.[EnrollmentId], s.[FullName] AS [StudentName], t.[Title] AS [TrackTitle], 
       p.[Amount], p.[PaymentMethod], p.[PaymentDate], p.[PaymentStatus], p.[ReferenceNumber], p.[Notes], p.[CreatedAt]
FROM [Payments] AS p
INNER JOIN [Enrollments] AS e ON p.[EnrollmentId] = e.[EnrollmentId] AND e.[IsDeleted] = 0
INNER JOIN [Students] AS s ON e.[StudentId] = s.[StudentId] AND s.[IsDeleted] = 0
INNER JOIN [TrainingTracks] AS t ON e.[TrainingTrackId] = t.[TrainingTrackId] AND t.[IsDeleted] = 0
WHERE p.[IsDeleted] = 0 
  AND p.[PaymentDate] >= @p0 
  AND p.[PaymentDate] <= @p1
ORDER BY p.[PaymentDate] DESC
OFFSET @p2 ROWS FETCH NEXT @p3 ROWS ONLY;
```
* **Performance Note:** Indexed with `CREATE NONCLUSTERED INDEX IX_Payments_PaymentDate ON Payments(PaymentDate) INCLUDE (Amount, PaymentStatus, EnrollmentId);` allowing Index Seek operations with zero table scans.

---

### 2. Query 14: Revenue Summary
* **Scenario:** Aggregate financial metrics including total revenue, paid count, pending count, and failed count.
* **C# LINQ Implementation ([`ReportService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-04-querying-filtering-reporting/TrainingCenter.Api/Services/Implementations/ReportService.cs)):**
```csharp
var realizedRevenue = await _context.Payments
    .Where(p => p.PaymentStatus == PaymentStatus.Completed)
    .SumAsync(p => (decimal?)p.Amount) ?? 0.00m;

var expectedRevenue = await _context.Enrollments
    .Where(e => e.Status != EnrollmentStatus.Cancelled)
    .Include(e => e.TrainingTrack)
    .SumAsync(e => (decimal?)(e.TrainingTrack != null ? e.TrainingTrack.Price : 0m)) ?? 0.00m;

var paidCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Completed);
var pendingCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Pending);
var failedCount = await _context.Payments.CountAsync(p => p.PaymentStatus == PaymentStatus.Failed);
```
* **Generated T-SQL:**
```sql
SELECT COALESCE(SUM(p.[Amount]), 0.0) 
FROM [Payments] AS p 
WHERE p.[IsDeleted] = 0 AND p.[PaymentStatus] = 'Completed';
```
* **Financial Safety:** Explicit cast to nullable `(decimal?)` ensures that if no rows match, SQL Server returns `NULL` and EF Core maps it to `0.00m` rather than throwing an `InvalidOperationException`.

---

### 3. Query 15: Revenue Per Track
* **Scenario:** Group paid payments by training track and compute realized revenue vs expected revenue.
* **C# LINQ Implementation ([`ReportService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-04-querying-filtering-reporting/TrainingCenter.Api/Services/Implementations/ReportService.cs)):**
```csharp
var report = await _context.TrainingTracks
    .AsNoTracking()
    .Select(t => new RevenueByTrackReportDto
    {
        TrackId = t.TrainingTrackId,
        TrackCode = t.Code,
        TrackTitle = t.Title,
        UnitPrice = t.Price,
        EnrollmentCount = t.Enrollments.Count(e => e.Status != EnrollmentStatus.Cancelled),
        TotalPaid = t.Enrollments
            .SelectMany(e => e.Payments)
            .Where(p => p.PaymentStatus == PaymentStatus.Completed)
            .Sum(p => (decimal?)p.Amount) ?? 0.00m,
        ExpectedRevenue = t.Enrollments
            .Where(e => e.Status != EnrollmentStatus.Cancelled)
            .Sum(e => (decimal?)t.Price) ?? 0.00m
    })
    .OrderByDescending(r => r.TotalPaid)
    .ToListAsync();
```
* **Generated T-SQL:**
```sql
SELECT t.[TrainingTrackId] AS [TrackId], t.[Code] AS [TrackCode], t.[Title] AS [TrackTitle], t.[Price] AS [UnitPrice],
       (SELECT COUNT(*) FROM [Enrollments] AS e WHERE t.[TrainingTrackId] = e.[TrainingTrackId] AND e.[IsDeleted] = 0 AND e.[Status] <> 'Cancelled') AS [EnrollmentCount],
       COALESCE((SELECT SUM(p.[Amount]) 
                 FROM [Enrollments] AS e0 
                 INNER JOIN [Payments] AS p ON e0.[EnrollmentId] = p.[EnrollmentId] 
                 WHERE t.[TrainingTrackId] = e0.[TrainingTrackId] AND p.[PaymentStatus] = 'Completed' AND p.[IsDeleted] = 0), 0.0) AS [TotalPaid]
FROM [TrainingTracks] AS t
WHERE t.[IsDeleted] = 0
ORDER BY [TotalPaid] DESC;
```

---

### 4. Query 18: Students Without Payments
* **Scenario:** Identify students with active/pending enrollments who have never completed a payment (strictly excluding cancelled enrollments).
* **C# LINQ Implementation ([`ReportService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-04-querying-filtering-reporting/TrainingCenter.Api/Services/Implementations/ReportService.cs)):**
```csharp
var report = await _context.Enrollments
    .AsNoTracking()
    .Where(e => (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Pending)
                && !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed))
    .Include(e => e.Student)
    .Include(e => e.TrainingTrack)
    .Select(e => new StudentWithoutPaymentReportDto
    {
        StudentId = e.StudentId,
        StudentName = e.Student != null ? e.Student.FullName : string.Empty,
        Email = e.Student != null ? e.Student.Email : string.Empty,
        PhoneNumber = e.Student != null ? e.Student.PhoneNumber : null,
        EnrollmentId = e.EnrollmentId,
        EnrollmentStatus = e.Status.ToString(),
        EnrollmentDate = e.EnrollmentDate,
        TrackId = e.TrainingTrackId,
        TrackCode = e.TrainingTrack != null ? e.TrainingTrack.Code : string.Empty,
        TrackTitle = e.TrainingTrack != null ? e.TrainingTrack.Title : string.Empty,
        TrackPrice = e.TrainingTrack != null ? e.TrainingTrack.Price : 0.00m,
        TotalPaid = 0.00m
    })
    .OrderBy(s => s.StudentName)
    .ToListAsync();
```
* **Generated T-SQL:**
```sql
SELECT e.[StudentId], s.[FullName] AS [StudentName], s.[Email], s.[PhoneNumber], 
       e.[EnrollmentId], e.[Status] AS [EnrollmentStatus], e.[EnrollmentDate], 
       e.[TrainingTrackId] AS [TrackId], t.[Code] AS [TrackCode], t.[Title] AS [TrackTitle], t.[Price] AS [TrackPrice]
FROM [Enrollments] AS e
INNER JOIN [Students] AS s ON e.[StudentId] = s.[StudentId] AND s.[IsDeleted] = 0
INNER JOIN [TrainingTracks] AS t ON e.[TrainingTrackId] = t.[TrainingTrackId] AND t.[IsDeleted] = 0
WHERE e.[IsDeleted] = 0 
  AND e.[Status] IN ('Active', 'Pending')
  AND NOT EXISTS (
      SELECT 1 
      FROM [Payments] AS p 
      WHERE e.[EnrollmentId] = p.[EnrollmentId] AND p.[PaymentStatus] = 'Completed' AND p.[IsDeleted] = 0
  )
ORDER BY s.[FullName];
```

---

### 5. Query 19: Advanced Enrollment Filter
* **Scenario:** Dynamically combine filters (`trackId`, `status`, `paymentStatus`, `studentId`, `date`, `search`) using conditional `IQueryable` composition without building brittle SQL strings.
* **C# LINQ Implementation ([`EnrollmentService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-03-real-backend-data-systems/task-04-querying-filtering-reporting/TrainingCenter.Api/Services/Implementations/EnrollmentService.cs)):**
```csharp
var query = _context.Enrollments.AsNoTracking().AsQueryable();

if (filters.TrackId.HasValue)
    query = query.Where(e => e.TrainingTrackId == filters.TrackId.Value);

if (filters.Status.HasValue)
    query = query.Where(e => e.Status == filters.Status.Value);

if (!string.IsNullOrWhiteSpace(filters.PaymentStatus))
{
    var normalized = filters.PaymentStatus.Trim().ToLowerInvariant();
    if (normalized is "paid" or "completed")
        query = query.Where(e => e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed));
    else if (normalized == "unpaid")
        query = query.Where(e => !e.Payments.Any(p => p.PaymentStatus == PaymentStatus.Completed));
}

var totalCount = await query.CountAsync();
var items = await query.OrderByDescending(e => e.EnrollmentDate)
    .Skip((filters.PageNumber - 1) * filters.PageSize)
    .Take(filters.PageSize)
    .Select(e => new EnrollmentListItemResponse { ... })
    .ToListAsync();
```

---

## 🛠️ Verification & How to Run

### 1. Build and Run API
```powershell
dotnet run --project phase-03-real-backend-data-systems/task-04-querying-filtering-reporting/TrainingCenter.Api/TrainingCenter.Api.csproj
```

### 2. Access Swagger UI
Open your browser at:
```text
http://localhost:5000/
```

### 3. Run Postman Test Suite
Import `TechMaster_Query_Pack_API.postman_collection.json` and `TechMaster_Query_Pack_API.postman_environment.json` into Postman and execute the full collection runner.

---

## 📊 Evaluation & Definition of Done Checklist

- [x] **Query 13:** Date range filtering with strict `from <= to` validation returning 400 Bad Request on invalid inputs.
- [x] **Query 14:** Revenue summary with decimal money values, paid count, pending count, and failed count.
- [x] **Query 15:** Revenue per track with grouped totals and collection percentages.
- [x] **Query 16:** Top tracks by enrollment with configurable count (default 5) and utilization metrics.
- [x] **Query 17:** Instructor workload reporting active tracks and supervised active students.
- [x] **Query 18:** Students without payments with strict exclusion of cancelled enrollments.
- [x] **Query 19:** Advanced enrollment dynamic filter with conditional `IQueryable` composition.
- [x] **Query 20:** Dashboard summary aggregating high-level system KPIs into one payload.
- [x] **Swagger / OpenAPI:** Interactive documentation at root `/` with XML doc summaries.
- [x] **Postman Collection & Environment:** Automated test suite in `postman/`.
- [x] **SQL Scripts:** Individual modular T-SQL query scripts in `queries/`.
- [x] **Clean Architecture & DTOs:** Strict separation between database entities and API response envelopes.
