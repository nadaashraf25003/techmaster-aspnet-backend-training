# Task 07: Audit Trail & Activity Timeline

> **ASP.NET Core Backend Career Training — Phase 04**  
> **Module:** Audit Logging, Historical Timelines, Fail-Safe Interceptors & Admin Observability  
> **Target Standard:** 100/100 Enterprise Auditing Standard  

---

## 🌟 Overview & Business Scenario

In real-world enterprise applications, regulatory compliance, internal accountability, and operational troubleshooting demand a tamper-proof audit trail. TechMaster administrators need full visibility into:
- **Who** initiated an action (User ID, Role, Email, IP Address).
- **What** changed (Entity type, Entity ID, and before/after state transition metadata).
- **When** the event took place (ISO 8601 UTC timestamp).
- **How** the request was correlated (Correlation ID / Request ID).

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│                           ACTIVITY AUDIT ARCHITECTURE                            │
├──────────────────────────────────────────────────────────────────────────────────┤
│  [ Trigger Sources ]  Register • Login • Track CRUD • Enrollment • Payment       │
│                                      │                                           │
│                                      ▼                                           │
│  [ Interceptor/Svc ]  IAuditService / AuditService (Async, Fail-Safe)            │
│                       Auto-extracts Claims, Client IP, & X-Correlation-ID        │
│                                      │                                           │
│                                      ▼                                           │
│  [ Persistence ]      DbSet<ActivityLog> ➔ Indexed SQL Database Table            │
│                                      │                                           │
│                                      ▼                                           │
│  [ Admin Endpoints ]  GET /api/admin/activity-logs (Paginated & Filterable)      │
│                       GET /api/admin/activity-logs/{id} (Detail View)            │
│                       GET /api/admin/activity-logs/summary (Aggregates & Stats)  │
│                       GET /api/admin/activity-logs/timeline/{entity}/{id}        │
└──────────────────────────────────────────────────────────────────────────────────┘
```

---

## 📊 ActivityLog Entity Schema

The `ActivityLog` table is configured with composite database indexes on `CreatedAt`, `UserId`, `EntityName`, and `Action` for fast pagination and reporting:

```csharp
public class ActivityLog
{
    public int ActivityLogId { get; set; }
    public int? UserId { get; set; }
    public string? UserRole { get; set; }
    public string? UserEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? CorrelationId { get; set; }
    public string? Metadata { get; set; }
}
```

---

## 📋 Required Logged Actions & Metadata

| Action Name | Target Entity | Trigger Condition | Stored Differential Metadata |
| :--- | :--- | :--- | :--- |
| `UserRegistered` | `ApplicationUser` | New student or instructor registers | `{ "role": "Student", "linkedStudentId": 5 }` |
| `UserLoggedIn` | `ApplicationUser` | Successful authentication | `{ "role": "Admin", "ipAddress": "127.0.0.1" }` |
| `LoginFailed` | `ApplicationUser` | Failed password or unknown email | `{ "reason": "InvalidPassword", "ipAddress": "..." }` |
| `TrackCreated` | `TrainingTrack` | Admin creates new track | `{ "code": "NET-01", "price": 7500.00, "capacity": 25 }` |
| `TrackUpdated` | `TrainingTrack` | Admin edits track | `{ "price": 8000.00, "capacity": 30 }` |
| `EnrollmentRequested`| `Enrollment` | Student self-enrolls or admin enrolls | `{ "studentId": 10, "trackId": 2, "status": "Pending" }` |
| `EnrollmentStatusUpdated`| `Enrollment` | Admin transitions status | `{ "previousStatus": "Pending", "newStatus": "Active" }` |
| `PaymentCreated` | `Payment` | Student submits payment | `{ "amount": 7500.00, "method": "CreditCard", "ref": "PAY-..." }` |
| `PaymentStatusUpdated`| `Payment` | Gateway confirms completion | `{ "previousStatus": "Pending", "newStatus": "Completed" }` |
| `ReportViewed` | `Report` | Admin views financial reports | `{ "report": "RevenueSummary", "totalRevenue": 150000 }` |

---

## 🔒 Security & RBAC Enforcement

- **Admin Role Only (`[Authorize(Roles = "Admin")]`):**
  - Only users with the `Admin` claim can query activity logs, summaries, and timelines.
- **Role Boundary Defenses:**
  - `401 Unauthorized` returned for anonymous requests.
  - `403 Forbidden` returned for `Student` and `Instructor` attempts to access audit logs.

---

## 🌐 Admin API Endpoints

| Method | Endpoint | Query Parameters | Description |
| :---: | :--- | :--- | :--- |
| `GET` | `/api/admin/activity-logs` | `pageNumber`, `pageSize`, `userId`, `entityName`, `action`, `from`, `to`, `search` | Paginated, filterable activity log query. |
| `GET` | `/api/admin/activity-logs/{id}` | - | Detailed log entry with raw metadata. |
| `GET` | `/api/admin/activity-logs/summary` | - | Aggregated metrics: total logs, action breakdown, recent feed. |
| `GET` | `/api/admin/activity-logs/timeline/{entityName}/{entityId}` | - | Chronological history of a specific entity (e.g. `Enrollment`, `Payment`). |

---

## 🧪 Verification & Automated Testing

Run the automated PowerShell verification suite:

```powershell
powershell -ExecutionPolicy Bypass -File .\scratch\test_task07.ps1
```

All audit trail criteria, security guards, filtering parameters, and timeline queries are verified automatically.
