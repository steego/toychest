-- Create a new database called 'Prodducts'
-- Connect to the 'master' database to run this snippet
USE master
GO
-- Create the new database if it does not exist already
IF NOT EXISTS (
    SELECT name
        FROM sys.databases
        WHERE name = N'Products'
)
CREATE DATABASE Products
GO

use Products
go

-- Create a new table called 'Products' in schema 'SchemaName'
-- Drop the table if it already exists
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL
DROP TABLE dbo.Product
GO
-- Create the table in the specified schema
CREATE TABLE dbo.Product
(
    [ProductId] INT NOT NULL PRIMARY KEY, -- primary key column
    [Name] [NVARCHAR](50) NOT NULL
);
GO

insert into dbo.Product 
    ([ProductId], [Name])
values
    (1, 'Socks'),
    (2, 'Black Shoes')
go

select * from dbo.Product