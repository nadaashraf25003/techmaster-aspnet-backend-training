# Task 04 - Book Store API

## Overview
The Book Store API is a RESTful ASP.NET Core Web API designed for catalog and inventory management of a book store. It models authors, categories, and books, enforces cross-resource validation rules, implements custom query filters with page pagination, and aggregates management report summaries.

---

## Domain Models & Business Rules

### 1. Author Model
- `AuthorId` (int, auto-generated unique identifier)
- `FullName` (string, required)
- `Bio` (string, optional)
- `CreatedAt` (DateTime)

### 2. Category Model
- `CategoryId` (int, auto-generated unique identifier)
- `Name` (string, required, unique, length 2-100)
- `Description` (string, optional, max 500 chars)
- `IsActive` (bool, default true)
- `CreatedAt` (DateTime)

### 3. Book Model
- `BookId` (int, auto-generated unique identifier)
- `Title` (string, required, length 1-200)
- `ISBN` (string, required, unique, max 50 chars)
- `Price` (decimal, required, strictly positive `> 0`)
- `StockQuantity` (int, required, non-negative `>= 0`)
- `AuthorId` (int, required foreign key referencing a valid Author)
- `CategoryId` (int, required foreign key referencing a valid Category)
- `IsAvailable` (bool, computed automatically as true if stock > 0)
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?, UTC timestamp on update)

### Critical Business Rules
1. **Author Deletion Check**: Deleting an author who currently has books in the catalog is blocked and returns `400 Bad Request`.
2. **Category Deletion Check**: Deleting a category that currently contains books is blocked and returns `400 Bad Request`.
3. **Inactive Category Constraint**: Creating or updating a book to refer to an inactive category (where `IsActive` is false) is blocked and returns `400 Bad Request`.
4. **ISBN & Category Name Uniqueness**: ISBN values for books and names for categories must be unique across the catalog (checked case-insensitively).

---

## Seed Data Summary
The in-memory database is pre-seeded with **3 authors**, **4 categories**, and **4 books** for immediate testing:
- **Authors**: George Orwell, J.K. Rowling, J.R.R. Tolkien
- **Categories**: Fiction (Active), Fantasy (Active), Science Fiction (Active), Biography (Inactive)
- **Books**:
  - *1984* (ISBN: 9780451524935, Price: $9.99, Stock: 10, Author: George Orwell, Category: Science Fiction)
  - *Animal Farm* (ISBN: 9780451526342, Price: $7.99, Stock: 0 [Out of Stock], Author: George Orwell, Category: Fiction)
  - *Harry Potter and the Sorcerer's Stone* (ISBN: 9780590353427, Price: $12.99, Stock: 25, Author: J.K. Rowling, Category: Fantasy)
  - *The Hobbit* (ISBN: 9780547928227, Price: $14.99, Stock: 15, Author: J.R.R. Tolkien, Category: Fantasy)

---

## API Endpoints & Routes

### Authors Endpoints
| HTTP Method | Route | Description | Status Codes |
|---|---|---|---|
| `GET` | `/api/authors` | Returns all authors with their book counts | `200 OK` |
| `GET` | `/api/authors/{id}` | Returns details for a single author with book count | `200 OK`, `404 Not Found` |
| `POST` | `/api/authors` | Creates a new author | `201 Created`, `400 Bad Request` |
| `PUT` | `/api/authors/{id}` | Updates an existing author's details | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/authors/{id}` | Deletes an author (blocked if they have associated books) | `200 OK`, `400 Bad Request`, `404 Not Found` |

### Categories Endpoints
| HTTP Method | Route | Description | Status Codes |
|---|---|---|---|
| `GET` | `/api/categories` | Returns active categories (or all if `includeInactive=true`) | `200 OK` |
| `GET` | `/api/categories/{id}` | Returns category details with book count | `200 OK`, `404 Not Found` |
| `POST` | `/api/categories` | Creates a new category (unique name check) | `201 Created`, `400 Bad Request` |
| `PUT` | `/api/categories/{id}` | Updates an existing category | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/categories/{id}` | Deletes a category (blocked if it contains books) | `200 OK`, `400 Bad Request`, `404 Not Found` |

### Books Endpoints
| HTTP Method | Route | Description | Status Codes |
|---|---|---|---|
| `GET` | `/api/books` | Returns a paginated list of books (supports Search, CategoryId, AuthorId, and IsAvailable filters) | `200 OK` |
| `GET` | `/api/books/{id}` | Returns a single book's details including author name and category name | `200 OK`, `404 Not Found` |
| `POST` | `/api/books` | Creates a new book after validating author, category, price, stock, and ISBN | `201 Created`, `400 Bad Request` |
| `PUT` | `/api/books/{id}` | Updates book information with full referential validations | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/books/{id}` | Deletes a book from the catalog | `200 OK`, `404 Not Found` |
| `GET` | `/api/books/reports/summary` | Generates a management summary of the bookstore | `200 OK` |

---

## How to Run & Test

### Run with .NET CLI
```bash
cd phase-02-web-api-basics/task-04-book-store-api/BookStoreApi
dotnet run
```

Access Swagger UI:
- `http://localhost:5242/swagger` or `https://localhost:7242/swagger` (check console outputs for actual port).

### Test via Postman
1. Open Postman and import `BookStoreApi.postman_collection.json`.
2. Run requests from the collections folder structure to verify endpoints.
