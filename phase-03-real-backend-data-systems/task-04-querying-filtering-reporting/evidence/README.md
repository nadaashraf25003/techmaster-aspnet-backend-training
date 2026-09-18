# Task 04: Querying, Filtering, and Reporting — Evidence & Verification

> **TechMaster Academy • Phase 03: Real Backend Data Systems**  
> **Topic:** Advanced EF Core Querying, Dynamic Filtering, Aggregations, Server-Side Pagination, and Business Analytics.

---

## 📸 1. Swagger Interactive Documentation Evidence

The API exposes an interactive Swagger documentation portal at the application root URL (`/` or `/swagger`):

```text
[GET] /api/payments?from={from}&to={to}&status={status}&method={method} (Query 13)
[GET] /api/reports/revenue-summary                                     (Query 14)
[GET] /api/reports/revenue-by-track                                    (Query 15)
[GET] /api/reports/top-tracks?count={count}                            (Query 16)
[GET] /api/reports/instructor-workload                                 (Query 17)
[GET] /api/reports/students-without-payments                           (Query 18)
[GET] /api/enrollments?trackId={id}&status={st}&paymentStatus={ps}     (Query 19)
[GET] /api/reports/dashboard-summary                                   (Query 20)
[GET] /api/reports/unpaid-enrollments
[GET] /api/reports/track-capacity
```

---

## 🧪 2. Core Query Responses & DTO Samples

### Query 13: Payments By Date Range (`GET /api/payments?from=2026-07-01&to=2026-07-31`)
```json
{
  "success": true,
  "message": "Payments retrieved successfully.",
  "statusCode": 200,
  "data": {
    "items": [
      {
        "paymentId": 4,
        "enrollmentId": 6,
        "studentName": "Khaled Mostafa",
        "trackTitle": "Enterprise Cloud DevOps & Kubernetes Engineering",
        "amount": 9000.00,
        "paymentMethod": "CreditCard",
        "paymentDate": "2026-07-15T10:15:00Z",
        "paymentStatus": "Completed",
        "referenceNumber": "PAY-2026-07-004",
        "notes": "Mastercard payment.",
        "createdAt": "2026-07-15T10:15:00Z"
      },
      {
        "paymentId": 3,
        "enrollmentId": 4,
        "studentName": "Omar Farouk",
        "trackTitle": "Advanced Applied Machine Learning & Deep Learning",
        "amount": 10500.00,
        "paymentMethod": "BankTransfer",
        "paymentDate": "2026-07-11T14:30:00Z",
        "paymentStatus": "Completed",
        "referenceNumber": "PAY-2026-07-003",
        "notes": "Direct CIB bank transfer.",
        "createdAt": "2026-07-11T14:30:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 6,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
}
```

### Query 13 Validation Failure (`GET /api/payments?from=2026-08-01&to=2026-07-01`)
```json
{
  "success": false,
  "message": "Invalid date range: 'from' date must be less than or equal to 'to' date.",
  "errors": [
    "'from' (2026-08-01) cannot be greater than 'to' (2026-07-01)."
  ],
  "statusCode": 400,
  "timestamp": "2026-09-18T20:30:00Z"
}
```

### Query 14: Revenue Summary (`GET /api/reports/revenue-summary`)
```json
{
  "success": true,
  "message": "Revenue summary retrieved successfully.",
  "statusCode": 200,
  "data": {
    "totalRevenue": 48000.00,
    "realizedRevenue": 48000.00,
    "expectedRevenue": 72500.00,
    "paidCount": 6,
    "pendingCount": 1,
    "failedCount": 1,
    "totalTransactionsCount": 8,
    "averagePaymentAmount": 8000.00
  }
}
```

### Query 15: Revenue Per Track (`GET /api/reports/revenue-by-track`)
```json
{
  "success": true,
  "message": "Revenue by track report retrieved successfully.",
  "statusCode": 200,
  "data": [
    {
      "trackId": 4,
      "trackCode": "REACT-FS-2026",
      "trackTitle": "Full Stack Modern React & Next.js MasterClass",
      "unitPrice": 7500.00,
      "enrollmentCount": 2,
      "totalPaid": 15000.00,
      "expectedRevenue": 15000.00,
      "collectionRatePercentage": 100.00
    },
    {
      "trackId": 1,
      "trackCode": "NET-BE-2026",
      "trackTitle": "ASP.NET Core Enterprise Backend BootCamp",
      "unitPrice": 8500.00,
      "enrollmentCount": 3,
      "totalPaid": 13500.00,
      "expectedRevenue": 25500.00,
      "collectionRatePercentage": 52.94
    },
    {
      "trackId": 2,
      "trackCode": "AI-ML-2026",
      "trackTitle": "Advanced Applied Machine Learning & Deep Learning",
      "unitPrice": 10500.00,
      "enrollmentCount": 2,
      "totalPaid": 10500.00,
      "expectedRevenue": 21000.00,
      "collectionRatePercentage": 50.00
    },
    {
      "trackId": 3,
      "trackCode": "DEVOPS-2026",
      "trackTitle": "Enterprise Cloud DevOps & Kubernetes Engineering",
      "unitPrice": 9000.00,
      "enrollmentCount": 1,
      "totalPaid": 9000.00,
      "expectedRevenue": 9000.00,
      "collectionRatePercentage": 100.00
    }
  ]
}
```

### Query 16: Top Tracks By Enrollment (`GET /api/reports/top-tracks?count=3`)
```json
{
  "success": true,
  "message": "Top 3 tracks by active enrollment retrieved successfully.",
  "statusCode": 200,
  "data": [
    {
      "trackId": 1,
      "trackCode": "NET-BE-2026",
      "trackTitle": "ASP.NET Core Enterprise Backend BootCamp",
      "capacity": 25,
      "activeEnrollmentCount": 3,
      "totalEnrollmentCount": 3,
      "availableSeats": 22,
      "utilizationPercentage": 12.00,
      "instructorName": "Eng. Mohamed Ali"
    },
    {
      "trackId": 2,
      "trackCode": "AI-ML-2026",
      "trackTitle": "Advanced Applied Machine Learning & Deep Learning",
      "capacity": 20,
      "activeEnrollmentCount": 1,
      "totalEnrollmentCount": 2,
      "availableSeats": 18,
      "utilizationPercentage": 5.00,
      "instructorName": "Dr. Sara Hassan"
    },
    {
      "trackId": 3,
      "trackCode": "DEVOPS-2026",
      "trackTitle": "Enterprise Cloud DevOps & Kubernetes Engineering",
      "capacity": 22,
      "activeEnrollmentCount": 1,
      "totalEnrollmentCount": 1,
      "availableSeats": 21,
      "utilizationPercentage": 4.55,
      "instructorName": "Eng. Tarek Mahmoud"
    }
  ]
}
```

### Query 17: Instructor Workload (`GET /api/reports/instructor-workload`)
```json
{
  "success": true,
  "message": "Instructor workload metrics retrieved successfully.",
  "statusCode": 200,
  "data": [
    {
      "instructorId": 1,
      "instructorName": "Eng. Mohamed Ali",
      "email": "m.ali@techmaster.edu",
      "specialization": ".NET Cloud & Enterprise Architecture",
      "numberOfTracks": 2,
      "activeTracksCount": 1,
      "activeStudentsCount": 3,
      "totalStudentsSupervised": 3,
      "totalRevenueGenerated": 13500.00
    },
    {
      "instructorId": 2,
      "instructorName": "Dr. Sara Hassan",
      "email": "s.hassan@techmaster.edu",
      "specialization": "AI & Machine Learning Engineering",
      "numberOfTracks": 1,
      "activeTracksCount": 1,
      "activeStudentsCount": 1,
      "totalStudentsSupervised": 2,
      "totalRevenueGenerated": 10500.00
    }
  ]
}
```

### Query 18: Students Without Payments (`GET /api/reports/students-without-payments`)
```json
{
  "success": true,
  "message": "Students with active/pending enrollments and zero payments retrieved successfully.",
  "statusCode": 200,
  "data": [
    {
      "studentId": 8,
      "studentName": "Hassan Radwan",
      "email": "hassan.radwan@outlook.com",
      "phoneNumber": "+201100008888",
      "enrollmentId": 8,
      "enrollmentStatus": "Pending",
      "enrollmentDate": "2026-07-18T15:00:00Z",
      "trackId": 5,
      "trackCode": "SEC-PEN-2026",
      "trackTitle": "Enterprise Cybersecurity & Penetration Testing",
      "trackPrice": 9500.00,
      "totalPaid": 0.00,
      "outstandingBalance": 9500.00
    },
    {
      "studentId": 3,
      "studentName": "Salma Ibrahim",
      "email": "salma.ibrahim@outlook.com",
      "phoneNumber": "+201100003333",
      "enrollmentId": 3,
      "enrollmentStatus": "Active",
      "enrollmentDate": "2026-07-04T11:00:00Z",
      "trackId": 1,
      "trackCode": "NET-BE-2026",
      "trackTitle": "ASP.NET Core Enterprise Backend BootCamp",
      "trackPrice": 8500.00,
      "totalPaid": 0.00,
      "outstandingBalance": 8500.00
    },
    {
      "studentId": 5,
      "studentName": "Yara Adel",
      "email": "yara.adel@gmail.com",
      "phoneNumber": "+201100005555",
      "enrollmentId": 5,
      "enrollmentStatus": "Pending",
      "enrollmentDate": "2026-07-12T16:00:00Z",
      "trackId": 2,
      "trackCode": "AI-ML-2026",
      "trackTitle": "Advanced Applied Machine Learning & Deep Learning",
      "trackPrice": 10500.00,
      "totalPaid": 0.00,
      "outstandingBalance": 10500.00
    }
  ]
}
```

### Query 19: Advanced Enrollment Filter (`GET /api/enrollments?trackId=1&status=Active&paymentStatus=Paid`)
```json
{
  "success": true,
  "message": "Enrollments retrieved successfully.",
  "statusCode": 200,
  "data": {
    "items": [
      {
        "enrollmentId": 2,
        "studentId": 2,
        "studentName": "Ahmed Mansour",
        "studentEmail": "ahmed.mansour@yahoo.com",
        "trainingTrackId": 1,
        "trackCode": "NET-BE-2026",
        "trackTitle": "ASP.NET Core Enterprise Backend BootCamp",
        "trackPrice": 8500.00,
        "enrollmentDate": "2026-07-03T10:30:00Z",
        "status": "Active",
        "progressPercentage": 50.00,
        "totalPaid": 5000.00,
        "financialStatus": "Partially Paid"
      },
      {
        "enrollmentId": 1,
        "studentId": 1,
        "studentName": "Nada Ashraf",
        "studentEmail": "nada.ashraf@gmail.com",
        "trainingTrackId": 1,
        "trackCode": "NET-BE-2026",
        "trackTitle": "ASP.NET Core Enterprise Backend BootCamp",
        "trackPrice": 8500.00,
        "enrollmentDate": "2026-07-02T09:00:00Z",
        "status": "Active",
        "progressPercentage": 65.00,
        "totalPaid": 8500.00,
        "financialStatus": "Fully Paid"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 2,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  }
}
```

### Query 20: Dashboard Summary (`GET /api/reports/dashboard-summary`)
```json
{
  "success": true,
  "message": "Dashboard summary metrics retrieved successfully.",
  "statusCode": 200,
  "data": {
    "studentsCount": 8,
    "activeStudentsCount": 8,
    "instructorsCount": 5,
    "tracksCount": 6,
    "activeTracksCount": 4,
    "totalEnrollmentsCount": 10,
    "activeEnrollments": 6,
    "revenue": 48000.00,
    "realizedRevenue": 48000.00,
    "expectedRevenue": 72500.00,
    "paidCount": 6,
    "unpaidCount": 5
  }
}
```
