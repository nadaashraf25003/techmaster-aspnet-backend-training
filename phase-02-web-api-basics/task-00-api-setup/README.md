# Task 00 - API Workspace Setup

## Description
Initial workspace setup for ASP.NET Core Web API, featuring controller configuration, Swagger/OpenAPI documentation, and HTTPS redirection.

## Features
- ASP.NET Core Web API (.NET 10.0)
- Controller support enabled (`AddControllers()`, `MapControllers()`)
- OpenAPI / Swagger documentation enabled (`Swashbuckle.AspNetCore`)
- Built-in weather forecast endpoint for initial smoke testing

## How To Run

```bash
cd phase-02-web-api-basics/task-00-api-setup
dotnet run
```

Access Swagger UI:
- `http://localhost:5176/swagger` or `https://localhost:7070/swagger` (or the dynamic port shown in terminal output).
