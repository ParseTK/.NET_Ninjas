USE [SalesLedgerDb]
GO

-- Insert Customers
INSERT INTO Customers (CustomerId, FirstName, LastName, Email)
VALUES 
(NEWID(), 'John',   'Doe',   'john.doe@example.com'),
(NEWID(), 'Sarah',  'Smith', 'sarah.smith@example.com'),
(NEWID(), 'Michael','Brown', 'michael.brown@example.com');
GO

-- Insert Products
INSERT INTO Products (ProductId, Name, Price, Description)
VALUES 
(NEWID(), 'Laptop 15-inch', 1200.00, NULL),
(NEWID(), 'Wireless Mouse',   25.50, NULL),
(NEWID(), 'Keyboard',         45.99, NULL);
GO

-- Insert Orders + OrderItems 
DECLARE @JohnId UNIQUEIDENTIFIER = (SELECT TOP 1 CustomerId FROM Customers WHERE Email = 'john.doe@example.com');
DECLARE @SarahId UNIQUEIDENTIFIER = (SELECT TOP 1 CustomerId FROM Customers WHERE Email = 'sarah.smith@example.com');

DECLARE @LaptopId UNIQUEIDENTIFIER = (SELECT TOP 1 ProductId FROM Products WHERE Name = 'Laptop 15-inch');
DECLARE @MouseId UNIQUEIDENTIFIER = (SELECT TOP 1 ProductId FROM Products WHERE Name = 'Wireless Mouse');
DECLARE @KeyboardId UNIQUEIDENTIFIER = (SELECT TOP 1 ProductId FROM Products WHERE Name = 'Keyboard');

-- Order 1
DECLARE @Order1Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (OrderId, CustomerId, OrderDate) 
VALUES (@Order1Id, @JohnId, '2025-01-10');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES (@Order1Id, @LaptopId, 1, 1200.00);

-- Order 2
DECLARE @Order2Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (OrderId, CustomerId, OrderDate) 
VALUES (@Order2Id, @JohnId, '2025-01-11');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES (@Order2Id, @MouseId, 2, 25.50);

-- Order 3
DECLARE @Order3Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (OrderId, CustomerId, OrderDate) 
VALUES (@Order3Id, @SarahId, '2025-01-12');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES (@Order3Id, @KeyboardId, 1, 45.99);
GO

PRINT 'Database seeded successfully!'