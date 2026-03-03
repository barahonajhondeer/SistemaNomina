using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaNomina.Web.ViewModels
{
    // ViewModel para filtros de reportes
    public class ReporteFiltrosViewModel
    {
        [Display(Name = "Departamento")]
        public string? DeptNo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Inicio")]
        public DateTime? FechaInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Fin")]
        public DateTime? FechaFin { get; set; }

        public List<DepartamentoListItem>? Departamentos { get; set; }
    }

    // ViewModel para items de departamento en listas desplegables
    public class DepartamentoListItem
    {
        public string DeptNo { get; set; } = string.Empty;
        public string DeptName { get; set; } = string.Empty;
    }

    // ViewModel para Nómina Vigente
    public class NominaVigenteViewModel
    {
        [Display(Name = "Código Departamento")]
        public string DeptNo { get; set; } = string.Empty;

        [Display(Name = "Departamento")]
        public string Departamento { get; set; } = string.Empty;

        [Display(Name = "N° Empleado")]
        public int EmpNo { get; set; }

        [Display(Name = "Empleado")]
        public string Empleado { get; set; } = string.Empty;

        [Display(Name = "Cédula")]
        public string Ci { get; set; } = string.Empty;

        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Display(Name = "Salario")]
        [DataType(DataType.Currency)]
        public decimal Salario { get; set; }

        [Display(Name = "Fecha Contratación")]
        [DataType(DataType.Date)]
        public DateTime FechaContratacion { get; set; }
    }

    // ViewModel para Cambios Salariales
    public class CambioSalarialViewModel
    {
        [Display(Name = "N° Empleado")]
        public int EmpNo { get; set; }

        [Display(Name = "Empleado")]
        public string Empleado { get; set; } = string.Empty;

        [Display(Name = "Cédula")]
        public string Ci { get; set; } = string.Empty;

        [Display(Name = "Salario Anterior")]
        [DataType(DataType.Currency)]
        public decimal SalarioAnterior { get; set; }

        [Display(Name = "Salario Nuevo")]
        [DataType(DataType.Currency)]
        public decimal SalarioNuevo { get; set; }

        [Display(Name = "Diferencia")]
        [DataType(DataType.Currency)]
        public decimal Diferencia { get; set; }

        [Display(Name = "Fecha Cambio")]
        public DateTime FechaCambio { get; set; }

        [Display(Name = "Usuario")]
        public string Usuario { get; set; } = string.Empty;

        [Display(Name = "Detalle")]
        public string Detalle { get; set; } = string.Empty;
    }

    // ViewModel para Estructura Organizacional
    public class EstructuraOrganizacionalViewModel
    {
        [Display(Name = "Código Departamento")]
        public string DeptNo { get; set; } = string.Empty;

        [Display(Name = "Departamento")]
        public string Departamento { get; set; } = string.Empty;

        [Display(Name = "Gerente")]
        public string? Gerente { get; set; }

        [Display(Name = "N° Gerente")]
        public int? GerenteEmpNo { get; set; }

        [Display(Name = "Empleados")]
        public List<EmpleadoDeptoViewModel> Empleados { get; set; } = new();

        [Display(Name = "Total Empleados")]
        public int TotalEmpleados => Empleados.Count;
    }

    // ViewModel para empleados dentro de departamento (Estructura Organizacional)
    public class EmpleadoDeptoViewModel
    {
        [Display(Name = "N° Empleado")]
        public int EmpNo { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Cédula")]
        public string Ci { get; set; } = string.Empty;

        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Display(Name = "Salario")]
        [DataType(DataType.Currency)]
        public decimal Salario { get; set; }
    }
}