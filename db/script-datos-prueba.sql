-- =============================================
-- DATOS DE PRUEBA
-- =============================================

-- Insertar empleados (el IDENTITY generará emp_no automáticamente)
INSERT INTO Employees (ci, birth_date, first_name, last_name, gender, hire_date, correo)
VALUES 
('11111111', '1990-01-01', 'Admin', 'Sistema', 'M', '2024-01-01', 'admin@sistema.com'),
('22222222', '1992-05-15', 'Juan', 'Pérez', 'M', '2024-02-01', 'juan.perez@sistema.com'),
('33333333', '1988-09-20', 'María', 'González', 'F', '2024-02-15', 'maria.gonzalez@sistema.com');
GO

-- Ver los IDs generados
SELECT * FROM Employees;
GO

-- Insertar departamentos
INSERT INTO Departments (dept_no, dept_name)
VALUES 
('IT', 'Tecnología'),
('HR', 'Recursos Humanos'),
('FIN', 'Finanzas'),
('MKT', 'Marketing');
GO

-- Insertar usuarios (contraseña: 123456 hasheada)
INSERT INTO Users (emp_no, usuario, clave, rol)
VALUES 
(1, 'admin', '$2a$11$9xXjQQqQqQqQqQqQqQqQqOqQqQqQqQqQqQqQqQqQqQqQqQqQq', 'Administrador'),
(2, 'jperez', '$2a$11$9xXjQQqQqQqQqQqQqQqQqOqQqQqQqQqQqQqQqQqQqQqQqQqQq', 'RRHH');
GO