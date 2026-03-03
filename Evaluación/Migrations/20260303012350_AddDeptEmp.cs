using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluación.Migrations
{
    /// <inheritdoc />
    public partial class AddDeptEmp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeptEmp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpNo = table.Column<int>(type: "int", nullable: false),
                    DeptNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeptEmp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeptEmp_Departments_DeptNo",
                        column: x => x.DeptNo,
                        principalTable: "Departments",
                        principalColumn: "DeptNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeptEmp_Employees_EmpNo",
                        column: x => x.EmpNo,
                        principalTable: "Employees",
                        principalColumn: "EmpNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmp_DeptNo",
                table: "DeptEmp",
                column: "DeptNo");

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmp_EmpNo",
                table: "DeptEmp",
                column: "EmpNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeptEmp");
        }
    }
}
