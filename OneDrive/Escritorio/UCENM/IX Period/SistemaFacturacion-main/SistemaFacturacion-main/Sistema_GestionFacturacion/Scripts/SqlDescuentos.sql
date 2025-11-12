USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Descuento', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Descuento (
        IdDescuento INT IDENTITY(1,1) PRIMARY KEY,
        Descuento DECIMAL(5,2) NOT NULL,
        Descripcion NVARCHAR(200) NOT NULL CONSTRAINT DF_Descuento_Descripcion DEFAULT ('Sin Descripción'),
        Estado VARCHAR(15) NOT NULL DEFAULT ('Activo')

    );
END;
GO
SELECT * FROM dbo.Descuento

    