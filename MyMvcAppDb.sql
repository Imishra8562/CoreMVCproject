-- Create Database If Not Exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MyMvcAppDb')
BEGIN
    CREATE DATABASE MyMvcAppDb;
END
GO

-- Use the Database
USE MyMvcAppDb;
GO

-- Drop Tables If They Already Exist
DROP TABLE IF EXISTS UserRoles;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Roles;
DROP TABLE IF EXISTS Products;
GO

-- Create Products Table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL
);

-- Create Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL, -- Store hashed passwords
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- Create Roles Table
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL
);

-- Create UserRoles Table (Many-to-Many Relationship)
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id) ON DELETE CASCADE
);

  CREATE VIEW UserRolesWithRoleName AS
SELECT 
    ur.UserId, 
    ur.RoleId, 
    r.Name AS RoleName
FROM 
    UserRoles ur
JOIN 
    Roles r ON ur.RoleId = r.Id;

SELECT * FROM UserRolesWithRoleName;

SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Role';
