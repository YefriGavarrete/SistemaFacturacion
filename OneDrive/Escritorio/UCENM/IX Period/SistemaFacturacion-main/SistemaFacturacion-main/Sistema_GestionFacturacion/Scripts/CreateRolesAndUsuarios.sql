CREATE DATABASE SistemaGestionFacturacion;
GO
USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Roles','U') IS NULL
BEGIN
		CREATE TABLE dbo.Roles (
	    IdRol INT IDENTITY(1,1) PRIMARY KEY,
		Rol NVARCHAR(50) NOT NULL UNIQUE,
		Estado VARCHAR(15) NOT NULL DEFAULT ('Activo')
   );
END;
GO

SELECT * FROM dbo.Roles