# Task 00: Workspace & Environment Setup

## 📋 Overview
Task 00 establishes the technical foundations and development workspace for **Phase 03: Real Backend Data Systems**. It prepares the environment for Entity Framework Core (EF Core), Microsoft SQL Server database connectivity, safe configuration practices, and project structure alignment according to production standards.

---

## 🏗️ Architecture & Project Structure

```text
phase-03-real-backend-data-systems/
├── README.md
├── task-00-workspace-environment-setup/
│   ├── README.md
│   └── TrainingCenter.SetupApi/
│       ├── Controllers/
│       │   └── SetupHealthController.cs
│       ├── Data/
│       │   └── AppDbContext.cs
│       ├── Entities/
│       │   └── Student.cs
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Program.cs
│       └── TrainingCenter.SetupApi.csproj
```

---

## 📦 Required Packages & Tooling

The project incorporates the official EF Core and OpenAPI packages:
* `Microsoft.EntityFrameworkCore.SqlServer` (v10.0+) — SQL Server Database Provider.
* `Microsoft.EntityFrameworkCore.Design` (v10.0+) — Design-time logic for migrations.
* `Microsoft.EntityFrameworkCore.Tools` (v10.0+) — Package Manager Console / CLI migration tooling.
* `Swashbuckle.AspNetCore` — Interactive Swagger documentation.

### Global CLI Tool Verification
To ensure `dotnet-ef` is globally accessible:
```bash
# Check existing version
dotnet ef --version

# Install or update if missing
dotnet tool install --global dotnet-ef
# or update to latest:
dotnet tool update --global dotnet-ef
```

---

## 🔒 Safe Configuration & Secrets Management

### 1. Local Development (`appsettings.Development.json`)
Local development uses a safe LocalDB or local SQL Server instance without sensitive credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TechMasterTrainingCenter_DevDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 2. Production Security Rules
* ⚠️ **Never commit production passwords or remote connection strings to Git.**
* Set remote credentials via **Environment Variables** or the hosting provider control panel (e.g., MonsterASP.NET Application Settings / Azure Configuration Settings).
* In production, the connection string is injected at runtime using:
  ```bash
  ConnectionStrings__DefaultConnection="Server=remote-server;Database=RemoteDb;User Id=user;Password=secret;"
  ```

---

## ⚙️ Core EF Core CLI Commands Reference

| Operation | CLI Command |
| :--- | :--- |
| **Add Initial Migration** | `dotnet ef migrations add InitialTrainingCenterSchema` |
| **Apply Migrations to DB** | `dotnet ef database update` |
| **Remove Last Migration** | `dotnet ef migrations remove` |
| **Generate SQL Script** | `dotnet ef migrations script -o migrations_script.sql` |
| **List All Migrations** | `dotnet ef migrations list` |

---

## 🚀 Verification & Health Endpoints

The setup API provides verification endpoints:
1. `GET /api/setuphealth/status` — Checks runtime status, environment, and configured EF Core provider.
2. `GET /api/setuphealth/db-check` — Safely verifies SQL Server connectivity and reports applied vs. pending migrations without exposing secrets.

---

## ✅ Task 00 Acceptance Criteria Checklist
- [x] Phase 03 directory structure organized and created.
- [x] ASP.NET Core Web API starter project created (`TrainingCenter.SetupApi`).
- [x] EF Core SQL Server, Design, and Tools packages installed.
- [x] `AppDbContext` and initial `Student` entity configured with Fluent API.
- [x] Safe connection string configured for development.
- [x] Health check endpoints available to test DB connectivity.
- [x] Zero sensitive secrets committed to repository.
