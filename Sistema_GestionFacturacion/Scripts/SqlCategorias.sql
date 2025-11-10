USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Categorias','U') IS NULL
BEGIN
	CREATE TABLE dbo.Categorias(
	IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
	Categoria NVARCHAR(100) NOT NULL UNIQUE,
	);
END;
GO

USE  SistemaGestionFacturacion
IF COL_LENGTH('dbo.Categorias', 'Estado') IS NULL
    BEGIN
        ALTER TABLE dbo.Categorias
        ADD Estado VARCHAR(15) NOT NULL DEFAULT ('Activo');
       
END;

Use SistemaGestionFacturacion
SELECT * FROM dbo.Cate	gorias

