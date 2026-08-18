-- ==========================================================
-- Scenario A: Library Management System - Required Queries
-- ==========================================================

-- 1. Select all books.
SELECT BookId, Title, ISBN, PublishedYear, AvailableCopies, AuthorId, CategoryId 
FROM Books;

-- 2. Select all active members.
SELECT MemberId, FullName, Email, PhoneNumber, JoinDate 
FROM Members 
WHERE IsActive = 1;

-- 3. Select books by category (Assuming CategoryId = 1)
SELECT BookId, Title, ISBN, AvailableCopies 
FROM Books 
WHERE CategoryId = 1;

-- 4. Count books per category.
SELECT c.CategoryId, c.Name AS CategoryName, COUNT(b.BookId) AS BookCount
FROM Categories c
LEFT JOIN Books b ON c.CategoryId = b.CategoryId
GROUP BY c.CategoryId, c.Name;

-- 5. Select borrow records with member name and book title using JOIN.
SELECT 
    br.BorrowRecordId,
    m.FullName AS MemberName,
    b.Title AS BookTitle,
    br.BorrowDate,
    br.DueDate,
    br.ReturnDate,
    br.Status
FROM BorrowRecords br
INNER JOIN Members m ON br.MemberId = m.MemberId
INNER JOIN Books b ON br.BookId = b.BookId;

-- 6. Select overdue books
SELECT DISTINCT b.BookId, b.Title, b.ISBN, m.FullName AS BorrowedBy, br.DueDate
FROM Books b
INNER JOIN BorrowRecords br ON b.BookId = br.BookId
INNER JOIN Members m ON br.MemberId = m.MemberId
WHERE br.Status = 'Overdue' 
   OR (br.ReturnDate IS NULL AND br.DueDate < GETDATE());

-- 7. Select borrowing history for one member (Assuming MemberId = 1)
SELECT br.BorrowRecordId, b.Title AS BookTitle, br.BorrowDate, br.DueDate, br.ReturnDate, br.Status
FROM BorrowRecords br
INNER JOIN Books b ON br.BookId = b.BookId
WHERE br.MemberId = 1
ORDER BY br.BorrowDate DESC;

-- 8. Select available books
SELECT BookId, Title, ISBN, AvailableCopies 
FROM Books 
WHERE AvailableCopies > 0;

-- 9. Count how many books each author has.
SELECT a.AuthorId, a.FullName AS AuthorName, COUNT(b.BookId) AS BookCount
FROM Authors a
LEFT JOIN Books b ON a.AuthorId = b.AuthorId
GROUP BY a.AuthorId, a.FullName;

-- 10. Select top 5 most borrowed books
SELECT TOP 5 b.BookId, b.Title, COUNT(br.BorrowRecordId) AS BorrowCount
FROM Books b
INNER JOIN BorrowRecords br ON b.BookId = br.BookId
GROUP BY b.BookId, b.Title
ORDER BY BorrowCount DESC;
