USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Facturas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Facturas (
        IdFactura INT IDENTITY(1,1) PRIMARY KEY,
        IdPedido INT NOT NULL,
        Fecha DATETIME NOT NULL DEFAULT (GETDATE()),
        NombreEmpleado NVARCHAR(100) NOT NULL,
        ApellidoEmpleado NVARCHAR(100) NOT NULL,
        NombreCliente NVARCHAR(100) NOT NULL,
        ApelidoCliente NVARCHAR(100) NOT NULL,      
        DNI NVARCHAR(20) NOT NULL,
        Total DECIMAL(18, 2) NOT NULL,
        CONSTRAINT FK_Facturas_Pedidos FOREIGN KEY (IdPedido)
            REFERENCES dbo.Pedidos (IdPedido)
            ON DELETE NO ACTION,
    );
END;
GO

