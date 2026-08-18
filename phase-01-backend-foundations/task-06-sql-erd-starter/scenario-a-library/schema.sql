-- ==========================================================
-- Scenario A: Library Management System - Schema Definitions
-- ==========================================================

-- Drop existing tables to ensure clean run
IF OBJECT_ID('dbo.BorrowRecords', 'U') IS NOT NULL DROP TABLE dbo.BorrowRecords;
IF OBJECT_ID('dbo.Books', 'U') IS NOT NULL DROP TABLE dbo.Books;
IF OBJECT_ID('dbo.Members', 'U') IS NOT NULL DROP TABLE dbo.Members;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Authors', 'U') IS NOT NULL DROP TABLE dbo.Authors;

CREATE TABLE Authors (
    AuthorId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    BirthDate DATE NULL,
    Country NVARCHAR(100) NULL
);

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL
);

CREATE TABLE Books (
    BookId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    ISBN NVARCHAR(20) NULL UNIQUE,
    PublishedYear INT NULL CHECK (PublishedYear >= 0 AND PublishedYear <= YEAR(GETDATE())),
    AvailableCopies INT NOT NULL CHECK (AvailableCopies >= 0),
    AuthorId INT NOT NULL FOREIGN KEY REFERENCES Authors(AuthorId),
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId)
);

CREATE TABLE Members (
    MemberId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NULL,
    JoinDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE BorrowRecords (
    BorrowRecordId INT IDENTITY(1,1) PRIMARY KEY,
    BookId INT NOT NULL FOREIGN KEY REFERENCES Books(BookId),
    MemberId INT NOT NULL FOREIGN KEY REFERENCES Members(MemberId),
    BorrowDate DATETIME NOT NULL DEFAULT GETDATE(),
    DueDate DATETIME NOT NULL,
    ReturnDate DATETIME NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Borrowed' CHECK (Status IN ('Borrowed', 'Returned', 'Overdue'))
);
