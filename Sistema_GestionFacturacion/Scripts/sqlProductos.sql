USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Productos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos (
        IdProducto INT IDENTITY(1,1) PRIMARY KEY,
        Marca NVARCHAR(100) NOT NULL,
        Modelo NVARCHAR(100) NOT NULL,
        Precio DECIMAL(18,2) NOT NULL DEFAULT (0.00),
        Stock INT NOT NULL DEFAULT (0),
        IdCategoria INT NOT NULL,
        IdDescuento INT NULL,
        Descripcion NVARCHAR(500) NULL,
        CONSTRAINT FK_Productos_Categorias FOREIGN KEY (IdCategoria)
            REFERENCES dbo.Categorias (IdCategoria)
            ON UPDATE NO ACTION
            ON DELETE NO ACTION,

        CONSTRAINT FK_Productos_Descuento FOREIGN KEY (IdDescuento)
            REFERENCES dbo.Descuento (IdDescuento)
            ON UPDATE NO ACTION
            ON DELETE SET NULL
    );
END;
GO