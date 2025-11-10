USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Descuento', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Descuento (
        IdDescuento INT IDENTITY(1,1) PRIMARY KEY,
        Descuento DECIMAL(5,2) NOT NULL
    );
END;
GO

Use SistemaGestionFacturacion
IF COL_LENGTH('dbo.Descuento', 'Descripcion') IS NULL
BEGIN
    ALTER TABLE dbo.Descuento
    ADD Descripcion NVARCHAR(200) NOT NULL CONSTRAINT DF_Descuento_Descripcion DEFAULT ('Sin Descripción');
END;
GO

USE  SistemaGestionFacturacion
IF COL_LENGTH('dbo.Descuento', 'Estado') IS NULL
    BEGIN
        ALTER TABLE dbo.Descuento
        ADD Estado VARCHAR(15) NOT NULL DEFAULT ('Activo');
       
END;

SELECT * FROM dbo.Descuento

    