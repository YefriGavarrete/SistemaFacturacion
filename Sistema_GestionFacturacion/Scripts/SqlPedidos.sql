USE SistemaGestionFacturacion    

IF OBJECT_ID('dbo.Pedidos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pedidos (
        IdPedido INT IDENTITY(1,1) PRIMARY KEY,
        NombreCliente NVARCHAR(100) NOT NULL,
        ApellidoCliente NVARCHAR(100) NOT NULL,
        DNI NVARCHAR(20) NULL,
        Fecha DATETIME NOT NULL DEFAULT (GETDATE()),
        IdEmpleado INT NULL, --
        NombreEmpleado NVARCHAR(100) NULL, 
        ApellidoEmpleado NVARCHAR(100) NULL, 
        Estado NVARCHAR(15) NOT NULL DEFAULT ('Activo'),
        CONSTRAINT FK_Pedidos_Empleados FOREIGN KEY (IdEmpleado)
            REFERENCES dbo.Empleados (IdEmpleado)
            ON UPDATE NO ACTION
            ON DELETE SET NULL
    );
END;
GO
