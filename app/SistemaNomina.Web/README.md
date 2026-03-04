# Sistema de Nómina - ASP.NET Core MVC

Sistema de gestión de nómina desarrollado en ASP.NET Core 8+ con C#, utilizando Entity Framework Core y SQL Server. Permite administrar empleados, departamentos, asignaciones, salarios, títulos y usuarios, con control de vigencias y auditoría de cambios salariales.

## 📋 Tabla de Contenidos
- [Requisitos Previos](#requisitos-previos)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Estructura del Repositorio](#estructura-del-repositorio)
- [Configuración Inicial](#configuración-inicial)
- [Ejecutar la Aplicación](#ejecutar-la-aplicación)
- [Migraciones de Base de Datos](#migraciones-de-base-de-datos)
- [Ramas y Flujo de Trabajo](#ramas-y-flujo-de-trabajo)
- [Convenciones de Commits](#convenciones-de-commits)
- [Despliegue](#despliegue)
- [Créditos](#créditos)

---

## ✅ Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (Express, Developer o superior)
- [Git](https://git-scm.com/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/es/vs/) (o VS Code con extensiones para C#)
- Opcional: [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms) para administrar la BD

---

## 🛠️ Tecnologías Utilizadas

- **Backend:** ASP.NET Core 8+ (MVC)
- **ORM:** Entity Framework Core
- **Base de Datos:** SQL Server
- **Frontend:** Bootstrap 5 / Tailwind CSS (a elección)
- **Control de Versiones:** Git (GitHub/GitLab)
- **Autenticación:** ASP.NET Core Identity + JWT (opcional) o hash personalizado
- **Reportes:** Exportación a PDF/Excel con librerías como iTextSharp o EPPlus

---

## 📁 Estructura del Repositorio
/
├── app/ # Aplicación ASP.NET Core MVC
│ ├── Controllers/ # Controladores
│ ├── Models/ # Modelos de EF Core y ViewModels
│ ├── Views/ # Vistas Razor
│ ├── Services/ # Lógica de negocio
│ ├── Data/ # Contexto de BD y configuraciones
│ ├── Migrations/ # Migraciones de EF Core
│ └── wwwroot/ # Archivos estáticos (CSS, JS, imágenes)
├── db/ # Scripts SQL adicionales
│ ├── seed-data.sql # Datos de prueba
│ └── schema.sql # Script de creación de BD (opcional)
├── docs/ # Documentación adicional
├── tests/ # Pruebas unitarias (xUnit/NUnit)
├── .gitignore # Archivos ignorados por Git
├── README.md # Este archivo
└── Nomina.sln # Solución de Visual Studio
## ⚙️ Configuración Inicial

### 1. Clonar el repositorio

```bash
git clone https://github.com/barahonajhondeer/SistemaNomina
cd nomina-aspnet-core

2. Configurar la cadena de conexión
En el archivo app/appsettings.json, modifica la sección ConnectionStrings:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NominaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}

3. Restaurar paquetes NuGet
Desde la carpeta app/:

bash
dotnet restore

🗄️ Migraciones de Base de Datos
Crear una migración inicial (si no existe):
bash
cd app
dotnet ef migrations add InitialCreate -o Migrations

Aplicar migraciones a la base de datos:
bash
dotnet ef database update

