# Phase 01: .NET Backend Foundations

Welcome to the documentation for **Phase 01 - Backend Foundations** of the ASP.NET Backend Career Training program at TechMaster Academy. 

This phase focuses on C# programming language mastery, Object-Oriented programming (OOP) principles, clean code design, advanced LINQ pipelines, relational database design (3NF ERDs), SQL querying, and professional development workflows.

---

##  Tech Stack & Badges
![C#](https://img.shields.io/badge/Language-C%23-blue.svg)
![.NET 10.0](https://img.shields.io/badge/Platform-.NET%2010.0-purple.svg)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red.svg)
![OOP](https://img.shields.io/badge/Paradigm-OOP-green.svg)
![Clean Architecture](https://img.shields.io/badge/Design-3--Layer%20Architecture-orange.svg)

---

##  Phase Tasks Overview

### [Task 00: Environment Setup](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-00-setup/)
- **Description**: Initial configuration and development tool chain installation.
- **Key Deliverables**: Installation of .NET 10 SDK and verifying console runtime health.

### [Task 01: C# Programming Drills](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-01-csharp-drills/)
- **Description**: 20 targeted C# logic puzzles focusing on data types, control flow, string manipulation, and list management.
- **Drill Categories**:
  - *Basic Logic*: Temperature converter, grade calculator, login validation.
  - *Data Structures*: Word counter, dictionary frequency counters, duplicate number detectors, array rotations.
  - *Method Refactoring*: Moving procedural scripts into single-responsibility, reusable methods.

### [Task 02: Bank Account System](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-02-bank-account-system/)
- **Description**: A console-based in-memory bank account auditor demonstrating basic object relations and encapsulation.
- **Key Features**:
  - Account creation with unique Guids.
  - Deposit, withdrawal, and peer-to-peer transfers with business logic checks.
  - Transaction log history tracking and account summary DTOs.
- **Core Design**: Follows a strict 3-Layer structure separating the console UI, banking service manager, and core domain models.

### [Task 03: Employee Management System](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-03-employee-management/)
- **Description**: In-memory employee administration directory utilizing alphanumeric identification formats and sorting pipelines.
- **Key Features**:
  - Employee profile modifications and soft deactivations (`IsActive = false`).
  - Search engine utilizing partial name matching and exact ID matches.
  - Advanced aggregations for department headcount and payroll analytics.

### [Task 04: Product Catalog LINQ System](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-04-product-catalog-linq/)
- **Description**: High-performance product catalog reporting pipeline showcasing 20 advanced LINQ queries.
- **LINQ Capabilities Covered**:
  - Complex category statistical groupings (`GroupBy` + Multi-aggregates like `Min`, `Max`, `Average`).
  - Dynamic paginated queries (`Skip` and `Take`).
  - Supplier analytics, date arithmetic filtering, and deferred query chaining.

### [Task 05: Order Calculator Debug & Refactoring](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-05-order-calculator-refactor/)
- **Description**: Re-architecting a legacy monolithic, error-prone ordering application into clean, professional code.
- **Key Refactoring Pillars**:
  - Upgrading currency arithmetic to exact `decimal` precision to prevent floating-point representation rounding issues.
  - Introducing strongly-typed `Enums` for loyalty tiers to eliminate case-sensitive string matching issues.
  - Creating a reusable validation helper class to prevent console crash issues on bad inputs.

### [Task 06: SQL & ERD Relational Starter](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-06-sql-erd-starter/)
- **Description**: Architectural database modeling and SQL queries across three distinct business domains.
- **Scenarios Modeled**:
  - **Scenario A (Library Management)**: Books, authors, categories, library members, and borrowing logs.
  - **Scenario B (Simple Store)**: Customers, categories, suppliers, catalog products, orders, and order items.
  - **Scenario C (Training Center)**: Students, instructors, course tracks, registrations, and payment receipts.
- **Highlights**: Features visual Mermaid ERD diagrams, 3NF normalization designs, constraints definition schemas, and complex join queries.

### [Task 07: Interview Answers & Explanation Pack](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-01-backend-foundations/task-07-interview-answers-explanation-pack/)
- **Description**: Comprehensive review guide covering 22 fundamental backend developer interview questions.
- **Topics Explored**:
  - *OOP*: Class vs Object, Encapsulation rules, Field vs Property, Constructors, Service Classes.
  - *C# Collections*: Array vs List vs Dictionary performance complexities.
  - *LINQ*: Operations, Select vs Where, GroupBy groupings, and Pagination.
  - *Databases*: Primary vs Foreign keys, Relationships, JOIN operations, and Entity mapping.
  - *Workflows*: Git commit strategies, GitHub collaboration, and README documentation.

---

##  Core Architectural Patterns

Across all interactive programs developed in this phase, a consistent **3-Layer Separation of Concerns** was maintained:

1.  **Presentation (UI) Layer**: Handles console reading, input prompting, output color formatting, and ASCII tables. Contains zero database modification logic or calculation formulas.
2.  **Service (Business) Layer**: Manages domain workflows, handles input boundaries, coordinates models, and processes logical outcomes.
3.  **Domain (Model) Layer**: Pure domain entities encapsulating private data structures, enforcing structural validation, and maintaining internal state invariants.
