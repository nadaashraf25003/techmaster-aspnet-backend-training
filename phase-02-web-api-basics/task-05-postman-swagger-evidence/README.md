# Task 05 - Swagger & Postman Evidence Pack

**TechMaster Academy | ASP.NET Backend Career Training**  
**Phase 02:** Web API Basics

---

##  Overview

This directory contains the **API Evidence Pack** for Phase 02. It serves as tangible verification and documentation proving that all ASP.NET Core Web API endpoints across **Student Management API**, **Products & Categories API**, and **Book Store API** are functional, adhere to REST standards, correctly handle data payloads, and respond with proper HTTP status codes.

---

##  Pack Contents & Structure

```text
task-05-postman-swagger-evidence/
├── TechMaster ASP.NET Phase 02.postman_collection.json    # Complete exported Postman Collection
├── TechMaster Phase 02.postman_environment.json           # Environment variables (Base URLs)
└── README.md                                              # This documentation
```

---

##  How to Run and Test

### 1. Run the APIs Locally
Each API project has its own dedicated HTTP port:
- **Student Management API** (Task 02): `http://localhost:5050`
- **Products & Categories API** (Task 03): `http://localhost:5060`
- **Book Store API** (Task 04): `http://localhost:5220`

To start any project:
```bash
# Navigate to the project folder and run:
dotnet run
```

### 2. Import into Postman
1. Open **Postman**.
2. Click **Import** in the top left corner.
3. Drag and drop `TechMaster ASP.NET Phase 02.postman_collection.json` and `TechMaster Phase 02.postman_environment.json`.
4. In Postman, select the environment **"TechMaster Phase 02 Local"**.
5. Set `{{baseUrl}}` to the port of the project currently running (e.g. `http://localhost:5050` for Student API, `http://localhost:5060` for Product API, or `http://localhost:5220` for Book Store API).

---

##  Evidence Checklist

### 1. Swagger Evidence (Minimum 8 Endpoints)
- [x] Swagger UI opens successfully for all projects at `/swagger`.
- [x] All controllers and actions visible with appropriate HTTP verbs (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`).
- [x] DTO request schemas and data types are explicitly documented.
- [x] Response codes (`200`, `201`, `400`, `404`) and schemas are visible.

### 2. Postman Collection Verification
- [x] **Student Management API**
  - [x] `POST /api/students` (201 Created)
  - [x] `GET /api/students` (200 OK - Paginated)
  - [x] `GET /api/students/{id}` (200 OK)
  - [x] `PUT /api/students/{id}` (200 OK)
  - [x] `PATCH /api/students/{id}/status` (200 OK)
  - [x] `GET /api/students/stats` (200 OK)
- [x] **Products & Categories API**
  - [x] `POST /api/categories` (201 Created)
  - [x] `POST /api/products` (201 Created)
  - [x] `GET /api/products` (200 OK - Filter/Search)
  - [x] `GET /api/products/low-stock` (200 OK)
  - [x] `GET /api/products/reports/stock-value` (200 OK)
- [x] **Book Store API**
  - [x] `POST /api/authors` (201 Created)
  - [x] `POST /api/categories` (201 Created)
  - [x] `POST /api/books` (201 Created)
  - [x] `GET /api/books` (200 OK - Search & Paging)
  - [x] `GET /api/books/reports/summary` (200 OK)
- [x] **Error Cases**
  - [x] Missing Resource (`404 Not Found`)
  - [x] Invalid Request Body / Negative Numbers (`400 Bad Request`)
  - [x] Model Validation Failure (`400 Bad Request / 422 Unprocessable Entity`)
