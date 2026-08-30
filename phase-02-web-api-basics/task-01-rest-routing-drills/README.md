# Task 01 - REST & Routing Drill Pack

## Overview
Task 01 provides 15 comprehensive ASP.NET Core Web API drills designed to bridge console programming with production-grade Web API design. The drills demonstrate controllers, route parameters, query strings, request body DTOs, HTTP verbs, status codes (200, 201, 204, 400, 404), in-memory data management, search, pagination, request headers, dependency injection, and standardized error formats.

---

## 15 API Drills Summary Table

| Drill No. | HTTP Method | Endpoint | Concept | Status | Evidence / Notes |
|---|---|---|---|---|---|
| **01** | `GET` | `/api/health` | Basic Endpoint & Status | Completed | Returns `200 OK` JSON with service name, status, and UTC timestamp |
| **02** | `GET` | `/api/tools/echo/{name}` | Route Parameters & Validation | Completed | Returns `200 OK` greeting; `400 Bad Request` if name is whitespace |
| **03** | `GET` | `/api/calculator/add?a=10&b=5` | Query String Parameters | Completed | Accepts query parameters `a` and `b`, returns structured addition result |
| **04** | `GET` | `/api/converter/celsius-to-fahrenheit?value=25` | DI & Service Layer Separation | Completed | Uses `IConverterService` registered via DI for business logic calculation |
| **05** | `GET` | `/api/grades/calculate?score=85` | Input Validation & Conditions | Completed | Validates 0-100 score; returns `400 Bad Request` if invalid, or grade (A-F) & Pass/Fail status |
| **06** | `POST` | `/api/notes` | Request Body & DTOs | Completed | Accepts `CreateNoteRequest` JSON body; returns `201 Created` with `CreatedAtAction` location |
| **07** | `GET` | `/api/notes` | Collection Responses | Completed | Returns `200 OK` with in-memory seeded list of notes |
| **08** | `GET` | `/api/notes/{id}` | Route ID & 404 Handling | Completed | Returns `200 OK` when found, or `404 Not Found` if ID does not exist |
| **09** | `PUT` | `/api/notes/{id}` | Resource Updates & Validation | Completed | Accepts `UpdateNoteRequest`, validates inputs, returns `200 OK` or `404 Not Found` |
| **10** | `DELETE` | `/api/notes/{id}` | DELETE & 204 No Content | Completed | Removes note from in-memory collection and returns `204 No Content` (or `404 Not Found`) |
| **11** | `GET` | `/api/notes/search?keyword=api` | Search Query String | Completed | Case-insensitive search matching against Title and Content |
| **12** | `GET` | `/api/notes/paged?pageNumber=1&pageSize=5` | Skip/Take Pagination | Completed | Validates page bounds and returns `PagedResult<T>` with metadata |
| **13** | `GET` | `/api/request-info` | Request Headers Inspection | Completed | Reads `X-Student-Name` header, returning `400 Bad Request` if missing |
| **14** | `GET`/`POST`/`DELETE` | `/api/status-codes/...` | HTTP Status Codes Practice | Completed | Demonstrates explicit responses for `200`, `201`, `204`, `400`, and `404` |
| **15** | `GET` | `/api/errors/demo?type=bad-request` | Standardized Error Shapes | Completed | Demonstrates unified error shape (`success`, `message`, `errors`, `statusCode`, `timestamp`) |

---

## Project Structure

```
task-01-rest-routing-drills/
├── README.md
└── ApiRoutingDrills/
    ├── ApiRoutingDrills.csproj
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Controllers/
    │   ├── HealthController.cs          # Drill 01: Health check
    │   ├── ToolsController.cs           # Drill 02: Route echo
    │   ├── CalculatorController.cs      # Drill 03: Calculator
    │   ├── ConverterController.cs       # Drill 04: Temperature converter
    │   ├── GradesController.cs          # Drill 05: Grade calculation
    │   ├── NotesController.cs           # Drills 06-12: Notes CRUD, Search & Pagination
    │   ├── RequestInfoController.cs     # Drill 13: Request headers
    │   ├── StatusCodesController.cs     # Drill 14: Status codes practice
    │   └── ErrorsDemoController.cs      # Drill 15: Standard error response
    ├── DTOs/
    │   ├── CreateNoteRequest.cs
    │   ├── UpdateNoteRequest.cs
    │   ├── PagedResult.cs
    │   └── StandardErrorResponse.cs
    ├── Models/
    │   └── Note.cs
    └── Services/
        ├── IConverterService.cs
        └── ConverterService.cs
```

---

## Detailed Drill Documentation

### Drill 01: Health Check Endpoint
- **Route:** `GET /api/health`
- **Response (200 OK):**
```json
{
  "status": "Running",
  "service": "TechMaster API",
  "time": "2026-08-29T21:30:00.0000000Z"
}
```

### Drill 02: Route Parameter Echo
- **Route:** `GET /api/tools/echo/{name}`
- **Example Request:** `GET /api/tools/echo/Mohamed`
- **Response (200 OK):**
```json
{
  "originalName": "Mohamed",
  "message": "Hello, Mohamed!"
}
```
- **Error Case (400 Bad Request):** If name is whitespace.

### Drill 03: Query String Calculator
- **Route:** `GET /api/calculator/add?a=10&b=5`
- **Response (200 OK):**
```json
{
  "a": 10.0,
  "b": 5.0,
  "operation": "add",
  "result": 15.0
}
```

### Drill 04: Temperature Conversion API
- **Route:** `GET /api/converter/celsius-to-fahrenheit?value=25`
- **Response (200 OK):**
```json
{
  "celsius": 25.0,
  "fahrenheit": 77.0,
  "formulaUsed": "F = (C * 9/5) + 32"
}
```

### Drill 05: Grade API
- **Route:** `GET /api/grades/calculate?score=85`
- **Response (200 OK):**
```json
{
  "score": 85.0,
  "grade": "B",
  "status": "Pass"
}
```
- **Error Case (400 Bad Request):** `GET /api/grades/calculate?score=120` -> `{"error": "Score must be between 0 and 100."}`

### Drill 06: Create Note
- **Route:** `POST /api/notes`
- **Request Body:**
```json
{
  "title": "ASP.NET Core Best Practices",
  "content": "Always separate business logic into services and use DTOs."
}
```
- **Response (201 Created):**
```json
{
  "id": 6,
  "title": "ASP.NET Core Best Practices",
  "content": "Always separate business logic into services and use DTOs.",
  "createdAt": "2026-08-29T21:30:00.0000000Z",
  "updatedAt": null
}
```

### Drill 07: Get All Notes
- **Route:** `GET /api/notes`
- **Response (200 OK):** Returns array of note objects.

### Drill 08: Get Note by ID
- **Route:** `GET /api/notes/1`
- **Response (200 OK):** Returns single note object.
- **Error Case (404 Not Found):** `GET /api/notes/999` -> `{"message": "Note with id 999 not found."}`

### Drill 09: Update Note
- **Route:** `PUT /api/notes/1`
- **Request Body:**
```json
{
  "title": "Updated Title",
  "content": "Updated content."
}
```
- **Response (200 OK):** Returns updated note with `updatedAt` timestamp.

### Drill 10: Delete Note
- **Route:** `DELETE /api/notes/1`
- **Response (204 No Content):** Empty body indicating successful deletion.
- **Error Case (404 Not Found):** If note ID does not exist.

### Drill 11: Search Notes
- **Route:** `GET /api/notes/search?keyword=rest`
- **Response (200 OK):** Returns array of notes containing "rest" in title or content.

### Drill 12: Pagination Demo
- **Route:** `GET /api/notes/paged?pageNumber=1&pageSize=3`
- **Response (200 OK):**
```json
{
  "items": [ ... ],
  "pageNumber": 1,
  "pageSize": 3,
  "totalCount": 5,
  "totalPages": 2
}
```

### Drill 13: Request Headers
- **Route:** `GET /api/request-info`
- **Header:** `X-Student-Name: Mohamed`
- **Response (200 OK):**
```json
{
  "studentName": "Mohamed",
  "requestPath": "/api/request-info",
  "timestamp": "2026-08-29T21:30:00.0000000Z"
}
```
- **Error Case (400 Bad Request):** If header is omitted.

### Drill 14: Status Codes Practice
- `GET /api/status-codes/200-ok` -> `200 OK`
- `POST /api/status-codes/201-created` -> `201 Created`
- `DELETE /api/status-codes/204-no-content` -> `204 No Content`
- `GET /api/status-codes/400-bad-request` -> `400 Bad Request`
- `GET /api/status-codes/404-not-found` -> `404 Not Found`

### Drill 15: Standard Error Shape
- **Route:** `GET /api/errors/demo?type=validation`
- **Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "One or more validation errors occurred.",
  "errors": [
    "Field 'Title' is required.",
    "Field 'Email' must be a valid email format.",
    "Field 'Age' must be between 18 and 100."
  ],
  "statusCode": 400,
  "timestamp": "2026-08-29T21:30:00.0000000Z"
}
```

---

## How to Run and Test

1. Navigate to the project directory:
   ```bash
   cd phase-02-web-api-basics/task-01-rest-routing-drills/ApiRoutingDrills
   ```
2. Run the application:
   ```bash
   dotnet run
   ```
3. Open your browser to the root URL (configured to open Swagger UI):
   - `http://localhost:<port>/` or `https://localhost:<port>/`
4. Test all endpoints interactively through Swagger UI or import them into Postman.
