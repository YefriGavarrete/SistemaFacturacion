-- tabla Roles

USE SistemaGestionFacturacion
CREATE TABLE Roles (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    Rol VARCHAR(50) NOT NULL UNIQUE, 
    Estado VARCHAR(15) NOT NULL
);

