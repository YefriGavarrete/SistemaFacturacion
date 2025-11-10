
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

USE SistemaGestionFacturacion

IF OBJECT_ID('dbo.Empleados', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Empleados (
        IdEmpleado INT IDENTITY(1,1) PRIMARY KEY,
        NombreEmpleado NVARCHAR(100) NOT NULL,
        ApellidoEmpleado NVARCHAR(100) NOT NULL,
        IdCargo INT NOT NULL,
        DNI NVARCHAR(20) NULL,
        CONSTRAINT FK_Empleados_Cargos FOREIGN KEY (IdCargo)
            REFERENCES dbo.Cargos (IdCargo)
            ON UPDATE NO ACTION
            ON DELETE NO ACTION
    );
END;
GO

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



USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Descuento', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Descuento (
        IdDescuento INT IDENTITY(1,1) PRIMARY KEY,
        Descuento DECIMAL(5,2) NOT NULL
    );
END;
GO


USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Categorias','U') IS NULL
BEGIN
	CREATE TABLE dbo.Categorias(
	IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
	Categoria NVARCHAR(100) NOT NULL UNIQUE,
	);
END;
GO




USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Cargos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cargos (
        IdCargo INT IDENTITY(1,1) PRIMARY KEY,
        Cargo NVARCHAR(100) NOT NULL UNIQUE
    );
END;
GO



Use SistemaGestionFacturacion
IF OBJECT_ID('dbo.Usuarios','U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Apellido NVARCHAR(100) NOT NULL,
        Usuario NVARCHAR(50) NOT NULL UNIQUE,
        Clave VARBINARY(512) NOT NULL,   -- almacenar hash (PBKDF2 / Rfc2898)
        Sal VARBINARY(128) NOT NULL,     -- salt usado para el hash
        Iteraciones INT NOT NULL DEFAULT (10000),
        IdRol INT NOT NULL,
        Estado VARCHAR(15) NOT NULL DEFAULT ('Activo'),

        CONSTRAINT FK_Usuarios_Pedidos FOREIGN KEY (IdRol)
            REFERENCES dbo.Roles (IdRol)
            ON DELETE NO ACTION
            ON UPDATE NO ACTION
    );
END;
GO


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
