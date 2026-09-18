# Phase 03: Task 03 API Testing & Evidence Pack

> **TechMaster Academy — Training Center Registration API**  
> **Topic:** Automated Testing Evidence, Postman Test Execution, Swagger API Contracts, and Response Verification.

---

## 📋 Evidence Overview

This document provides visual and textual proof of the **Training Center Registration API** functionality, error handling, business rule validations, status transitions, and analytical reporting metrics.

---

## 🎯 Test Scenarios Matrix

| # | Endpoint / Scenario | HTTP Method | Expected Status | Verified Behavior |
| :---: | :--- | :---: | :---: | :--- |
| **1** | `GET /api/students` | `GET` | `200 OK` | Returns paginated list of students with active enrollment counts. |
| **2** | `GET /api/students/{id}` | `GET` | `200 OK` | Returns student details with full course and payment history. |
| **3** | `GET /api/students/99999` *(Negative)* | `GET` | `404 Not Found` | Returns structured `NotFoundException` error response. |
| **4** | `POST /api/students` | `POST` | `201 Created` | Creates new student with unique email and returns `CreatedAtRoute`. |
| **5** | `POST /api/students` *(Negative)* | `POST` | `409 Conflict` | Rejects duplicate email registration. |
| **6** | `PUT /api/students/{id}` | `PUT` | `200 OK` | Updates student information and sets `UpdatedAt` UTC timestamp. |
| **7** | `DELETE /api/students/{id}` | `DELETE` | `200 OK` | Soft-deletes student record (`IsDeleted = 1`). |
| **8** | `GET /api/instructors` | `GET` | `200 OK` | Returns instructors list with active track counts. |
| **9** | `GET /api/tracks` | `GET` | `200 OK` | Filters tracks by level, status, keyword, and instructor. |
| **10** | `POST /api/tracks` *(Negative)* | `POST` | `400 Bad Request` | Rejects track creation when `EndDate < StartDate`. |
| **11** | `DELETE /api/tracks/{id}` *(Negative)* | `DELETE` | `400 Bad Request` | Prevents track deletion when active student enrollments exist. |
| **12** | `POST /api/enrollments` | `POST` | `201 Created` | Enrolls student into track with available capacity. |
| **13** | `POST /api/enrollments` *(Negative)* | `POST` | `409 Conflict` | Prevents student from enrolling in the same track twice. |
| **14** | `POST /api/enrollments` *(Negative)* | `POST` | `400 Bad Request` | Prevents enrollment when track has reached full capacity. |
| **15** | `PUT /api/enrollments/{id}/status` | `PUT` | `200 OK` | Updates progress percentage and final certification result. |
| **16** | `POST /api/payments` | `POST` | `201 Created` | Processes payment and calculates outstanding balance. |
| **17** | `POST /api/payments` *(Negative)* | `POST` | `409 Conflict` | Rejects duplicate gateway reference numbers. |
| **18** | `GET /api/reports/dashboard-summary` | `GET` | `200 OK` | Delivers real-time KPIs (active learners, revenue, tracks). |
| **19** | `GET /api/reports/unpaid-enrollments` | `GET` | `200 OK` | Identifies all unpaid and partially paid enrollments. |
| **20** | `GET /api/reports/revenue-by-track` | `GET` | `200 OK` | Groups total expected vs realized revenue per track. |

---

## 📦 Sample API Request & Response Payloads

### 1. Create Student (`POST /api/students`)
**Request Body:**
```json
{
  "fullName": "Amr Khaled",
  "email": "amr.khaled@student.techmaster.net",
  "phoneNumber": "+201099887766",
  "isActive": true
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Student created successfully.",
  "data": {
    "studentId": 7,
    "fullName": "Amr Khaled",
    "email": "amr.khaled@student.techmaster.net",
    "phoneNumber": "+201099887766",
    "isActive": true,
    "createdAt": "2026-09-18T14:30:00Z",
    "updatedAt": null,
    "enrollments": []
  },
  "errors": [],
  "statusCode": 201
}
```

---

### 2. Duplicate Email Conflict (`POST /api/students` - 409 Conflict)
**Response:**
```json
{
  "success": false,
  "message": "A student with email 'nada.ashraf@student.techmaster.net' already exists.",
  "data": null,
  "errors": [],
  "statusCode": 409
}
```

---

### 3. Track Full Capacity Protection (`POST /api/enrollments` - 400 Bad Request)
**Response:**
```json
{
  "success": false,
  "message": "Training track 'ASP.NET Core Enterprise Backend BootCamp' has reached its maximum capacity of 3 students.",
  "data": null,
  "errors": [],
  "statusCode": 400
}
```

---

### 4. Dashboard Reporting KPIs (`GET /api/reports/dashboard-summary`)
**Response (200 OK):**
```json
{
  "success": true,
  "message": "Dashboard summary metrics retrieved successfully.",
  "data": {
    "totalStudents": 6,
    "activeStudents": 6,
    "totalInstructors": 4,
    "totalTracks": 4,
    "activeTracks": 2,
    "totalEnrollments": 7,
    "activeEnrollments": 5,
    "totalExpectedRevenue": 34900.00,
    "totalRealizedRevenue": 18500.00,
    "totalOutstandingRevenue": 16400.00,
    "completedPaymentsCount": 6
  },
  "errors": [],
  "statusCode": 200
}
```
