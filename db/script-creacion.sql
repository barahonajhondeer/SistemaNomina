-- =============================================
-- SCRIPT COMPLETO DE BASE DE DATOS - SISTEMA NÓMINA
-- =============================================

USE [master]
GO

-- Eliminar base de datos si existe
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SistemaNominaDB')
BEGIN
    DROP DATABASE SistemaNominaDB;
END
GO

-- Crear base de datos
CREATE DATABASE SistemaNominaDB;
GO

USE SistemaNominaDB;
GO

-- =============================================
-- TABLA: Employees
-- =============================================
CREATE TABLE Employees (
    emp_no INT IDENTITY(1,1) NOT NULL,
    ci NVARCHAR(20) NOT NULL,
    birth_date DATE NOT NULL,
    first_name NVARCHAR(50) NOT NULL,
    last_name NVARCHAR(50) NOT NULL,
    gender NVARCHAR(1) NOT NULL,
    hire_date DATE NOT NULL,
    correo NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Employees PRIMARY KEY (emp_no),
    CONSTRAINT UQ_Employees_ci UNIQUE (ci),
    CONSTRAINT UQ_Employees_correo UNIQUE (correo),
    CONSTRAINT CK_Employees_gender CHECK (gender IN ('M', 'F'))
);
GO

-- =============================================
-- TABLA: Departments
-- =============================================
CREATE TABLE Departments (
    dept_no NVARCHAR(10) NOT NULL,
    dept_name NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Departments PRIMARY KEY (dept_no),
    CONSTRAINT UQ_Departments_name UNIQUE (dept_name)
);
GO

-- =============================================
-- TABLA: DeptEmp (Asignaciones)
-- =============================================
CREATE TABLE DeptEmp (
    emp_no INT NOT NULL,
    dept_no NVARCHAR(10) NOT NULL,
    from_date DATE NOT NULL,
    to_date DATE NULL,
    CONSTRAINT PK_DeptEmp PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT FK_DeptEmp_Employee FOREIGN KEY (emp_no) REFERENCES Employees(emp_no) ON DELETE CASCADE,
    CONSTRAINT FK_DeptEmp_Department FOREIGN KEY (dept_no) REFERENCES Departments(dept_no) ON DELETE CASCADE
);
GO

-- =============================================
-- TABLA: DeptManager (Gerentes)
-- =============================================
CREATE TABLE DeptManager (
    emp_no INT NOT NULL,
    dept_no NVARCHAR(10) NOT NULL,
    from_date DATE NOT NULL,
    to_date DATE NULL,
    CONSTRAINT PK_DeptManager PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT FK_DeptManager_Employee FOREIGN KEY (emp_no) REFERENCES Employees(emp_no) ON DELETE CASCADE,
    CONSTRAINT FK_DeptManager_Department FOREIGN KEY (dept_no) REFERENCES Departments(dept_no) ON DELETE CASCADE
);
GO

-- =============================================
-- TABLA: Titles (Títulos/Cargos)
-- =============================================
CREATE TABLE Titles (
    emp_no INT NOT NULL,
    title NVARCHAR(100) NOT NULL,
    from_date DATE NOT NULL,
    to_date DATE NULL,
    CONSTRAINT PK_Titles PRIMARY KEY (emp_no, title, from_date),
    CONSTRAINT FK_Titles_Employee FOREIGN KEY (emp_no) REFERENCES Employees(emp_no) ON DELETE CASCADE
);
GO

-- =============================================
-- TABLA: Salaries (Salarios)
-- =============================================
CREATE TABLE Salaries (
    emp_no INT NOT NULL,
    from_date DATE NOT NULL,
    salary DECIMAL(18,2) NOT NULL,
    to_date DATE NULL,
    CONSTRAINT PK_Salaries PRIMARY KEY (emp_no, from_date),
    CONSTRAINT FK_Salaries_Employee FOREIGN KEY (emp_no) REFERENCES Employees(emp_no) ON DELETE CASCADE,
    CONSTRAINT CK_Salaries_salary CHECK (salary > 0)
);
GO

-- =============================================
-- TABLA: Users (Usuarios)
-- =============================================
CREATE TABLE Users (
    emp_no INT NOT NULL,
    usuario NVARCHAR(50) NOT NULL,
    clave NVARCHAR(255) NOT NULL,
    rol NVARCHAR(20) NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY (emp_no),
    CONSTRAINT FK_Users_Employee FOREIGN KEY (emp_no) REFERENCES Employees(emp_no) ON DELETE CASCADE,
    CONSTRAINT UQ_Users_usuario UNIQUE (usuario),
    CONSTRAINT CK_Users_rol CHECK (rol IN ('Administrador', 'RRHH'))
);
GO

-- =============================================
-- TABLA: LogAuditoriaSalarios (Auditoría)
-- =============================================
CREATE TABLE LogAuditoriaSalarios (
    id INT IDENTITY(1,1) NOT NULL,
    usuario NVARCHAR(50) NOT NULL,
    fechaActualizacion DATETIME NOT NULL,
    DetalleCambio NVARCHAR(500) NOT NULL,
    salario DECIMAL(18,2) NOT NULL,
    emp_no INT NOT NULL,
    CONSTRAINT PK_LogAuditoriaSalarios PRIMARY KEY (id)
);
GO