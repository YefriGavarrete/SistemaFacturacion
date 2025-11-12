
USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Cargos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cargos (
        IdCargo INT IDENTITY(1,1) PRIMARY KEY,
        Cargo NVARCHAR(100) NOT NULL UNIQUE,
        Estado VARCHAR(15) NOT NULL DEFAULT ('Activo')
    );
END;
GO

USE  SistemaGestionFacturacion
select * from dbo.Cargos