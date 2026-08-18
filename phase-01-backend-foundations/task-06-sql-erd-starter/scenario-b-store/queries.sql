-- ==========================================================
-- Scenario B: Simple Store & Orders System - Required Queries
-- ==========================================================

-- 1. Select all products
SELECT ProductId, Name, Price, StockQuantity, IsAvailable 
FROM Products;

-- 2. Select available products
SELECT ProductId, Name, Price, StockQuantity 
FROM Products 
WHERE IsAvailable = 1;

-- 3. Select products by category (Assuming CategoryId = 1)
SELECT ProductId, Name, Price, StockQuantity 
FROM Products 
WHERE CategoryId = 1;

-- 4. Select products with low stock (e.g., StockQuantity < 5)
SELECT ProductId, Name, StockQuantity 
FROM Products 
WHERE StockQuantity < 5;

-- 5. Select orders for one customer (Assuming CustomerId = 1)
SELECT OrderId, OrderDate, Status, TotalAmount 
FROM Orders 
WHERE CustomerId = 1;

-- 6. Select order details using JOIN
SELECT 
    o.OrderId,
    c.FullName AS CustomerName,
    o.OrderDate,
    o.Status AS OrderStatus,
    p.Name AS ProductName,
    oi.Quantity,
    oi.UnitPrice,
    (oi.Quantity * oi.UnitPrice) AS ItemTotal,
    o.TotalAmount AS OrderGrandTotal
FROM Orders o
INNER JOIN Customers c ON o.CustomerId = c.CustomerId
INNER JOIN OrderItems oi ON o.OrderId = oi.OrderId
INNER JOIN Products p ON oi.ProductId = p.ProductId;

-- 7. Calculate total sales
SELECT SUM(TotalAmount) AS TotalSales 
FROM Orders 
WHERE Status = 'Completed';

-- 8. Count products per category
SELECT c.CategoryId, c.Name AS CategoryName, COUNT(p.ProductId) AS ProductCount
FROM Categories c
LEFT JOIN Products p ON c.CategoryId = p.CategoryId
GROUP BY c.CategoryId, c.Name;

-- 9. Select best-selling products
SELECT p.ProductId, p.Name AS ProductName, SUM(oi.Quantity) AS TotalUnitsSold
FROM Products p
INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
GROUP BY p.ProductId, p.Name
ORDER BY TotalUnitsSold DESC;

-- 10. Select suppliers with their products
SELECT s.SupplierId, s.Name AS SupplierName, p.ProductId, p.Name AS ProductName, p.Price
FROM Suppliers s
LEFT JOIN Products p ON s.SupplierId = p.SupplierId
ORDER BY s.Name, p.Name;
