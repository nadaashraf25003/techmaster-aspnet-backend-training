# Phase 03: Real Backend Data Systems • EF Core • SQL Server • Remote Database

> **ASP.NET Core Backend Career Training — TechMaster Academy**  
> **Score Target:** 100/100 (Top Student Performance Standard)  
> **Core Theme:** Production Data-Driven Execution Manual: From Business Requirements to a Live Database-Driven API.

---

## 🎯 Phase 03 Overview & Mission

In Phase 01, we established solid C# OOP foundations, data manipulation, and refactoring practices.  
In Phase 02, we transitioned into ASP.NET Core Web API architecture, DTOs, layered services, Swagger, and RESTful routing standards.  
**Phase 03 transforms the API into a real-world, database-driven backend system powered by Entity Framework Core (EF Core), Microsoft SQL Server, and remote cloud hosting.**

### 🏆 Definition of Done (DoD)
1. **Local API Works with SQL Server:** Full relational schema mapped with EF Core DbContext, migrations, and navigations.
2. **Remote Database Configured:** Cloud/Remote SQL Server instance provisioned without exposing credentials.
3. **Live Swagger URL:** Deployed and accessible online on cloud hosting.
4. **Business Rules Enforced:** Domain validation, capacity protection, financial precision, and soft-delete safeguards.
5. **Production Evidence:** Comprehensive documentation, ERD diagrams, Postman collections, and demo walkthroughs.

---

## 📂 Phase 03 Repository Structure

```text
phase-03-real-backend-data-systems/
├── README.md
├── task-00-workspace-environment-setup/
├── task-01-ef-core-modeling-drills/
├── task-02-requirements-to-erd/
├── task-03-training-center-database-api/
├── task-04-querying-filtering-reporting/
├── task-05-business-rules-data-integrity/
├── task-06-production-hosting-remote-database/
├── task-07-ef-core-api-refactor-pack/
└── task-08-interview-demo-pack/
```

---

## 📊 Evaluation Rubric (100 Points Model)

| Component | Weight | Key Assessment Criteria |
| :--- | :---: | :--- |
| **1. Database Design & ERD** | 15 pts | Entities, PK/FK constraints, relationship integrity, and clear ERD visualization. |
| **2. EF Core Setup & Migrations** | 15 pts | `DbContext`, `DbSet<T>`, clean migration history, SQL Server table generation. |
| **3. API Functionality** | 20 pts | Full CRUD operations, strict DTOs, service layer abstraction, HTTP status codes. |
| **4. Queries & Reports** | 15 pts | Advanced LINQ filtering, server-side pagination, projections (`.Select()`), aggregation. |
| **5. Business Rules & Integrity** | 15 pts | Capacity constraints, duplicate prevention, payment status workflows, soft delete. |
| **6. Production Deployment** | 10 pts | Live deployed Swagger URL, remote SQL database, safe secret handling. |
| **7. Interview & Explanation** | 10 pts | Technical mastery of EF Core mechanics, trade-offs, and architecture decisions. |

---

## 📅 10-Day Sprint Execution Plan

* **Days 1–2 (Tasks 00 & 01):** Workspace environment setup, EF Core SQL Server configuration, 10 modeling drills (1:1, 1:N, M:N, Soft Delete, Audit, Projections, Pagination).
* **Days 3–5 (Tasks 02 & 03):** Business requirements translation into ERD, Training Center API core entities, migrations, and layered endpoints.
* **Days 6–7 (Tasks 04, 05 & 06):** Complex querying and reports (20 query specs), domain business rules enforcement, and remote database hosting setup.
* **Days 8–9 (Tasks 07 & 08):** EF Core legacy code refactoring pack, Postman test suites, Swagger evidence, and interview answers.
* **Day 10:** Review, validation, mentor evaluation, and Phase 04 preparation.

---

## 🛡️ Production Mindset & Standards

1. **No Hardcoded Secrets:** Connection strings and passwords must never be committed to Git.
2. **DTOs Everywhere:** Never leak EF entities directly in API endpoints.
3. **Encapsulated Collections:** Expose navigation collections as `IReadOnlyCollection<T>` / `IReadOnlyList<T>`.
4. **Exact API Contracts:** Adhere strictly to the requested route shapes, HTTP methods, and status codes.
5. **Decimal for Currency:** Always use `decimal(18,2)` for financial calculations and audit tracking in UTC.
