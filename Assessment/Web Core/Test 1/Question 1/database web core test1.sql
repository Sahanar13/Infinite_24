Create database Ekart


CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    PhoneNumber NVARCHAR(20)
);

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(CustomerId),
    OrderDate DATETIME,
    TotalAmount DECIMAL(10, 2)
);

CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY,
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId),
    ProductName NVARCHAR(100),
    Quantity INT,
    UnitPrice DECIMAL(10, 2)
);