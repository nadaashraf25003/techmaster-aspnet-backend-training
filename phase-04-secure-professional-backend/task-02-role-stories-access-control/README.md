# Task 02: Role Stories & Access Control (RBAC)

> **Phase 04 — Secure Professional Backend Systems**  
> **TechMaster ASP.NET Backend Career Training**  

---

## 🎯 Task Overview

Define and implement role-based access control (RBAC) across the three core user personas:
1. **Admin Persona:** Full system access, track creation, price modifications, user management, financial reports, and audit logs.
2. **Instructor Persona:** Access to assigned tracks, student roster for assigned tracks, and profile management. Cannot alter track prices or access system financials.
3. **Student Persona:** Access to personal enrollments, course catalogs, payment submissions, and personal balance due. Strictly isolated from other students' data.
4. **Ownership Verification:** Custom claims extension methods and authorization guards preventing IDOR vulnerabilities.
