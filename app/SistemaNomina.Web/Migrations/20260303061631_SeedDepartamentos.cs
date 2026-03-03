using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaNomina.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedDepartamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "dept_no", "dept_name" },
                values: new object[,]
                {
                    { "FIN", "Finanzas" },
                    { "HR", "Recursos Humanos" },
                    { "IT", "Tecnología" },
                    { "MKT", "Marketing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "dept_no",
                keyValue: "FIN");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "dept_no",
                keyValue: "HR");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "dept_no",
                keyValue: "IT");

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "dept_no",
                keyValue: "MKT");
        }
    }
}
