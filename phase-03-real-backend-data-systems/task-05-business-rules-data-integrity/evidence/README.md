# Task 05 — Business Rules & Data Integrity Evidence

## Executive Summary

This folder contains test verification evidence, endpoint outputs, Postman collection exports, and integrity validation logs for **Task 05: Business Rules & Data Integrity** under **Phase 03 — Real Backend Data Systems**.

---

## 1. Postman Test Suite Execution

The automated Postman verification suite covers **100% of all required positive and negative business rule scenarios**:

| Rule ID | Business Rule Description | Endpoint Tested | Expected HTTP | Actual HTTP | Result |
| :--- | :--- | :--- | :---: | :---: | :---: |
| **BR-S1** | Duplicate Student Email Rejection | `POST /api/students` | `400 Bad Request` | `400` | **PASS** |
| **BR-S2** | Mandatory FullName Validation | `POST /api/students` | `400 Bad Request` | `400` | **PASS** |
| **BR-S3** | Soft-Deleted Students Hidden from Queries | `GET /api/students` | `200 OK` | `200` | **PASS** |
| **BR-S4** | Soft-Delete Safeguard (Preserves Row) | `DELETE /api/students/7` | `200 OK` | `200` | **PASS** |
| **BR-S5** | Inactive Student Blocked from New Enrollment | `POST /api/enrollments` | `400 Bad Request` | `400` | **PASS** |
| **BR-T1** | Unique Track Code Enforcement | `POST /api/tracks` | `400 Bad Request` | `400` | **PASS** |
| **BR-T2** | Mandatory Track Title Validation | `POST /api/tracks` | `400 Bad Request` | `400` | **PASS** |
| **BR-T3** | Strictly Positive Capacity (`Capacity > 0`) | `POST /api/tracks` | `400 Bad Request` | `400` | **PASS** |
| **BR-T3b** | Date Chronology (`StartDate < EndDate`) | `POST /api/tracks` | `400 Bad Request` | `400` | **PASS** |
| **BR-T4** | Active Instructor Assignment Required | `POST /api/tracks` | `400 Bad Request` | `400` | **PASS** |
| **BR-T5** | Track Capacity Exceeded Enforcement | `POST /api/enrollments` | `400 Bad Request` | `400` | **PASS** |
| **BR-T6** | Closed/Completed Track Enrollment Rejection | `POST /api/enrollments` | `400 Bad Request` | `400` | **PASS** |
| **BR-E1** | Duplicate Active/Pending Enrollment Prevention | `POST /api/enrollments` | `400 Bad Request` | `400` | **PASS** |
| **BR-E2** | Default Initial Status is `Pending` | `POST /api/enrollments` | `201 Created` | `201` | **PASS** |
| **BR-E3** | Auto-Activation on Successful Payment | `POST /api/payments` | `201 Created` | `201` | **PASS** |
| **BR-E4** | Completed Enrollment Cannot Be Cancelled | `PATCH /api/enrollments/{id}/status` | `400 Bad Request` | `400` | **PASS** |
| **BR-E5** | Cancelled Enrollments Ignored in Capacity | `GET /api/reports/track-capacities` | `200 OK` | `200` | **PASS** |
| **BR-P1** | Strictly Positive Payment (`Amount > 0`) | `POST /api/payments` | `400 Bad Request` | `400` | **PASS** |
| **BR-P2** | Overpayment Rejection (`Amount > Remaining`) | `POST /api/payments` | `400 Bad Request` | `400` | **PASS** |
| **BR-P3** | Valid Payment Method Enum Validation | `POST /api/payments` | `400 Bad Request` | `400` | **PASS** |
| **BR-P4** | Only Completed Payments Count in Revenue | `GET /api/reports/revenue-summary` | `200 OK` | `200` | **PASS** |
| **BR-P5** | Failed Payments Do Not Reduce Balance | `POST /api/payments` | `400/201` | `201` | **PASS** |
| **BR-P6** | Payment on Cancelled Enrollment Blocked | `POST /api/payments` | `400 Bad Request` | `400` | **PASS** |

---

## 2. Sample Verification Outputs

### Overpayment Rejection (BR-P2)
```http
POST /api/payments
Content-Type: application/json

{
  "enrollmentId": 1,
  "amount": 10000.00,
  "paymentMethod": "CreditCard",
  "notes": "Attempting overpayment"
}
```
**Response (HTTP 400 Bad Request):**
```json
{
  "success": false,
  "message": "Payment amount of 10,000.00 EGP exceeds remaining outstanding balance of 0.00 EGP.",
  "data": null,
  "errors": [
    "Maximum allowable payment for this enrollment is 0.00 EGP."
  ],
  "statusCode": 400,
  "timestamp": "2026-09-18T21:27:57.3296881Z"
}
```

### Completed Enrollment Cancellation Blocked (BR-E4)
```http
PATCH /api/enrollments/1/status
Content-Type: application/json

{
  "status": "Cancelled",
  "notes": "Attempting to cancel completed graduation"
}
```
**Response (HTTP 400 Bad Request):**
```json
{
  "success": false,
  "message": "Completed enrollments cannot be cancelled directly. Please contact an administrator.",
  "data": null,
  "errors": [],
  "statusCode": 400,
  "timestamp": "2026-09-18T21:27:57.310388Z"
}
```

### Capacity Exceeded Rejection (BR-T5)
```http
POST /api/enrollments
Content-Type: application/json

{
  "studentId": 1,
  "trainingTrackId": 2,
  "notes": "Enrollment attempt on full track"
}
```
**Response (HTTP 400 Bad Request):**
```json
{
  "success": false,
  "message": "Track 'Full Stack Modern React & Next.js MasterClass' has reached its maximum capacity of 2 students.",
  "data": null,
  "errors": [],
  "statusCode": 400,
  "timestamp": "2026-09-18T21:27:57.2464248Z"
}
```

---

## 3. Postman Deliverables
- Collection: [`postman/TechMaster_Business_Rules_API.postman_collection.json`](../postman/TechMaster_Business_Rules_API.postman_collection.json)
- Environment: [`postman/TechMaster_Task05_Environment.postman_environment.json`](../postman/TechMaster_Task05_Environment.postman_environment.json)
