# Task 07: Audit Trail & Activity Timeline

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  

---

## 🎯 Task Overview

Implement an immutable audit trail and historical timeline for tracking entity mutations:
1. **Audit Entity Model:** `ActivityLog` capturing `ActorId`, `ActorRole`, `ActionType`, `EntityName`, `EntityId`, `DetailsJson`, `IpAddress`, `TimestampUtc`.
2. **Automated Interceptor / Hook:** DbContext interceptor or domain event dispatcher capturing mutations on Tracks, Enrollments, Payments, and User roles.
3. **Audit Query Endpoints:**
   - `GET /api/admin/audit/timeline` — Query activity timeline with filtering by date range, actor, and entity type.
   - `GET /api/admin/audit/entities/{entityName}/{entityId}` — Historical change trail for a specific entity.
