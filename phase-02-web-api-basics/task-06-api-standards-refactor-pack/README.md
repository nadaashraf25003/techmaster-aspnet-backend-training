# Task 06 - API Standards & Refactor Pack

**TechMaster Academy | ASP.NET Backend Career Training**  
**Phase 02:** Web API Basics

---

##  Task Purpose

The objective of this task is to recognize anti-patterns and bad practices in ASP.NET Core Web API design and refactor messy, unlayered legacy code into an enterprise-grade, clean architecture following RESTful principles, strong typing, proper HTTP status codes, Data Transfer Objects (DTOs), and separation of concerns via a dedicated service layer.

---

##  Original Problems

The legacy implementation in [`OriginalBadCode/ProductsController.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/OriginalBadCode/ProductsController.cs) suffered from multiple severe architectural and stylistic defects:

1. **Controller Contained Storage and State:**
   - A `static List<Product>` was held directly inside the controller class, causing state coupling, potential concurrency issues, and untestable code.
2. **Missing Service Layer / Business Logic in Controller:**
   - Validation, ID generation, filtering loops, and persistence operations were all embedded directly within controller actions.
3. **Improper POST Parameter Handling:**
   - The `POST` endpoint accepted multiple loose string/primitive arguments (`string name, decimal price, int stock`) rather than binding a structured, encapsulated Request DTO from the HTTP request body (`[FromBody]`).
4. **False 200 OK Status Code for Errors:**
   - Validation failures (such as empty name or negative price) returned `Ok("bad name")` and `Ok("bad price")` (HTTP 200) instead of standard client error codes (`400 Bad Request`).
5. **False 200 OK Status Code for Missing Resources:**
   - When a requested ID was not found, the controller returned `Ok("not found")` with HTTP 200 instead of `404 Not Found`.
6. **Non-RESTful Route Naming (RPC-Style):**
   - Routes were named with arbitrary action verbs like `[HttpGet("all")]` and `[HttpGet("get")]` rather than utilizing RESTful resource URIs (`GET /api/products` and `GET /api/products/{id}`).
7. **Domain Entity Exposure with Public Fields:**
   - The `Product` class used public mutable fields (`public int Id; public string Name; ...`) rather than standard C# auto-properties (`{ get; set; }`), violating basic object-oriented encapsulation.
8. **Inconsistent and Unstructured Response Shapes:**
   - The controller mixed data objects (`Ok(p)`), collections (`Ok(products)`), and plain error strings (`Ok("bad name")`), resulting in unpredictable payloads that break frontend/client contracts.

---

##  Improvements Made (Refactoring Breakdown)

The refactored solution in [`RefactoredApi/`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/) introduces the following 10 comprehensive architectural enhancements:

1. **Domain Model with C# Properties:**
   - Converted the `Product` entity to use typed C# properties with encapsulation (`Id`, `Name`, `Price`, `Stock`) within [`Models/Product.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/Models/Product.cs).
2. **Created Request DTO (`CreateProductRequest`):**
   - Introduced [`DTOs/CreateProductRequest.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/DTOs/CreateProductRequest.cs) to strongly bind incoming JSON payloads with DataAnnotation validation rules (`[Required]`, `[StringLength]`, `[Range]`).
3. **Created Response DTO (`ProductResponse`):**
   - Introduced [`DTOs/ProductResponse.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/DTOs/ProductResponse.cs) to provide an explicit, versionable contract decoupled from internal database representations.
4. **Extracted Service Interface (`IProductService`):**
   - Defined a clean contract in [`Services/IProductService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/Services/IProductService.cs) specifying business operations (`GetAll`, `GetById`, `Create`).
5. **Decoupled Business Logic into `ProductService`:**
   - Implemented state management, ID generation, thread-safety, and model-to-DTO mappings inside [`Services/ProductService.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/Services/ProductService.cs).
6. **Dependency Injection (DI) Registration:**
   - Registered `IProductService` in ASP.NET Core's built-in IoC container (`builder.Services.AddSingleton<IProductService, ProductService>()`), adhering to the Dependency Inversion Principle.
7. **Adherence to RESTful URI Standards:**
   - Replaced `GET /api/products/all` with `GET /api/products`.
   - Replaced `GET /api/products/get?id=1` with `GET /api/products/{id}` using standard route parameters and type constraints (`{id:int}`).
   - Replaced loose POST query params with `POST /api/products` accepting JSON in the HTTP request body.
8. **Semantic HTTP Status Codes:**
   - Returns **`201 Created`** with a `Location` header (`CreatedAtAction`) upon successful resource creation.
   - Returns **`400 Bad Request`** with structured JSON error messages when request validation fails.
   - Returns **`404 Not Found`** with a structured message when a product is not found.
   - Returns **`200 OK`** for successful retrieval requests.
9. **Thin Controller Pattern:**
   - Reduced [`Controllers/ProductsController.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi/Controllers/ProductsController.cs) to a concise coordinator that handles HTTP routing, binds inputs, triggers service methods, and formats HTTP responses.
10. **Swagger / OpenAPI Documentation:**
    - Configured Swashbuckle OpenAPI with route metadata and `[ProducesResponseType]` annotations for all endpoints.

---

##  Architecture & Structural Comparison

```
┌──────────────────────────────────────────────────┐      ┌─────────────────────────────────────────────────────────┐
│              ORIGINAL (ANTI-PATTERN)             │      │               REFACTORED (CLEAN ARCHITECTURE)           │
├──────────────────────────────────────────────────┤      ├─────────────────────────────────────────────────────────┤
│                                                  │      │                     HTTP Request                        │
│                     HTTP Request                 │      │                          │                              │
│                          │                       │      │                          ▼                              │
│                          ▼                       │      │             ┌─────────────────────────┐                 │
│         ┌──────────────────────────────────┐     │      │             │   ProductsController    │                 │
│         │        ProductsController        │     │      │             └────────────┬────────────┘                 │
│         │ ──────────────────────────────── │     │      │                          │ Uses IProductService         │
│         │ • Public fields                  │     │      │                          ▼                              │
│         │ • Static in-memory list          │     │      │             ┌─────────────────────────┐                 │
│         │ • String input arguments         │     │      │             │     ProductService      │                 │
│         │ • Business logic & validation    │     │      │             └────────────┬────────────┘                 │
│         │ • Non-REST route names           │     │      │                          │ Maps DTOs & Models           │
│         │ • 200 OK for errors / 404s       │     │      │       ┌──────────────────┴──────────────────┐           │
│         └──────────────────────────────────┘     │      │       ▼                                     ▼           │
│                          │                       │      │  ┌───────────┐                       ┌──────────────┐   │
│                          ▼                       │      │  │  Product  │ (Domain Entity)       │ DTOs (In/Out)│   │
│                     HTTP Response                │      │  └───────────┘                       └──────────────┘   │
│                 (Inconsistent Shape)             │      │                          │                              │
│                                                  │      │                          ▼                              │
│                                                  │      │               Structured HTTP Response                  │
│                                                  │      │                 (200 / 201 / 400 / 404)                 │
└──────────────────────────────────────────────────┘      └─────────────────────────────────────────────────────────┘
```

---

##  Pack Contents & Structure

```text
task-06-api-standards-refactor-pack/
├── OriginalBadCode/
│   └── ProductsController.cs                              # Original messy legacy code
├── RefactoredApi/
│   ├── Controllers/
│   │   └── ProductsController.cs                          # Clean, thin REST controller
│   ├── DTOs/
│   │   ├── CreateProductRequest.cs                        # Input DTO with validation attributes
│   │   └── ProductResponse.cs                             # Output DTO contract
│   ├── Models/
│   │   └── Product.cs                                     # Domain entity with proper properties
│   ├── Properties/
│   │   └── launchSettings.json                            # Local development launch configuration
│   ├── Services/
│   │   ├── IProductService.cs                             # Service contract interface
│   │   └── ProductService.cs                              # Business logic & thread-safe storage
│   ├── appsettings.json                                   # Application configuration
│   ├── Program.cs                                         # Dependency injection & pipeline setup
│   └── RefactoredApi.csproj                               # .NET 10 project file
└── README.md                                              # This refactoring analysis and guide
```

---

##  Endpoints & Status Code Mapping

| Operation | Legacy Route | Refactored Route | Method | Payload / Params | Status Code (Success) | Status Code (Failure) |
| :--- | :--- | :--- | :---: | :--- | :---: | :---: |
| **Get All Products** | `/api/products/all` | `/api/products` | `GET` | *None* | `200 OK` | — |
| **Get Product by ID** | `/api/products/get?id={id}` | `/api/products/{id}` | `GET` | Route `id` (int) | `200 OK` | `404 Not Found` |
| **Create Product** | `/api/products?name=...` | `/api/products` | `POST` | JSON Body (`CreateProductRequest`) | `201 Created` | `400 Bad Request` |

---

##  What I Learned

> "Refactoring legacy code into a clean, layered architecture illustrates why software design principles are vital in production environments. Separating data transfer objects (DTOs) from internal models prevents accidental over-posting and secures internal data representations. Moving state and logic out of the controller into a dedicated service layer adheres to the Single Responsibility Principle and unlocks unit testability. Finally, using precise HTTP status codes (such as `201 Created`, `400 Bad Request`, and `404 Not Found`) transforms an unpredictable API into a reliable, self-describing RESTful contract that client applications can safely consume."

---

##  How to Run and Test

### 1. Build and Run
```bash
# Navigate to the refactored project directory:
cd phase-02-web-api-basics/task-06-api-standards-refactor-pack/RefactoredApi

# Run the API:
dotnet run
```
The application starts at `http://localhost:5280` (or the port specified in your console).

### 2. Access Swagger UI
Open your browser and navigate to:
```text
http://localhost:5280/swagger
```

### 3. Test with cURL

#### Create a Product (Success - 201 Created):
```bash
curl -X POST "http://localhost:5280/api/products" \
  -H "Content-Type: application/json" \
  -d '{"name": "Mechanical Keyboard", "price": 89.99, "stock": 25}'
```

#### Create a Product with Invalid Data (Validation Error - 400 Bad Request):
```bash
curl -X POST "http://localhost:5280/api/products" \
  -H "Content-Type: application/json" \
  -d '{"name": "", "price": -10, "stock": -5}'
```

#### Get All Products (200 OK):
```bash
curl -X GET "http://localhost:5280/api/products"
```

#### Get Existing Product by ID (200 OK):
```bash
curl -X GET "http://localhost:5280/api/products/1"
```

#### Get Non-Existent Product (404 Not Found):
```bash
curl -X GET "http://localhost:5280/api/products/999"
```

---

##  Acceptance Criteria Checklist

- [x] Original bad code is preserved in [`OriginalBadCode/ProductsController.cs`](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-06-api-standards-refactor-pack/OriginalBadCode/ProductsController.cs).
- [x] Refactored version compiles and runs cleanly with zero errors.
- [x] Controller is small, clean, and acts only as an HTTP coordinator.
- [x] Domain model uses standard C# properties with encapsulation.
- [x] `CreateProductRequest` and `ProductResponse` DTOs are created and used.
- [x] `IProductService` interface and `ProductService` implementation exist with DI registration.
- [x] HTTP status codes are corrected (`201`, `200`, `400`, `404`).
- [x] Route names follow REST naming conventions without RPC verbs (`all`, `get`).
- [x] README explains at least 8 distinct improvements in detail.
- [x] 5-8 line reflection section on the value of refactoring included.
