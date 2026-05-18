CREATE DATABASE MultiVendorDB;
GO
USE MultiVendorDB;
GO

CREATE TABLE Users (
    UserID      INT PRIMARY KEY IDENTITY(1,1),
    Username    NVARCHAR(50)  NOT NULL UNIQUE,
    Password    NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(100),
    FullName    NVARCHAR(100),
    Phone       NVARCHAR(20),
    Role        NVARCHAR(20)  NOT NULL DEFAULT 'Customer',
    IsActive    BIT           NOT NULL DEFAULT 1,
    CreatedDate DATETIME      DEFAULT GETDATE()
);

CREATE TABLE Vendors (
    VendorID    INT PRIMARY KEY IDENTITY(1,1),
    UserID      INT FOREIGN KEY REFERENCES Users(UserID),
    StoreName   NVARCHAR(100),
    Description NVARCHAR(300),
    Phone       NVARCHAR(20),
    Address     NVARCHAR(200),
    IsApproved  BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Customers (
    CustomerID  INT PRIMARY KEY IDENTITY(1,1),
    UserID      INT FOREIGN KEY REFERENCES Users(UserID),
    FullName    NVARCHAR(100),
    Email       NVARCHAR(100),
    Phone       NVARCHAR(20),
    Address     NVARCHAR(200),
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categories (
    CategoryID   INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(200)
);

CREATE TABLE Products (
    ProductID   INT PRIMARY KEY IDENTITY(1,1),
    VendorID    INT FOREIGN KEY REFERENCES Vendors(VendorID),
    CategoryID  INT FOREIGN KEY REFERENCES Categories(CategoryID),
    Name        NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500),
    Price       DECIMAL(10,2) NOT NULL,
    Stock       INT           NOT NULL DEFAULT 0,
    IsActive    BIT           NOT NULL DEFAULT 1,
    CreatedDate DATETIME      DEFAULT GETDATE()
);

CREATE TABLE Orders (
    OrderID         INT PRIMARY KEY IDENTITY(1,1),
    CustomerID      INT FOREIGN KEY REFERENCES Customers(CustomerID),
    TotalAmount     DECIMAL(12,2),
    Status          NVARCHAR(30) DEFAULT 'Pending',
    PaymentMethod   NVARCHAR(50),
    ShippingAddress NVARCHAR(300),
    OrderDate       DATETIME DEFAULT GETDATE()
);

CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID     INT FOREIGN KEY REFERENCES Orders(OrderID),
    ProductID   INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity    INT           NOT NULL,
    Price       DECIMAL(10,2) NOT NULL
);

CREATE TABLE Reviews (
    ReviewID   INT PRIMARY KEY IDENTITY(1,1),
    ProductID  INT FOREIGN KEY REFERENCES Products(ProductID),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    Rating     INT  NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment    NVARCHAR(500),
    ReviewDate DATETIME DEFAULT GETDATE()
);

-- Default Users
INSERT INTO Users (Username,Password,Email,FullName,Role,IsActive)
VALUES ('admin','admin123','admin@shop.com','Administrator','Admin',1);

INSERT INTO Users (Username,Password,Email,FullName,Role,IsActive)
VALUES ('vendor','vendor123','vendor@shop.com','Tech Vendor','Vendor',1);

INSERT INTO Users (Username,Password,Email,FullName,Role,IsActive)
VALUES ('customer','cust123','customer@shop.com','John Customer','Customer',1);

INSERT INTO Vendors (UserID,StoreName,Description,Phone,Address,IsApproved)
VALUES (2,'TechZone Store','Best electronics!','9876543210','123 Tech Street',1);

INSERT INTO Customers (UserID,FullName,Email,Phone,Address)
VALUES (3,'John Customer','customer@shop.com','9123456789','456 Customer Lane');

INSERT INTO Categories (CategoryName,Description) VALUES ('Electronics','Gadgets');
INSERT INTO Categories (CategoryName,Description) VALUES ('Clothing','Fashion');
INSERT INTO Categories (CategoryName,Description) VALUES ('Books','Education');
INSERT INTO Categories (CategoryName,Description) VALUES ('Home & Kitchen','Appliances');
INSERT INTO Categories (CategoryName,Description) VALUES ('Sports','Equipment');

INSERT INTO Products (VendorID,CategoryID,Name,Description,Price,Stock,IsActive)
VALUES (1,1,'Wireless Headphones','Bluetooth headphones',2499.00,50,1);
INSERT INTO Products (VendorID,CategoryID,Name,Description,Price,Stock,IsActive)
VALUES (1,1,'Smartphone X200','6.5 inch AMOLED',18999.00,30,1);
INSERT INTO Products (VendorID,CategoryID,Name,Description,Price,Stock,IsActive)
VALUES (1,2,'Cotton T-Shirt','Premium cotton',499.00,200,1);
INSERT INTO Products (VendorID,CategoryID,Name,Description,Price,Stock,IsActive)
VALUES (1,3,'Python Book','Complete Python guide',449.00,60,1);
INSERT INTO Products (VendorID,CategoryID,Name,Description,Price,Stock,IsActive)
VALUES (1,4,'Electric Kettle','1.5L kettle',1199.00,45,1);
GO