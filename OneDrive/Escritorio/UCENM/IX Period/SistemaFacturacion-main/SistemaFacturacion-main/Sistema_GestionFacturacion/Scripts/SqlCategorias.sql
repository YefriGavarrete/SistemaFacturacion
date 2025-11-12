USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Categorias','U') IS NULL
BEGIN
	CREATE TABLE dbo.Categorias(
	IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
	Categoria NVARCHAR(100) NOT NULL UNIQUE,
	Estado VARCHAR(15) NOT NULL DEFAULT ('Activo')
	);
END;
GO

SELECT * FROM dbo.Categorias
