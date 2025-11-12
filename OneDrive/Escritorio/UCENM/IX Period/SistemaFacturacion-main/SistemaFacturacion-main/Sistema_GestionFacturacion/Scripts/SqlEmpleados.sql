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

USE  SistemaGestionFacturacion
IF COL_LENGTH('dbo.Empleados', 'Estado') IS NULL
    BEGIN
        ALTER TABLE dbo.Empleados
        ADD Estado VARCHAR(15) NOT NULL DEFAULT ('Activo');
       
END;