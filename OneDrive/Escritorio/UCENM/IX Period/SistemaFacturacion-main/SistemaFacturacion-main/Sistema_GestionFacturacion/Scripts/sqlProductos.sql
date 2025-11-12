USE SistemaGestionFacturacion
IF OBJECT_ID('dbo.Productos', 'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[Productos](
		[IdProducto] INT IDENTITY(1,1) NOT NULL,
		[Codigo] VARCHAR(10) NOT NULL,
		[Marca] VARCHAR(50) NOT NULL,
		[Modelo] VARCHAR(100) NOT NULL,
		[Precio] DECIMAL(10,2) NOT NULL,
		[Stock] INT NOT NULL,
		[IdCategoria] INT NOT NULL,
		[IdDescuento] INT NOT NULL,
		[Descripcion] VARCHAR(255) NOT NULL,
	PRIMARY KEY CLUSTERED ([IdProducto] ASC)
);
END;
GO

CREATE TABLE [dbo].[DetallesPedidos](
	[IdDetallesPedidos] INT IDENTITY(1,1) NOT NULL,
	[IdPedido] INT NOT NULL,
	[IdProducto] INT NOT NULL,
	[Categoria] NVARCHAR(100) NOT NULL,
	[Marca] NVARCHAR(100) NOT NULL,
	[Modelo] NVARCHAR(100) NOT NULL,
	[Precio] DECIMAL(20,4) NOT NULL,
	[Cantidad] INT NOT NULL,
	[Descuento] DECIMAL(10,4) NOT NULL,
	[Subtotal] DECIMAL(10,4) NOT NULL,
	[ISV] DECIMAL(10,4) NOT NULL,
	[Total] DECIMAL(20,4) NOT NULL,
PRIMARY KEY CLUSTERED ([IdDetallesPedidos] ASC)
);
GO




