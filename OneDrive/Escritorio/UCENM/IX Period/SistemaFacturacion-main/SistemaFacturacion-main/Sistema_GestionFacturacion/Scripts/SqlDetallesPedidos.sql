USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.DetallesPedidos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DetallesPedidos (
        IdDetallePedido INT IDENTITY(1,1) PRIMARY KEY,
        IdPedido INT NOT NULL,
        IdProducto INT NOT NULL,
        Categoria NVARCHAR(150) NULL,   
        Marca NVARCHAR(100) NULL,       
        Modelo NVARCHAR(100) NULL,      
        Precio DECIMAL(18,2) NOT NULL,  
        Cantidad INT NOT NULL,
        Subtotal DECIMAL(18,2) NOT NULL, 
        ISV DECIMAL(18,2) NOT NULL,     
        Total DECIMAL(18,2) NOT NULL,   
        CONSTRAINT FK_DetallesPedidos_Pedidos FOREIGN KEY (IdPedido)
            REFERENCES dbo.Pedidos (IdPedido)
            ON DELETE CASCADE, -- al eliminar un pedido se eliminan sus detalles
        CONSTRAINT FK_DetallesPedidos_Productos FOREIGN KEY (IdProducto)
            REFERENCES dbo.Productos (IdProducto)
            ON DELETE NO ACTION
    );
END;
GO




