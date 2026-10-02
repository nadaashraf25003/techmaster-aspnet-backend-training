# Task 02: Role Stories & Access Control • RBAC Matrix • Ownership Guards • IDOR Prevention

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  
> **Score Standard:** 100/100 (Top Student Performance Standard)  
> **Core Theme:** Enforcing strict Role-Based Access Control (RBAC), domain ownership validation, and endpoint-level security boundaries across Admin, Instructor, and Student roles.

---

## 📌 1. Executive Summary & Mission

In **Task 01**, we established identity, password hashing, and JWT issuance. However, authentication alone only proves *who* the caller is; it does not determine *what* they are permitted to do.

**Task 02 turns generic CRUD endpoints into secure business workflows respecting strict role boundaries:**
1. **Admin Role Boundary:** Comprehensive governance. Can manage students, instructors, tracks, view all enrollments, update payment transaction statuses, and access executive revenue analytics.
2. **Instructor Role Boundary:** Restricted to pedagogical scope. Can view own assigned tracks (`/api/instructor/my-tracks`), view students enrolled in their assigned tracks, and view track-level progress. Instructors are strictly forbidden from viewing or modifying financial/revenue reports.
3. **Student Role Boundary:** Restricted to self-service. Can view their own profile, track enrollments (`/api/student/my-enrollments`), payment history (`/api/student/my-payments`), and explore publicly offered tracks. Students are forbidden from viewing other students' private data or modifying payment records.
4. **Ownership-Based IDOR Defense:** Enforced cross-tenant domain ownership checks (preventing Insecure Direct Object References). Even with a valid token, an Instructor cannot inspect another Instructor's track roster, and a Student cannot inspect or mutate another Student's profile or enrollments.

---

## 🔐 2. Access Matrix (Source of Truth)

The matrix below is the governing security specification for the TechMaster Backend platform:

| Endpoint Group | Admin | Instructor | Student | Security Rule & Design Rationale |
| :--- | :---: | :---: | :---: | :--- |
| **Students CRUD** | **Full Access** | Read students in own tracks only | Own profile only | No public student directory. Protects student PII and privacy. |
| **Instructors CRUD** | **Full Access** | Own profile only | No access | Instructors cannot register or elevate other instructors. |
| **Tracks Management** | **Full Access** | Assigned tracks read/update limited | Read available tracks | Track creation, deletion, and pricing are admin-owned operations. |
| **Enrollments** | **Full Access** | Read own track enrollments | Own enrollments only | Students cannot enroll other students or approve their own enrollments. |
| **Payments** | **Full Access** | No revenue access | Own payment history only | Payment reconciliation and status updates are restricted to Admin. |
| **Reports** | **Full Access** | Own track reports | No admin reports | Executive revenue summaries are confidential to leadership/Admin. |
| **Audit Logs** | **Full Access** | Own operations optional | No access | Audit log tampering prevention; administrative visibility only. |

---

## 🛡️ 3. Complete Endpoint Security & Role Boundary Specification

### A. Authentication & Identity (`/api/auth/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/auth/register` | `POST` | Public | Allows prospective students and instructors to register accounts. | Admin self-registration is blocked (`400 Bad Request`). |
| `/api/auth/login` | `POST` | Public | Authenticates credentials and returns JWT + Refresh Token. | Invalid credentials return `401 Unauthorized`; inactive users receive `403 Forbidden`. |
| `/api/auth/me` | `GET` | Authenticated (`Admin`, `Instructor`, `Student`) | Resolves the authenticated caller's identity and domain profile. | Unauthenticated requests return `401 Unauthorized`. |
| `/api/auth/change-password` | `POST` | Authenticated | Allows users to securely update their own passwords. | Unauthenticated requests return `401 Unauthorized`. |
| `/api/auth/refresh-token` | `POST` | Public | Rotates expired access tokens using a valid refresh token. | Expired or invalid tokens return `401 Unauthorized`. |
| `/api/auth/logout` | `POST` | Authenticated | Revokes active refresh token sessions. | Unauthenticated requests return `401 Unauthorized`. |

---

### B. Students Management & Profiles (`/api/students/*` & `/api/student/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/students` | `GET` | `Admin` | System-wide student roster and registration oversight. | `401 Unauthorized` if unauthenticated; `403 Forbidden` for Instructor & Student. |
| `/api/students/{id}` | `GET` | `Admin`, `Student` (Self Only) | Administrative inspection or student self-profile retrieval. | `403 Forbidden` if a Student attempts to view `{otherId}` or Instructor calls directly. |
| `/api/students` | `POST` | `Admin` | Direct creation of student records by administration. | `401 Unauthorized` / `403 Forbidden` for non-Admin. |
| `/api/students/{id}` | `PUT` | `Admin`, `Student` (Self Only) | Update profile details. | `403 Forbidden` if a Student attempts to update another student's profile. |
| `/api/students/{id}` | `DELETE` | `Admin` | Student offboarding and record deactivation. | `403 Forbidden` for Instructor & Student. |
| `/api/student/my-profile` | `GET` | `Student` | Dedicated shortcut endpoint returning the calling student's profile. | `403 Forbidden` for Admin & Instructor; `401 Unauthorized` if no token. |
| `/api/student/my-enrollments` | `GET` | `Student` | Fetches courses and track progress for the logged-in student. | `403 Forbidden` for Instructor & Admin; `401 Unauthorized` if no token. |
| `/api/student/my-payments` | `GET` | `Student` | Displays personal billing and payment history. | `403 Forbidden` for Instructor & Admin; `401 Unauthorized` if no token. |

---

### C. Instructors Management (`/api/instructors/*` & `/api/instructor/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/instructors` | `GET` | `Admin` | Global directory of faculty and teaching staff. | `403 Forbidden` for Student & Instructor. |
| `/api/instructors/{id}` | `GET` | `Admin`, `Instructor` (Self Only) | Profile lookup for instructor management. | `403 Forbidden` if an Instructor attempts to access another instructor's record. |
| `/api/instructors` | `POST` | `Admin` | Staff hiring and onboarding into the academy. | `403 Forbidden` for Instructor & Student. |
| `/api/instructors/{id}` | `PUT` | `Admin`, `Instructor` (Self Only) | Bio and contact detail updates. | `403 Forbidden` if cross-instructor modification is attempted. |
| `/api/instructors/{id}` | `DELETE` | `Admin` | Faculty termination/offboarding. | `403 Forbidden` for non-Admins. |
| `/api/instructor/my-tracks` | `GET` | `Instructor` | Retrieves all training tracks assigned to the logged-in instructor. | `403 Forbidden` for Student & Admin; `401 Unauthorized` if no token. |
| `/api/instructor/my-students` | `GET` | `Instructor` | Retrieves all students currently enrolled in the instructor's tracks. | `403 Forbidden` for Student & Admin; `401 Unauthorized` if no token. |

---

### D. Tracks & Course Management (`/api/tracks/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/tracks` | `GET` | Public / Authenticated | Public course catalog for discovery and enrollment exploration. | Allowed for all callers (`200 OK`). |
| `/api/tracks/{id}` | `GET` | Public / Authenticated | Individual track overview, prerequisites, and syllabus. | Allowed for all callers (`200 OK`). |
| `/api/tracks` | `POST` | `Admin` | Track curriculum creation and pricing structure. | `403 Forbidden` for Instructor & Student. |
| `/api/tracks/{id}` | `PUT` | `Admin` | Track metadata and schedule updates. | `403 Forbidden` for Instructor & Student. |
| `/api/tracks/{id}` | `DELETE` | `Admin` | Track deprecation and removal. | `403 Forbidden` for Instructor & Student. |
| `/api/tracks/{id}/students` | `GET` | `Admin`, `Instructor` (Assigned Only) | Retrieves enrolled students for a track. Instructors can ONLY view their own tracks. | `403 Forbidden` if Instructor A requests roster for Instructor B's track (`{otherId}`). |

---

### E. Enrollments & Registrations (`/api/enrollments/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/enrollments` | `GET` | `Admin` | Academy-wide enrollment directory and capacity management. | `403 Forbidden` for Instructor & Student. |
| `/api/enrollments/{id}` | `GET` | `Admin`, `Student` (Self Only), `Instructor` (Assigned Track) | Enrollment lookup with granular ownership validation. | `403 Forbidden` if Student/Instructor lacks ownership. |
| `/api/enrollments` | `POST` | `Admin`, `Student` (Self Only) | Self-enrollment for students or manual enrollment by Admin. | `403 Forbidden` if a Student passes a `studentId` other than their own. |
| `/api/enrollments/{id}/status` | `PUT` | `Admin` | Approving, rejecting, or canceling student enrollment state. | `403 Forbidden` for Student & Instructor. |
| `/api/enrollments/{id}` | `DELETE` | `Admin` | Hard deletion of enrollment records. | `403 Forbidden` for Student & Instructor. |

---

### F. Payments & Financial Transactions (`/api/payments/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/payments` | `GET` | `Admin` | Full ledger and transaction audit trail. | `403 Forbidden` for Instructor & Student. |
| `/api/payments/{id}` | `GET` | `Admin`, `Student` (Self Only) | Receipt and transaction detail inspection. | `403 Forbidden` for Instructors or Students accessing others' invoices. |
| `/api/payments` | `POST` | `Admin`, `Student` (Self Only) | Student tuition payment processing. | `403 Forbidden` if Student attempts to log payment for another student. |
| `/api/payments/{id}/status` | `PUT` / `PATCH` | `Admin` | Bank reconciliation and payment status confirmation (`Paid`, `Refunded`, `Failed`). | `403 Forbidden` for Student & Instructor. |

---

### G. Executive & Analytical Reports (`/api/reports/*`)
| Endpoint | Method | Allowed Roles | Business Justification | Failure Outcome (Wrong Role / Unauthenticated) |
| :--- | :---: | :--- | :--- | :--- |
| `/api/reports/revenue-summary` | `GET` | `Admin` | Executive financial summary, total collected revenue, and outstanding dues. | `401 Unauthorized` (No token), `403 Forbidden` (Student/Instructor). |
| `/api/reports/enrollment-stats` | `GET` | `Admin` | Academy conversion rates, capacity saturation, and seat metrics. | `403 Forbidden` for Student & Instructor. |
| `/api/reports/track-performance`| `GET` | `Admin` | Top-performing tracks and instructor capacity evaluation. | `403 Forbidden` for Student & Instructor. |

---

## 🧪 4. Required Authorization Verification Evidence

The 8 mandatory security verification scenarios were executed against the live API and recorded below:

| # | Test Scenario | HTTP Request | Auth State / Token | Target Resource | Expected Result | Actual Result | Status |
| :-: | :--- | :--- | :--- | :--- | :-: | :-: | :-: |
| **1** | Anonymous access to Admin Report | `GET /api/reports/revenue-summary` | **None** | Executive Revenue Summary | `401 Unauthorized` | `401 Unauthorized` | ✅ **PASS** |
| **2** | Student attempts to read Admin Report | `GET /api/reports/revenue-summary` | **Student** (`student@techmaster.com`) | Executive Revenue Summary | `403 Forbidden` | `403 Forbidden` | ✅ **PASS** |
| **3** | Instructor reads their assigned tracks | `GET /api/instructor/my-tracks` | **Instructor** (`instructor@techmaster.com`) | Assigned Tracks (InstructorId=1) | `200 OK` | `200 OK` | ✅ **PASS** |
| **4** | Instructor accesses another instructor's track students | `GET /api/tracks/3/students` | **Instructor** (`instructor@techmaster.com`, Id=1) | Track 3 (Assigned to InstructorId=2) | `403 Forbidden` | `403 Forbidden` | ✅ **PASS** |
| **5** | Student views their own enrollments | `GET /api/student/my-enrollments` | **Student** (`student@techmaster.com`) | Student 1's Enrollments | `200 OK` | `200 OK` | ✅ **PASS** |
| **6** | Student attempts to access another student's profile | `GET /api/students/2` | **Student** (`student@techmaster.com`, Id=1) | Student 2 (`Salma Mahmoud`) | `403 Forbidden` | `403 Forbidden` | ✅ **PASS** |
| **7** | Admin updates payment transaction status | `PUT /api/payments/1/status` | **Admin** (`admin@techmaster.com`) | Payment 1 (`status: "Completed"`) | `200 OK` | `200 OK` | ✅ **PASS** |
| **8** | Student attempts to update payment status | `PUT /api/payments/1/status` | **Student** (`student@techmaster.com`) | Payment 1 (`status: "Completed"`) | `403 Forbidden` | `403 Forbidden` | ✅ **PASS** |

---

## ⚙️ 5. Implementation Architecture: IDOR Defense & Ownership Guards

Role-based authorization is implemented at two distinct architectural levels:

### 1. Declarative RBAC Middleware (`[Authorize(Roles = "...")]`)
Applied on controllers and actions to reject callers lacking the required broad capability immediately.
```csharp
[Authorize(Roles = "Admin")]
public class ReportsController : BaseApiController
{
    [HttpGet("revenue-summary")]
    public async Task<IActionResult> GetRevenueSummary() => ...
}
```

### 2. Imperative Domain Ownership Guards (BaseApiController + Services)
Prevents horizontal privilege escalation (IDOR) by validating that the calling user's claim matches the target resource:
```csharp
// StudentsController.cs - Checking Self Access
if (IsStudent())
{
    var currentStudentId = GetAuthenticatedStudentId();
    if (currentStudentId != id)
    {
        throw new ForbiddenException("Access denied. You are only authorized to view your own student profile.");
    }
}

// TracksController.cs - Checking Instructor Track Assignment
if (IsInstructor())
{
    var instructorId = GetAuthenticatedInstructorId();
    var track = await _trackService.GetTrackByIdAsync(id);
    if (track == null || track.InstructorId != instructorId)
    {
        throw new ForbiddenException("Access denied. You are only authorized to view students enrolled in your own assigned tracks.");
    }
}
```

---

## 📦 6. Postman Test Suite Execution

A fully configured Postman collection is supplied in this folder:
📁 `phase-04-secure-professional-backend/task-02-role-stories-access-control/postman/TechMaster_Phase04_Task02_Role_Stories.postman_collection.json`

### To Run the Postman Suite:
1. Open **Postman** -> Click **Import** -> Select the JSON file above.
2. Set the collection variable `baseUrl` to `http://localhost:5000` (or `http://localhost:5399`).
3. Open **Collection Runner** -> Select all 11 requests -> Click **Run TechMaster Phase 04 - Task 02 - Role Stories & Access Control**.
4. All 11 automated test assertions will execute sequentially and pass (`100% PASS`).

---

## ✅ 7. Task 02 Acceptance Criteria Checklist

- [x] Strict RBAC enforced on all controllers (`[Authorize(Roles = "...")]`).
- [x] Admin workflows: Full management for students, instructors, tracks, enrollments, payments, and reports.
- [x] Instructor workflows: Restricted to `/api/instructor/my-tracks`, `/api/instructor/my-students`, and assigned track rosters.
- [x] Student workflows: Restricted to `/api/student/my-profile`, `/api/student/my-enrollments`, `/api/student/my-payments`, and available tracks.
- [x] IDOR defense: Cross-student profile access returns `403 Forbidden`.
- [x] IDOR defense: Cross-instructor track student access returns `403 Forbidden`.
- [x] Financial protection: Payment status update restricted to Admin (Student receives `403 Forbidden`).
- [x] Executive reporting protection: Revenue summary returns `401 Unauthorized` without token and `403 Forbidden` for non-Admin.
- [x] 8 Mandatory test cases verified and documented.
- [x] Complete Access Matrix included in README.
- [x] Solution builds with 0 Errors and 0 Warnings.
