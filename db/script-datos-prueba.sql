-- =========================
-- Departamentos
-- =========================
INSERT INTO Departments (dept_no, dept_name) VALUES
('RRHH', 'Recursos Humanos'),
('GER', 'Gerencia'),
('OPE', 'Operarios'),
('VEN', 'Ventas');
GO

-- =========================
-- Empleados
-- =========================
-- NOTA: emp_no es IDENTITY, no se debe insertar manualmente
INSERT INTO Employees (ci, birth_date, first_name, last_name, gender, hire_date, correo) VALUES
('1728392010', '1985-03-12', 'Carlos', 'Perez', 'M', '2010-05-01', 'carlos.perez@empresa.com'),
('1827364551', '1990-07-25', 'Maria', 'Gonzalez', 'F', '2012-09-15', 'maria.gonzalez@empresa.com'),
('1928374650', '1988-11-02', 'Jorge', 'Ramirez', 'M', '2015-01-20', 'jorge.ramirez@empresa.com'),
('1357924680', '1992-06-18', 'Lucia', 'Fernandez', 'F', '2018-04-10', 'lucia.fernandez@empresa.com');
GO

-- Obtener los IDs generados
SELECT emp_no, first_name, last_name FROM Employees;
GO

-- =========================
-- Relación empleados con departamentos
-- =========================
-- Reemplaza los IDs con los generados arriba (ejemplo con IDs 1-4)
INSERT INTO DeptEmp (emp_no, dept_no, from_date, to_date) VALUES
(1, 'RRHH', '2010-05-01', '2018-12-31'),
(2, 'GER', '2012-09-15', '2020-06-30'),
(3, 'OPE', '2015-01-20', '2023-12-31'),
(4, 'VEN', '2018-04-10', '2024-12-31');
GO

-- =========================
-- Managers de cada departamento
-- =========================
INSERT INTO DeptManager (emp_no, dept_no, from_date, to_date) VALUES
(1, 'RRHH', '2015-01-01', '2018-12-31'),
(2, 'GER', '2016-02-01', '2020-06-30'),
(3, 'OPE', '2020-03-01', '2023-12-31'),
(4, 'VEN', '2021-04-01', '2024-12-31');
GO

-- =========================
-- Títulos (cargos)
-- =========================
INSERT INTO Titles (emp_no, title, from_date, to_date) VALUES
(1, 'Analista de RRHH', '2010-05-01', '2014-12-31'),
(1, 'Jefe de RRHH', '2015-01-01', '2018-12-31'),
(2, 'Asistente Financiera', '2012-09-15', '2015-12-31'),
(2, 'Analista Financiero Senior', '2016-01-01', '2020-06-30'),
(3, 'Desarrollador Suplente', '2015-01-20', '2019-12-31'),
(3, 'Líder de Desarrollo', '2020-01-01', '2023-12-31'),
(4, 'Ejecutiva de Ventas', '2018-04-10', '2020-12-31'),
(4, 'Supervisora de Ventas', '2021-01-01', '2024-12-31');
GO

-- =========================
-- Salarios
-- =========================
INSERT INTO Salaries (emp_no, salary, from_date, to_date) VALUES
(1, 1800, '2010-05-01', '2014-12-31'),
(1, 2500, '2015-01-01', '2018-12-31'),
(2, 2000, '2012-09-15', '2015-12-31'),
(2, 3000, '2016-01-01', '2020-06-30'),
(3, 1500, '2015-01-20', '2019-12-31'),
(3, 2800, '2020-01-01', '2023-12-31'),
(4, 1700, '2018-04-10', '2020-12-31'),
(4, 2600, '2021-01-01', '2024-12-31');
GO

-- =========================
-- Usuarios de login (con contraseñas hasheadas)
-- =========================
-- NOTA: Las contraseñas deben estar hasheadas con BCrypt
-- Aquí usamos el hash de '123456' para pruebas
INSERT INTO Users (emp_no, usuario, clave, rol) VALUES
(1, 'cperez', '$2a$11$9xXjQQqQqQqQqQqQqQqQqOqQqQqQqQqQqQqQqQqQqQqQqQqQq', 'RRHH'),
(2, 'mgonzalez', '$2a$11$9xXjQQqQqQqQqQqQqQqQqOqQqQqQqQqQqQqQqQqQqQqQqQqQq', 'RRHH'),
(3, 'jramirez', '$2a$11$9xXjQQqQqQqQqQqQqQqQqOqQqQqQqQqQqQqQqQqQqQqQqQqQq', 'RRHH'),
(4, 'lfernandez', '$2a$11$9xXjQQqQqQqQqQqQqQqQqOqQqQqQqQqQqQqQqQqQqQqQqQqQq', 'RRHH');
GO

-- =========================
-- Auditoría de cambios de salarios
-- =========================
INSERT INTO LogAuditoriaSalarios (usuario, fechaActualizacion, DetalleCambio, salario, emp_no) VALUES
('cperez', '2015-01-10', 'Ascenso a Jefe de RRHH, incremento salarial', 2500, 1),
('mgonzalez', '2016-01-15', 'Promoción a Analista Financiero Senior', 3000, 2),
('jramirez', '2020-01-05', 'Ascenso a Líder de Desarrollo', 2800, 3),
('lfernandez', '2021-01-12', 'Promoción a Supervisora de Ventas', 2600, 4);
GO