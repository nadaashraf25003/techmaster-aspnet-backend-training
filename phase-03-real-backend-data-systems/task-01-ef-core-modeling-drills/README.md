# Task 01: EF Core Modeling Drill Pack (10 Production Drills)

> **Phase 03: Real Backend Data Systems**  
> **Topic:** Relational Data Modeling, EF Core Fluent API, Relationships, Projections, Soft Delete, and Auditing.

---

## 🎯 Executive Summary & Architecture Overview

The **Task 01 Drill Pack** builds the foundational Entity Framework Core data architecture for the TechMaster Academy Training Center system. It rigorously covers all relational database patterns (1:1, 1:N, M:N via join entities), query optimization (projections, server-side pagination), lifecycle tracking (soft delete, UTC audit interceptors), and encapsulated domain models (`IReadOnlyCollection<T>`).

### 📦 Folder Structure
```text
task-01-ef-core-modeling-drills/
├── README.md
├── EFCoreModelingDrills.csproj
├── Program.cs
├── Common/
│   ├── Enums.cs
│   ├── BaseAuditableEntity.cs
│   └── PaginationResult.cs
├── Data/
│   └── DrillsDbContext.cs
├── Drill01_DbContextFirstMigration/
│   ├── Student.cs
│   └── Drill01Runner.cs
├── Drill02_OneToOneStudentProfile/
│   ├── StudentProfile.cs
│   └── Drill02Runner.cs
├── Drill03_OneToManyInstructorTracks/
│   ├── Instructor.cs
│   ├── TrainingTrack.cs
│   └── Drill03Runner.cs
├── Drill04_ManyToManyEnrollment/
│   ├── Enrollment.cs
│   └── Drill04Runner.cs
├── Drill05_PaymentSummary/
│   ├── PaymentSummary.cs
│   └── Drill05Runner.cs
├── Drill06_SeedData/
│   ├── ModelBuilderExtensions.cs
│   └── Drill06Runner.cs
├── Drill07_SoftDelete/
│   └── Drill07Runner.cs
├── Drill08_AuditFields/
│   └── Drill08Runner.cs
├── Drill09_ProjectionDTO/
│   ├── DTOs.cs
│   └── Drill09Runner.cs
└── Drill10_Pagination/
    └── Drill10Runner.cs
```

---

## 🛠️ Drill-by-Drill Technical Specifications

### Drill 01: DbContext & First Migration Mapping
* **Concept:** `DbContext`, `DbSet<T>`, Fluent API configurations, and table generation.
* **Scenario:** Map the foundational `Student` entity (`Id`, `FullName`, `Email`, `PhoneNumber`, `IsActive`, `CreatedAt`).
* **Technical Precision:** Unique index on `Email`, UTC default timestamps, and collection encapsulation.
* **Schema Blueprint:**
  ```csharp
  modelBuilder.Entity<Student>(entity =>
  {
      entity.HasKey(s => s.Id);
      entity.Property(s => s.FullName).IsRequired().HasMaxLength(150);
      entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
      entity.HasIndex(s => s.Email).IsUnique();
  });
  ```

---

### Drill 02: One-to-One Relationship (`Student` ↔ `StudentProfile`)
* **Concept:** Principal vs. Dependent entity, unique Foreign Key constraint, cascade deletion.
* **Scenario:** A student has exactly one official profile (`NationalId`, `Address`, `EmergencyPhone`, `DateOfBirth`).
* **Fluent API Blueprint:**
  ```csharp
  modelBuilder.Entity<StudentProfile>(entity =>
  {
      entity.HasKey(p => p.Id);
      entity.HasOne(p => p.Student)
            .WithOne(s => s.Profile)
            .HasForeignKey<StudentProfile>(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
      entity.HasIndex(p => p.StudentId).IsUnique();
  });
  ```

---

### Drill 03: One-to-Many Relationship (`Instructor` → `TrainingTracks`)
* **Concept:** Principal `Instructor` with multiple dependent `TrainingTrack` entities.
* **Scenario:** An instructor can teach multiple tracks; each track requires one assigned instructor.
* **Encapsulation Standard:** `Instructor.Tracks` is exposed as `IReadOnlyCollection<TrainingTrack>` to protect internal list mutations.
* **Fluent API Blueprint:**
  ```csharp
  modelBuilder.Entity<TrainingTrack>(entity =>
  {
      entity.HasOne(t => t.Instructor)
            .WithMany(i => i.Tracks)
            .HasForeignKey(t => t.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
  });
  ```

---

### Drill 04: Many-to-Many via Join Entity (`Student` + `Track` via `Enrollment`)
* **Concept:** Explicit join entity carrying rich business payload rather than implicit EF Core many-to-many.
* **Scenario:** Track student registration with `EnrollmentDate`, `Status` (Pending/Active/Completed/Cancelled), `ProgressPercentage`, and `FinalGrade`.
* **Fluent API Blueprint:**
  ```csharp
  modelBuilder.Entity<Enrollment>(entity =>
  {
      entity.HasKey(e => e.Id);
      entity.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

      entity.HasOne(e => e.TrainingTrack)
            .WithMany(t => t.Enrollments)
            .HasForeignKey(e => e.TrainingTrackId)
            .OnDelete(DeleteBehavior.Restrict);
  });
  ```

---

### Drill 05: One-to-One Payment Summary & Financial Precision
* **Concept:** Financial aggregation entity with explicit decimal column precision (`decimal(18,2)`).
* **Scenario:** An enrollment has one `PaymentSummary` tracking `TotalRequired`, `TotalPaid`, `RemainingAmount`, and `PaymentStatus`.
* **Fluent API Blueprint:**
  ```csharp
  modelBuilder.Entity<PaymentSummary>(entity =>
  {
      entity.Property(ps => ps.TotalRequired).HasPrecision(18, 2);
      entity.Property(ps => ps.TotalPaid).HasPrecision(18, 2);
      entity.HasOne(ps => ps.Enrollment)
            .WithOne(e => e.PaymentSummary)
            .HasForeignKey<PaymentSummary>(ps => ps.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
      entity.HasIndex(ps => ps.EnrollmentId).IsUnique();
  });
  ```

---

### Drill 06: Deterministic Seed Data (`HasData`)
* **Concept:** Repeatable seed data for development testing and automated verification.
* **Seeded Entities:**
  * **2 Instructors:** Eng. Ahmed Hassan, Eng. Sara Mahmoud.
  * **3 Tracks:** ASP.NET Core Backend Mastery, Database Design & SQL Performance, C# OOP Fundamentals.
  * **5 Students:** Nada Ashraf, Mohamed Ayman, Youssef Ibrahim, Mariam Ali, Omar Khaled.
  * **5 Enrollments & Payment Summaries:** Full realistic scenario coverage.

---

### Drill 07: Soft Delete & Global Query Filters
* **Concept:** Non-destructive deletion using `IsDeleted` and `DeletedAt` with `HasQueryFilter`.
* **Behavior:**
  * Standard queries (`context.Students`) transparently omit deleted records.
  * Administrative queries can bypass using `.IgnoreQueryFilters()`.
* **Implementation:**
  ```csharp
  modelBuilder.Entity<Student>().HasQueryFilter(s => !s.IsDeleted);
  ```

---

### Drill 08: Automated UTC Audit Tracking
* **Concept:** Centralized audit tracking without manual property assignments.
* **Mechanism:** Overriding `SaveChangesAsync` in `DrillsDbContext` to intercept `EntityState.Added` and `EntityState.Modified`, setting `CreatedAt` and `UpdatedAt` in UTC.

---

### Drill 09: LINQ Projection DTOs (`.Select`)
* **Concept:** Query shaping directly in SQL to prevent entity leakage, circular references, and N+1 performance bottlenecks.
* **Projections:**
  * `StudentListItemDto`: ID, Name, Email, Phone, Active Status, and Active Enrollments Count.
  * `TrackDetailsDto`: Code, Title, Level, Capacity, Enrolled Count, Available Seats, Price, and Instructor details.

---

### Drill 10: Server-Side Pagination
* **Concept:** Database-level pagination using `CountAsync()`, `Skip()`, and `Take()`.
* **Metadata Calculations:**
  $$\text{TotalPages} = \lceil \frac{\text{TotalCount}}{\text{PageSize}} \rceil$$
* **Validation:** Rejects `pageNumber < 1` and `pageSize > 50` via `ArgumentOutOfRangeException`.

---

## 🧪 Verification & Test Results

```text
================================================================================
🚀 TechMaster Academy - Phase 03: Task 01 - EF Core Modeling Drills (10 Drills)
================================================================================
✅ Drill 01: DbContext, DbSet & First Migration Mapping passed.
✅ Drill 02: 1:1 Student <-> Profile verified with unique FK.
✅ Drill 03: 1:N Instructor -> Tracks with IReadOnlyCollection verified.
✅ Drill 04: Rich M:N Enrollment join entity verified.
✅ Drill 05: 1:1 PaymentSummary with decimal(18,2) verified.
✅ Drill 06: Deterministic Seed Data verified (5 students, 2 instructors, 3 tracks, 5 enrollments).
✅ Drill 07: Soft Delete & Global Query Filter verified.
✅ Drill 08: SaveChangesAsync UTC Audit Tracking verified.
✅ Drill 09: LINQ Projection DTOs (.Select) verified.
✅ Drill 10: Server-Side Pagination & math validation verified.
================================================================================
🎉 ALL 10 EF CORE MODELING DRILLS COMPLETED SUCCESSFULLY (100% PASS RATE)
================================================================================
```

---

## 🏃 How to Run the Drills Locally

```bash
cd phase-03-real-backend-data-systems/task-01-ef-core-modeling-drills
dotnet run
```
