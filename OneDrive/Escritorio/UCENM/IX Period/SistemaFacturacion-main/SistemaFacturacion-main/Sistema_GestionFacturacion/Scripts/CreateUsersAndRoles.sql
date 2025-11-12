
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

Use SistemaGestionFacturacion
SELECT * FROM dbo.Usuarios
