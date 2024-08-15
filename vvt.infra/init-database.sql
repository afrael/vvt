use master;
GO

-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'vvt')
BEGIN
    CREATE DATABASE vvt;
END;
GO

-- Switch to database
USE vvt;
GO

DROP TABLE IF EXISTS FileUploads;

CREATE TABLE FileUploads
(
    id INT IDENTITY(1,1) PRIMARY KEY,
    Run_Id UNIQUEIDENTIFIER NOT NULL,
    Company_Id VARCHAR(20) NULL,
    Company_Code VARCHAR(50) NULL,
    Company_Description VARCHAR(250) NULL,
    Employee_Number VARCHAR(20) NULL,
    Employee_FirstName VARCHAR(250) NULL,
    Employee_LastName VARCHAR(250) NULL,
    Employee_Email VARCHAR(250) NULL,
    Employee_Department VARCHAR(250) NULL,
    Hire_Date VARCHAR(20) NULL,
    Manager_Employee_Number VARCHAR(20) NULL,
    CreatedAt DATETIME NOT NULL
);

ALTER TABLE Employees DROP CONSTRAINT FK_Employee_Company_Id;
DROP TABLE IF EXISTS Companies;

CREATE TABLE Companies
(
    Company_Id INT PRIMARY KEY,
    Company_Code VARCHAR(50) NULL,
    Company_Description VARCHAR(250) NULL,
    CreatedAt DATETIME NOT NULL
);

DROP TABLE IF EXISTS Employees;

CREATE TABLE Employees
(
    id INT IDENTITY(1,1) PRIMARY KEY,
    Employee_Number VARCHAR(20) NULL,
    Employee_FirstName VARCHAR(250) NULL,
    Employee_LastName VARCHAR(250) NULL,
    Employee_Email VARCHAR(250) NULL,
    Employee_Department VARCHAR(250) NULL,
    Hire_Date DATETIME2 NOT NULL,
    Manager_Employee_Number VARCHAR(20) NULL,
    Company_Id INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    CONSTRAINT FK_Employee_Company_Id FOREIGN KEY (Company_Id)
        REFERENCES Companies(Company_Id)
);
