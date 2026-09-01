# Task 07 - Interview Answers & API Explanation Pack

**TechMaster Academy | ASP.NET Backend Career Training**  
**Phase 02:** Web API Basics

---

##  Overview

This folder contains the complete, high-quality answers to the **Phase 02 Technical Interview & Architectural Questions**. It is formatted as:
1. **[Phase02_Interview_Answers.tex](file:///c:/Users/user/Desktop/Tech_Master/techmaster-aspnet-backend-training/phase-02-web-api-basics/task-07-interview-answers/Phase02_Interview_Answers.tex)**: Ready-to-compile LaTeX document using modern styling (Slate & Sky Blue palette, custom `answer` environment, and C# syntax highlighting).
2. **PDF Compilation Instructions**: See below.

---

##  How to Compile LaTeX to PDF

You can compile `Phase02_Interview_Answers.tex` using any of the following methods:

### Option A: Online with Overleaf (Easiest)
1. Open [Overleaf](https://www.overleaf.com).
2. Create a new blank project.
3. Paste the contents of `Phase02_Interview_Answers.tex` into `main.tex`.
4. Click **Recompile** and download your PDF.

### Option B: Local CLI (pdflatex / latexmk)
```bash
pdflatex Phase02_Interview_Answers.tex
```

---

##  Questions & Concepts Covered

1. **REST & HTTP**: REST definition in Web APIs, HTTP verbs (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`), HTTP status codes (`200`, `201`, `204`, `400`, `404`).
2. **Routing & Controllers**: Route vs. Query parameters, Controller responsibilities in ASP.NET Core.
3. **DTOs & Services**: Benefits of DTOs over raw entities, Separation of Concerns & Business Logic in Services.
4. **Dependency Injection**: IoC containers and constructor injection in ASP.NET Core.
5. **Validation**: Data Annotations on DTOs vs. Business validation in Service layers.
6. **Tooling**: Swagger/OpenAPI vs. Postman, consistent response shapes.
7. **Search, Filtering & Pagination**: Implementation with LINQ, `Skip()`, and `Take()`.
8. **Architecture & Engineering**: UI / Controller / Service / Data Storage layers, Git commit standards, debugging 500 errors, defensive security, and why in-memory storage requires migration to SQL Server in Phase 03.
