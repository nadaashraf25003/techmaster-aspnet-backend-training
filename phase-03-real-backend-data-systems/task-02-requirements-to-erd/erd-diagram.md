# Entity Relationship Diagram (ERD) Deliverable

> **Phase 03: Task 02 • Requirements to ERD**  
> **TechMaster Academy Database Architecture**

---

## 1. Core Domain ERD (Mermaid Notation)

```mermaid
erDiagram
    STUDENTS ||--o{ ENROLLMENTS : "places (1:N)"
    TRAINING_TRACKS ||--o{ ENROLLMENTS : "contains (1:N)"
    INSTRUCTORS ||--o{ TRAINING_TRACKS : "instructs (1:N)"
    ENROLLMENTS ||--o{ PAYMENTS : "generates (1:N)"

    STUDENTS {
        int StudentId PK "Identity(1,1)"
        nvarchar(150) FullName "Required"
        varchar(150) Email UK "Required, Unique"
        varchar(25) PhoneNumber "Optional"
        bit IsActive "Default 1"
        bit IsDeleted "Default 0, Soft Delete"
        datetime2 CreatedAt "UTC, Default SYSUTCDATETIME()"
        datetime2 UpdatedAt "UTC, Nullable"
        datetime2 DeletedAt "UTC, Nullable"
    }

    INSTRUCTORS {
        int InstructorId PK "Identity(1,1)"
        nvarchar(150) FullName "Required"
        varchar(150) Email UK "Required, Unique"
        nvarchar(100) Specialization "Required"
        nvarchar(1000) Bio "Optional"
        bit IsActive "Default 1"
        datetime2 CreatedAt "UTC, Default SYSUTCDATETIME()"
        datetime2 UpdatedAt "UTC, Nullable"
    }

    TRAINING_TRACKS {
        int TrainingTrackId PK "Identity(1,1)"
        nvarchar(200) Title "Required"
        varchar(30) Code UK "Required, Unique"
        nvarchar(2000) Description "Optional"
        varchar(20) Level "Beginner | Intermediate | Advanced"
        decimal(18_2) Price "Tuition, >= 0"
        int Capacity "Seats count, > 0"
        date StartDate "Track launch date"
        date EndDate "Track finish date >= StartDate"
        varchar(20) Status "Draft | Upcoming | InProgress | Completed | Cancelled"
        int InstructorId FK "References Instructors(InstructorId)"
        bit IsDeleted "Default 0, Soft Delete"
        datetime2 CreatedAt "UTC, Default SYSUTCDATETIME()"
        datetime2 UpdatedAt "UTC, Nullable"
        datetime2 DeletedAt "UTC, Nullable"
    }

    ENROLLMENTS {
        int EnrollmentId PK "Identity(1,1)"
        int StudentId FK "References Students(StudentId)"
        int TrainingTrackId FK "References TrainingTracks(TrainingTrackId)"
        datetime2 EnrollmentDate "UTC booking date"
        varchar(20) Status "Pending | Active | Completed | Dropped | Suspended"
        decimal(5_2) ProgressPercentage "Range 0.00 to 100.00"
        varchar(20) FinalResult "Pass | Fail | Distinction | Incomplete | Null"
        datetime2 CreatedAt "UTC, Default SYSUTCDATETIME()"
        datetime2 UpdatedAt "UTC, Nullable"
    }

    PAYMENTS {
        int PaymentId PK "Identity(1,1)"
        int EnrollmentId FK "References Enrollments(EnrollmentId)"
        decimal(18_2) Amount "Financial amount > 0"
        varchar(30) PaymentMethod "CreditCard | BankTransfer | Cash | Fawry | VodafoneCash"
        datetime2 PaymentDate "UTC timestamp of settlement"
        varchar(20) PaymentStatus "Pending | Completed | Failed | Refunded"
        varchar(100) ReferenceNumber UK "Transaction UUID/Bank Ref"
        nvarchar(500) Notes "Optional comments"
        datetime2 CreatedAt "UTC, Default SYSUTCDATETIME()"
    }
```

---

## 2. Extended Domain ERD (Including Bonus Entities)

```mermaid
erDiagram
    INSTRUCTORS ||--o{ TRAINING_TRACKS : "teaches (1:N)"
    STUDENTS ||--o{ ENROLLMENTS : "registers (1:N)"
    TRAINING_TRACKS ||--o{ ENROLLMENTS : "accepts (1:N)"
    ENROLLMENTS ||--o{ PAYMENTS : "billed_by (1:N)"
    
    TRAINING_TRACKS ||--o{ TRACK_SESSIONS : "schedules (1:N)"
    TRACK_SESSIONS ||--o{ ATTENDANCES : "records (1:N)"
    STUDENTS ||--o{ ATTENDANCES : "logs_presence (1:N)"
    
    TRAINING_TRACKS ||--o{ ASSIGNMENTS : "assigns (1:N)"
    ASSIGNMENTS ||--o{ ASSIGNMENT_SUBMISSIONS : "collects (1:N)"
    STUDENTS ||--o{ ASSIGNMENT_SUBMISSIONS : "hands_in (1:N)"

    STUDENTS {
        int StudentId PK
        nvarchar(150) FullName
        varchar(150) Email UK
        varchar(25) PhoneNumber
        bit IsActive
        bit IsDeleted
        datetime2 CreatedAt
    }

    INSTRUCTORS {
        int InstructorId PK
        nvarchar(150) FullName
        varchar(150) Email UK
        nvarchar(100) Specialization
        bit IsActive
    }

    TRAINING_TRACKS {
        int TrainingTrackId PK
        nvarchar(200) Title
        varchar(30) Code UK
        decimal(18_2) Price
        int Capacity
        date StartDate
        date EndDate
        varchar(20) Status
        int InstructorId FK
    }

    ENROLLMENTS {
        int EnrollmentId PK
        int StudentId FK
        int TrainingTrackId FK
        datetime2 EnrollmentDate
        varchar(20) Status
        decimal(5_2) ProgressPercentage
        varchar(20) FinalResult
    }

    PAYMENTS {
        int PaymentId PK
        int EnrollmentId FK
        decimal(18_2) Amount
        varchar(30) PaymentMethod
        datetime2 PaymentDate
        varchar(20) PaymentStatus
        varchar(100) ReferenceNumber UK
    }

    TRACK_SESSIONS {
        int SessionId PK
        int TrainingTrackId FK
        nvarchar(150) Title
        date SessionDate
        time StartTime
        time EndTime
        nvarchar(255) RoomOrLink
        bit IsCompleted
    }

    ATTENDANCES {
        int AttendanceId PK
        int TrackSessionId FK
        int StudentId FK
        varchar(20) Status
        datetime2 MarkedAt
        nvarchar(255) Remarks
    }

    ASSIGNMENTS {
        int AssignmentId PK
        int TrainingTrackId FK
        nvarchar(150) Title
        datetime2 DueDate
        decimal(5_2) MaxScore
        decimal(5_2) WeightPercentage
    }

    ASSIGNMENT_SUBMISSIONS {
        int SubmissionId PK
        int AssignmentId FK
        int StudentId FK
        datetime2 SubmittedAt
        nvarchar(500) ContentUrl
        decimal(5_2) Score
        nvarchar(1000) Feedback
        datetime2 GradedAt
    }
```

---

## 3. Cardinality & Crow's Foot Notation Explanation

| Notation Symbol | Symbol Meaning | Relationship Context |
| :---: | :--- | :--- |
| `||--o{` | **One-to-Many (Mandatory Parent, Optional Child)** | An `Instructor` can exist with zero `TrainingTracks` initially, but every `TrainingTrack` MUST have exactly one `Instructor`. |
| `||--o{` | **One-to-Many (Mandatory Parent, Optional Child)** | A `Student` may have 0 or more `Enrollments`. Each `Enrollment` MUST link to exactly one `Student`. |
| `||--o{` | **One-to-Many (Mandatory Parent, Optional Child)** | A `TrainingTrack` may have 0 or more `Enrollments`. Each `Enrollment` MUST link to exactly one `TrainingTrack`. |
| `||--o{` | **One-to-Many (Mandatory Parent, Optional Child)** | An `Enrollment` may have 0 or more `Payments` (installment payments). Each `Payment` belongs to exactly one `Enrollment`. |

---

## 4. Key Constraints and Indexing Matrix

| Table | Index Name | Type | Columns | Properties |
| :--- | :--- | :--- | :--- | :--- |
| `Students` | `PK_Students` | Clustered | `StudentId` | Primary Key |
| `Students` | `UQ_Students_Email` | Non-Clustered | `Email` | Filtered: `WHERE IsDeleted = 0` (Unique) |
| `Instructors` | `PK_Instructors` | Clustered | `InstructorId` | Primary Key |
| `Instructors` | `UQ_Instructors_Email` | Non-Clustered | `Email` | Unique |
| `TrainingTracks` | `PK_TrainingTracks` | Clustered | `TrainingTrackId` | Primary Key |
| `TrainingTracks` | `UQ_TrainingTracks_Code`| Non-Clustered | `Code` | Filtered: `WHERE IsDeleted = 0` (Unique) |
| `TrainingTracks` | `IX_TrainingTracks_InstructorId` | Non-Clustered | `InstructorId` | Foreign Key Performance Index |
| `Enrollments` | `PK_Enrollments` | Clustered | `EnrollmentId` | Primary Key |
| `Enrollments` | `UQ_Enrollments_Student_Track` | Non-Clustered | `StudentId, TrainingTrackId` | Unique (Prevents duplicate enrollment) |
| `Enrollments` | `IX_Enrollments_TrackId` | Non-Clustered | `TrainingTrackId` | Foreign Key / Lookup Performance Index |
| `Payments` | `PK_Payments` | Clustered | `PaymentId` | Primary Key |
| `Payments` | `UQ_Payments_ReferenceNumber` | Non-Clustered | `ReferenceNumber` | Unique (Idempotency / Gateway ref) |
| `Payments` | `IX_Payments_EnrollmentId` | Non-Clustered | `EnrollmentId` | Foreign Key / Ledger lookup |
