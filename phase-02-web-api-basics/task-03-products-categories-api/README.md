# Task 03 - Products & Categories API

## Overview
The **Products & Categories API** is a multi-resource ASP.NET Core Web API designed for store inventory management. It models related resources (**Categories** and **Products**), enforces cross-resource business validation, provides granular multi-criteria filtering, and generates advanced stock valuation reports using LINQ aggregations.

---

## Domain Models & Business Rules

### 1. Category Model
- `CategoryId` (int, auto-generated unique identifier)
- `Name` (string, required, unique, length 2-100)
- `Description` (string, optional, max 500 chars)
- `IsActive` (bool, default true)
- `CreatedAt` (DateTime, UTC timestamp)

### 2. Product Model
- `ProductId` (int, auto-generated unique identifier)
- `Name` (string, required, length 2-150)
- `CategoryId` (int, required foreign key referencing a valid Category)
- `Price` (decimal, required, strictly positive `> 0`)
- `StockQuantity` (int, required, non-negative `>= 0`)
- `IsAvailable` (bool, default true; automatically false if stock is 0)
- `SupplierName` (string, required, length 2-100)
- `CreatedAt` (DateTime, UTC timestamp)
- `UpdatedAt` (DateTime?, UTC timestamp on update)

### Critical Business Rules
1. **Cross-Resource Category Validation**: Before creating or updating any product, the API verifies that `CategoryId` exists. If invalid, the request is rejected with `400 Bad Request`.
2. **Category Name Uniqueness**: Category names must be unique across the catalog (case-insensitive).
3. **Protected Category Deletion**: Attempting to delete a category that currently has active products will return `400 Bad Request`, preventing orphan product records.
4. **Price & Stock Constraints**: Prices must be strictly positive (`> 0`) and stock quantities cannot be negative (`>= 0`).
5. **Default Category Inactive Filter**: Inactive categories are filtered out by default on `GET /api/categories` unless `includeInactive=true` is explicitly requested.

---

## Seed Data Summary
The in-memory database is pre-seeded with **4 categories** and **16 products** across various stock categories (healthy, low stock <= 5, and out of stock = 0):

| Category ID | Category Name | Seed Products Count | Sample Products |
|---|---|---|---|
| **1** | **Electronics** | 6 | Pro Laptop 16", Wireless Mouse (Low Stock), Gaming Keyboard, 4K Monitor (Out of Stock), 7-in-1 USB Hub, Earbuds |
| **2** | **Furniture** | 3 | Ergonomic Mesh Chair (Low Stock), Standing Desk, LED Desk Lamp |
| **3** | **Stationery** | 4 | Dotted Grid Journal, Gel Pens 10pk (Low Stock), Highlighters 6pk, Copy Paper (Out of Stock) |
| **4** | **Accessories** | 4 | Waterproof Backpack, Mouse Pad, Laptop Sleeve 15.6" (Low Stock), Cable Organizer |

---

## API Endpoints & Routes

### Categories Endpoints
| HTTP Method | Route | Description | Status Codes |
|---|---|---|---|
| `GET` | `/api/categories` | Returns active categories (or all if `includeInactive=true`) | `200 OK` |
| `GET` | `/api/categories/{id}` | Returns category details with product count | `200 OK`, `404 Not Found` |
| `POST` | `/api/categories` | Creates a new category (unique name check) | `201 Created`, `400 Bad Request` |
| `PUT` | `/api/categories/{id}` | Updates an existing category | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/categories/{id}` | Deletes a category (blocked if it contains products) | `200 OK`, `400 Bad Request`, `404 Not Found` |

### Products Endpoints
| HTTP Method | Route | Description | Status Codes |
|---|---|---|---|
| `GET` | `/api/products` | Returns products with search, category, price range, availability, and low-stock filters | `200 OK` |
| `GET` | `/api/products/{id}` | Returns single product details including category name | `200 OK`, `404 Not Found` |
| `POST` | `/api/products` | Creates a product after category existence & price/stock validation | `201 Created`, `400 Bad Request` |
| `PUT` | `/api/products/{id}` | Updates product information with category validation | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `PATCH` | `/api/products/{id}/stock` | Updates stock quantity and adjusts availability status | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/products/{id}` | Deletes a product from the inventory | `200 OK`, `404 Not Found` |
| `GET` | `/api/products/low-stock` | Returns all products where stock &le; threshold (default 5) | `200 OK` |
| `GET` | `/api/products/reports/stock-value` | Generates total stock valuation, category breakdown, and low/out-of-stock reports | `200 OK` |

---

## Request & Response Samples

### 1. Create Product with Category Validation (`POST /api/products`)
**Request Body**:
```json
{
  "name": "Mechanical Gaming Keyboard",
  "categoryId": 1,
  "price": 89.99,
  "stockQuantity": 18,
  "isAvailable": true,
  "supplierName": "KeyCraft Hardware"
}
```
**Response (`201 Created`)**:
```json
{
  "productId": 3,
  "name": "Mechanical Gaming Keyboard",
  "categoryId": 1,
  "categoryName": "Electronics",
  "price": 89.99,
  "stockQuantity": 18,
  "isAvailable": true,
  "supplierName": "KeyCraft Hardware",
  "createdAt": "2026-04-30T19:52:24.1045902Z",
  "updatedAt": null
}
```

### 2. Multi-Criteria Product Search (`GET /api/products?categoryId=1&minPrice=40&maxPrice=200`)
**Response (`200 OK`)**:
```json
[
  {
    "productId": 3,
    "name": "Mechanical Gaming Keyboard",
    "categoryId": 1,
    "categoryName": "Electronics",
    "price": 89.99,
    "stockQuantity": 18,
    "isAvailable": true,
    "supplierName": "KeyCraft Hardware",
    "createdAt": "2026-04-30T19:52:24.1045902Z",
    "updatedAt": null
  },
  {
    "productId": 5,
    "name": "7-in-1 USB-C Hub Multiport Adapter",
    "categoryId": 1,
    "categoryName": "Electronics",
    "price": 45.00,
    "stockQuantity": 25,
    "isAvailable": true,
    "supplierName": "AnkerPro Accessories",
    "createdAt": "2026-06-30T19:52:24.1045908Z",
    "updatedAt": null
  },
  {
    "productId": 6,
    "name": "Active Noise Cancelling Earbuds",
    "categoryId": 1,
    "categoryName": "Electronics",
    "price": 129.99,
    "stockQuantity": 8,
    "isAvailable": true,
    "supplierName": "SoundWave Audio",
    "createdAt": "2026-06-30T19:52:24.104591Z",
    "updatedAt": null
  }
]
```

### 3. Stock Valuation Report (`GET /api/products/reports/stock-value`)
**Response (`200 OK`)**:
```json
{
  "totalStockValue": 38433.18,
  "totalProducts": 18,
  "totalUnitsInStock": 324,
  "lowStockProductsCount": 3,
  "outOfStockProductsCount": 2,
  "stockValuePerCategory": [
    {
      "categoryId": 1,
      "categoryName": "Electronics",
      "totalProducts": 7,
      "totalUnitsInStock": 136,
      "totalCategoryValue": 30483.89
    },
    {
      "categoryId": 2,
      "categoryName": "Furniture",
      "totalProducts": 3,
      "totalUnitsInStock": 25,
      "totalCategoryValue": 4841.35
    },
    {
      "categoryId": 3,
      "categoryName": "Stationery",
      "totalProducts": 4,
      "totalUnitsInStock": 82,
      "totalCategoryValue": 939.98
    },
    {
      "categoryId": 4,
      "categoryName": "Accessories",
      "totalProducts": 4,
      "totalUnitsInStock": 81,
      "totalCategoryValue": 2167.96
    }
  ],
  "lowStockProducts": [
    { "productId": 7, "name": "Ergonomic Mesh Office Chair", "stockQuantity": 3, "categoryName": "Furniture" },
    { "productId": 11, "name": "Gel Ink Rollerball Pens (10-pack)", "stockQuantity": 2, "categoryName": "Stationery" },
    { "productId": 16, "name": "Shockproof Padded Laptop Sleeve 15.6", "stockQuantity": 5, "categoryName": "Accessories" }
  ],
  "outOfStockProducts": [
    { "productId": 4, "name": "4K UHD Monitor 27-inch", "stockQuantity": 0, "isAvailable": false, "categoryName": "Electronics" },
    { "productId": 13, "name": "Premium A4 Multipurpose Copy Paper", "stockQuantity": 0, "isAvailable": false, "categoryName": "Stationery" }
  ]
}
```

---

## How to Run & Test

### Run with .NET CLI
```bash
cd phase-02-web-api-basics/task-03-products-categories-api/ProductsCategoriesApi
dotnet run
```
Access Swagger UI at:
- `http://localhost:5060/swagger`
- `https://localhost:7060/swagger`

### Test via Postman
1. Open Postman and import `ProductsCategoriesApi.postman_collection.json`.
2. Execute tests in folders `Feature 01 - Category CRUD`, `Feature 02 - Product CRUD`, `Feature 03 - Search and Filters`, and `Feature 04 - Stock Reports`.
