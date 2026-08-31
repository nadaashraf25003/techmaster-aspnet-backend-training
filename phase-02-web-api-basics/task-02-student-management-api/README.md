# Task 02 - Student Management API

## Overview
The **Student Management API** is a RESTful ASP.NET Core Web API built for **TechMaster Academy** administrators, mentors, and management to manage student profiles, tracks, statuses, and performance statistics throughout the career training program.

---

## Architecture & Clean Code Principles
This project strictly follows the **Separation of Concerns** principle:
- **Controllers Layer (`Controllers/`)**: Handles HTTP requests, model state validation, HTTP status codes (`201 Created`, `200 OK`, `400 Bad Request`, `404 Not Found`), and produces standard response formats.
- **Service Layer (`Services/`)**: Encapsulates business logic, email uniqueness checks, thread-safe in-memory storage, pagination calculations, and statistical aggregations.
- **DTOs Layer (`DTOs/`)**: Enforces input validation attributes (`[Required]`, `[EmailAddress]`, `[Phone]`, `[Range]`) and shapes clean API outputs decoupled from internal domain entities.
- **Models Layer (`Models/`)**: Represents internal domain entities with creation and modification tracking (`CreatedAt`, `UpdatedAt`).

---

## Features Summary

| Feature # | Feature Name | HTTP Method | Endpoint Route | Description & Business Rules |
|---|---|---|---|---|
| **01** | **Create Student** | `POST` | `/api/students` | Creates a new student profile. Validates required fields and ensures email is unique. Returns `201 Created` with `Location` header. |
| **02** | **Get All Students** | `GET` | `/api/students` | Returns paginated students. Supports search by Name/Email, filter by `TrackName`, and filter by `IsActive`. |
| **03** | **Get Student By ID** | `GET` | `/api/students/{id}` | Returns a single student profile. Returns `404 Not Found` if missing (never `null` with `200`). |
| **04** | **Update Student** | `PUT` | `/api/students/{id}` | Modifies an existing student's details. Prevents ID modification and validates unique email across other students. |
| **05** | **Update Student Status** | `PATCH` | `/api/students/{id}/status` | Activates or deactivates a student without deleting historical data. Returns status change message. |
| **06** | **Student Statistics** | `GET` | `/api/students/stats` | Aggregates total students, active count, inactive count, and student distribution by track. |

---

## API Endpoints & Request/Response Samples

### 1. Create Student (`POST /api/students`)
**Request Body**:
```json
{
  "fullName": "Nada Ashraf",
  "email": "nada.ashraf@techmaster.com",
  "phoneNumber": "+201099887766",
  "trackName": "ASP.NET Backend",
  "isActive": true
}
```
**Response (`201 Created`)**:
```json
{
  "id": 9,
  "fullName": "Nada Ashraf",
  "email": "nada.ashraf@techmaster.com",
  "phoneNumber": "+201099887766",
  "trackName": "ASP.NET Backend",
  "isActive": true,
  "createdAt": "2026-08-30T19:09:35.3204468Z",
  "updatedAt": null
}
```

---

### 2. Get All Students (`GET /api/students`)
**Query Parameters**:
- `search` *(string, optional)*: Substring search across `fullName` and `email`.
- `trackName` *(string, optional)*: Filter by track (e.g., `ASP.NET Backend`, `Frontend React`).
- `isActive` *(bool, optional)*: Filter by `true` or `false`.
- `pageNumber` *(int, optional, default: 1)*: Page number (1-based).
- `pageSize` *(int, optional, default: 10)*: Number of records per page.

**Example Request**: `GET /api/students?trackName=ASP.NET%20Backend&pageNumber=1&pageSize=2`

**Response (`200 OK`)**:
```json
{
  "totalCount": 2,
  "pageNumber": 1,
  "pageSize": 2,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false,
  "items": [
    {
      "id": 1,
      "fullName": "Ahmed Hassan",
      "email": "ahmed.hassan@techmaster.com",
      "phoneNumber": "+201012345678",
      "trackName": "ASP.NET Backend",
      "isActive": true,
      "createdAt": "2026-05-30T19:09:34.9402404Z",
      "updatedAt": null
    },
    {
      "id": 2,
      "fullName": "Sara Mahmoud",
      "email": "sara.mahmoud@techmaster.com",
      "phoneNumber": "+201023456789",
      "trackName": "ASP.NET Backend",
      "isActive": true,
      "createdAt": "2026-06-30T19:09:34.9403089Z",
      "updatedAt": null
    }
  ]
}
```

---

### 3. Get Student By ID (`GET /api/students/{id}`)
**Example Request**: `GET /api/students/1`
**Response (`200 OK`)**:
```json
{
  "id": 1,
  "fullName": "Ahmed Hassan",
  "email": "ahmed.hassan@techmaster.com",
  "phoneNumber": "+201012345678",
  "trackName": "ASP.NET Backend",
  "isActive": true,
  "createdAt": "2026-05-30T19:09:34.9402404Z",
  "updatedAt": null
}
```

**Missing ID Request**: `GET /api/students/999`
**Response (`404 Not Found`)**:
```json
{
  "message": "Student with ID 999 was not found.",
  "timestamp": "2026-08-30T19:09:35.1234567Z"
}
```

---

### 4. Update Student (`PUT /api/students/{id}`)
**Request Body**:
```json
{
  "fullName": "Ahmed Hassan Updated",
  "email": "ahmed.updated@techmaster.com",
  "phoneNumber": "+201011112222",
  "trackName": "ASP.NET Backend",
  "isActive": true
}
```
**Response (`200 OK`)**:
```json
{
  "id": 1,
  "fullName": "Ahmed Hassan Updated",
  "email": "ahmed.updated@techmaster.com",
  "phoneNumber": "+201011112222",
  "trackName": "ASP.NET Backend",
  "isActive": true,
  "createdAt": "2026-05-30T19:09:34.9402404Z",
  "updatedAt": "2026-08-30T19:09:35.3824238Z"
}
```

---

### 5. Update Student Status (`PATCH /api/students/{id}/status`)
**Request Body**:
```json
{
  "isActive": false
}
```
**Response (`200 OK`)**:
```json
{
  "message": "Student status successfully updated to Inactive.",
  "student": {
    "id": 1,
    "fullName": "Ahmed Hassan Updated",
    "email": "ahmed.updated@techmaster.com",
    "phoneNumber": "+201011112222",
    "trackName": "ASP.NET Backend",
    "isActive": false,
    "createdAt": "2026-05-30T19:09:34.9402404Z",
    "updatedAt": "2026-08-30T19:09:35.4064535Z"
  }
}
```

---

### 6. Student Statistics (`GET /api/students/stats`)
**Response (`200 OK`)**:
```json
{
  "totalStudents": 9,
  "activeStudents": 6,
  "inactiveStudents": 3,
  "countByTrack": {
    "ASP.NET Backend": 3,
    "Frontend React": 2,
    "Mobile Flutter": 2,
    "DevOps & Cloud": 2
  }
}
```

---

## How to Run & Test

### Run locally with .NET CLI
```bash
cd phase-02-web-api-basics/task-02-student-management-api/StudentManagementApi
dotnet run
```
Swagger UI will be accessible at:
- `http://localhost:5050/swagger`
- `https://localhost:7050/swagger`

### Test with Postman
1. Import `StudentManagementApi.postman_collection.json` into Postman.
2. Select the desired request from folders `Feature 01` through `Feature 06`.
3. Execute and inspect status codes and response bodies.
