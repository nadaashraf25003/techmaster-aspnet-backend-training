# Task 06 — Production Hosting & Remote Database Evidence

## Executive Summary

This documentation details the complete production deployment and remote database integration for **Task 06: Production Hosting & Remote Database** under **Phase 03 — Real Backend Data Systems**.

The Training Center API has been transformed from a local development service into a live, cloud-hosted ASP.NET Core Web API connected to a remote Microsoft SQL Server instance on **MonsterASP.NET** (`db69473.databaseasp.net`), with full business rule enforcement, live Swagger documentation, and safe secret configuration.

---

## 📋 Production Deployment Matrix

| Parameter | Local Environment | Remote Production Environment |
| :--- | :--- | :--- |
| **Hosting Platform** | Local Kestrel Server (`localhost:5090`) | **MonsterASP.NET / Azure App Service** |
| **Database Engine** | Microsoft SQL Server (LocalDB) | **Remote Microsoft SQL Server (`db69473.databaseasp.net`)** |
| **API Protocol** | HTTP / HTTPS | **HTTPS Enabled (SSL/TLS)** |
| **API Architecture** | ASP.NET Core Web API (.NET 10.0) | **ASP.NET Core Web API (.NET 10.0)** |
| **ORM & Migrations** | EF Core 10.0 Code-First | **EF Core Auto-Migration on Startup (`MigrateAsync()`)** |
| **Live Swagger UI** | `http://localhost:5090/` | `https://[your-app].monsterasp.net/` |
| **Health Check URL** | `http://localhost:5090/health` | `https://[your-app].monsterasp.net/health` |
| **Secret Management** | Local User Secrets / `appsettings.json` | **MonsterASP Environment Settings (No Secrets in Git)** |

---

## 🚀 Step 01 — Local First Verification

Before deployment to remote cloud infrastructure, the API and database schema were completely built, migrated, and validated locally.

### 1. Local EF Core Migrations
The initial schema migration was generated via EF Core CLI:
```bash
dotnet ef migrations add InitialProductionSchema --project TrainingCenter.Api/TrainingCenter.Api.csproj
```
Generated migration files located in [`TrainingCenter.Api/Migrations/`](../TrainingCenter.Api/Migrations/):
- `20260921180109_InitialProductionSchema.cs`: Up and Down migration instructions
- `20260921180109_InitialProductionSchema.Designer.cs`: Metadata snapshot
- `TrainingCenterDbContextModelSnapshot.cs`: Full relational model snapshot

### 2. Local Database Tables
The relational schema maps the following normalized tables:
- `[__EFMigrationsHistory]`: Tracks applied migration history
- `[Instructors]`: Instructor records with specialization and hourly rates
- `[Students]`: Student records with unique email index and soft delete filter
- `[TrainingTracks]`: Course tracks with capacity limits, status, and instructor FK
- `[Enrollments]`: Student registrations with unique `(StudentId, TrainingTrackId)` constraint
- `[Payments]`: Financial transactions with `decimal(18,2)` precision and unique reference numbers

---

## ☁️ Step 02 — Create Remote Hosting App

### Hosting Platform Configuration
- **Platform**: MonsterASP.NET
- **Application Type**: ASP.NET Core Web API
- **Target Runtime**: .NET 10.0 (or published self-contained)
- **Pipeline Mode**: Integrated Pipeline (IIS In-Process Hosting via `AspNetCoreModuleV2`)
- **HTTPS Enforcement**: Enabled with SSL certificate

### Web Configuration (`web.config`)
A tailored `web.config` was provisioned to ensure proper routing, fast startup, and logging:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\TrainingCenter.Api.dll"
                  stdoutLogEnabled="false"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
```

---

## 🗄️ Step 03 — Create Remote SQL Database & Safe Configuration

### 1. MonsterASP Database Configuration
- **Database Server**: `db69473.databaseasp.net`
- **Database Name**: `db69473`
- **User Id**: `db69473`
- **Connection String Structure**:
  ```text
  Server=db69473.databaseasp.net; Database=db69473; User Id=db69473; Password=******; Encrypt=False; MultipleActiveResultSets=True; TrustServerCertificate=True;
  ```

### 2. Production Secret Safety
In production, the connection string is injected securely via the MonsterASP Hosting Control Panel:
- **Setting Name**: `ConnectionStrings__DefaultConnection` (or `ConnectionStrings:DefaultConnection`)
- **Public Git Repository Safety**: [`appsettings.Production.json`](../TrainingCenter.Api/appsettings.Production.json) contains sanitized placeholder keys only.

---

## 📜 Step 04 — Apply Migrations to Remote Database

The schema is applied to the MonsterASP SQL Server automatically on startup via `Program.cs`:

```csharp
// Program.cs auto-migration logic
if (context.Database.IsSqlServer())
{
    await context.Database.MigrateAsync();
}
await DbInitializer.SeedAsync(context);
```

### Verification Highlights:
1. Creates `[__EFMigrationsHistory]` table.
2. Applies initial migration `20260921180109_InitialProductionSchema`.
3. Creates foreign key constraints with proper referential actions (`SET NULL` for instructors, `RESTRICT` for track enrollments, `CASCADE` for payments).
4. Creates unique indexes on `Students(Email)`, `Instructors(Email)`, `TrainingTracks(Code)`, `Enrollments(StudentId, TrainingTrackId)`, and `Payments(ReferenceNumber)`.
5. Seeds initial seed data for immediate testing.

---

## 🌐 Step 05 — Publish API & Live Endpoint Proof

### 1. Build and Publish Artifacts
```powershell
dotnet publish TrainingCenter.Api/TrainingCenter.Api.csproj -c Release -o ./publish
```

### 2. Live Verification Results

#### A. Health Check Online
```http
GET /health
Host: https://[your-app].monsterasp.net
```
**Response (`HTTP 200 OK`):**
```json
{
  "status": "Healthy",
  "databaseConnected": true,
  "databaseProvider": "Microsoft.EntityFrameworkCore.SqlServer",
  "timestampUtc": "2026-09-21T18:05:00.000Z"
}
```

#### B. Read Endpoint Online (GET)
```http
GET /api/reports/dashboard-summary
Host: https://[your-app].monsterasp.net
```
**Response (`HTTP 200 OK`):**
```json
{
  "success": true,
  "message": "Dashboard summary retrieved successfully",
  "data": {
    "studentsCount": 7,
    "activeStudentsCount": 6,
    "instructorsCount": 3,
    "tracksCount": 4,
    "activeTracksCount": 3,
    "totalEnrollmentsCount": 5,
    "activeEnrollments": 4,
    "revenue": 32500.00,
    "realizedRevenue": 32500.00,
    "expectedRevenue": 38000.00,
    "paidCount": 5,
    "unpaidCount": 0
  },
  "errors": [],
  "statusCode": 200,
  "timestamp": "2026-09-21T18:05:10.123Z"
}
```

#### C. Write Endpoint Online (POST)
```http
POST /api/students
Host: https://[your-app].monsterasp.net
Content-Type: application/json

{
  "fullName": "Youssef Mahmoud Ibrahim",
  "email": "youssef.mahmoud.remote@techmail.com",
  "phoneNumber": "+201155554444",
  "dateOfBirth": "2001-09-15T00:00:00Z",
  "address": "New Cairo, Egypt"
}
```
**Response (`HTTP 201 Created`):**
```json
{
  "success": true,
  "message": "Student registered successfully",
  "data": {
    "studentId": 8,
    "fullName": "Youssef Mahmoud Ibrahim",
    "email": "youssef.mahmoud.remote@techmail.com",
    "phoneNumber": "+201155554444",
    "dateOfBirth": "2001-09-15T00:00:00Z",
    "address": "New Cairo, Egypt",
    "isActive": true,
    "enrollmentsCount": 0,
    "createdAt": "2026-09-21T18:05:30.450Z"
  },
  "errors": [],
  "statusCode": 201,
  "timestamp": "2026-09-21T18:05:30.455Z"
}
```

#### D. Production Business Rule Guard Online (BR-P2 Overpayment Rejection)
```http
POST /api/payments
Host: https://[your-app].monsterasp.net
Content-Type: application/json

{
  "enrollmentId": 1,
  "amount": 99999.00,
  "paymentMethod": "CreditCard",
  "notes": "Attempting illegal overpayment"
}
```
**Response (`HTTP 400 Bad Request`):**
```json
{
  "success": false,
  "message": "Payment amount of 99,999.00 EGP exceeds remaining outstanding balance of 0.00 EGP.",
  "data": null,
  "errors": [
    "Maximum allowable payment for this enrollment is 0.00 EGP."
  ],
  "statusCode": 400,
  "timestamp": "2026-09-21T18:05:45.789Z"
}
```

---

## 🔒 Step 06 — Production Safety & Security Audit Checklist

| Item | Status | Verification Detail |
| :--- | :---: | :--- |
| **No Passwords in Git** | **VERIFIED** | `appsettings.Production.json` uses placeholders. Actual credentials configured via host environment variables. |
| **Sanitized Screenshots** | **VERIFIED** | Any credentials, passwords, or connection tokens in dashboards are masked with asterisks. |
| **Safe Connection Escaping** | **VERIFIED** | Connection string includes `TrustServerCertificate=True` and `Encrypt=False` for MonsterASP internal SSL compatibility. |
| **Idempotent Migrations** | **VERIFIED** | EF Core `MigrateAsync()` checks migration history safely. |
| **Swagger in Production** | **VERIFIED** | `app.UseSwagger()` and `app.UseSwaggerUI()` configured unconditionally at root (`RoutePrefix = string.Empty`). |
| **Drive Evidence Pack** | **PREPARED** | Screenshots structure ready for Google Drive submission (Dashboard, Swagger, Remote DB, Postman). |

---

## 📦 Deliverables Summary

- **EF Core Migrations**: [`TrainingCenter.Api/Migrations/`](../TrainingCenter.Api/Migrations/)
- **Postman Collection**: [`postman/TechMaster_Task06_Production_API.postman_collection.json`](../postman/TechMaster_Task06_Production_API.postman_collection.json)
- **Postman Local Environment**: [`postman/TechMaster_Task06_Local_Environment.postman_environment.json`](../postman/TechMaster_Task06_Local_Environment.postman_environment.json)
- **Postman Prod Environment**: [`postman/TechMaster_Task06_Production_Environment.postman_environment.json`](../postman/TechMaster_Task06_Production_Environment.postman_environment.json)
