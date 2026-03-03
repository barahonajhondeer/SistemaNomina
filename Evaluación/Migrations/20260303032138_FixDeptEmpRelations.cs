using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluación.Migrations
{
    /// <inheritdoc />
    public partial class FixDeptEmpRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeptEmp_Departments_DeptNo",
                table: "DeptEmp");

            migrationBuilder.DropForeignKey(
                name: "FK_DeptEmp_Employees_EmpNo",
                table: "DeptEmp");

            migrationBuilder.DropIndex(
                name: "IX_DeptEmp_DeptNo",
                table: "DeptEmp");

            migrationBuilder.DropIndex(
                name: "IX_DeptEmp_EmpNo",
                table: "DeptEmp");

            migrationBuilder.AlterColumn<string>(
                name: "DeptNo",
                table: "DeptEmp",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentDeptNo",
                table: "DeptEmp",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeEmpNo",
                table: "DeptEmp",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmp_DepartmentDeptNo",
                table: "DeptEmp",
                column: "DepartmentDeptNo");

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmp_EmployeeEmpNo",
                table: "DeptEmp",
                column: "EmployeeEmpNo");

            migrationBuilder.AddForeignKey(
                name: "FK_DeptEmp_Departments_DepartmentDeptNo",
                table: "DeptEmp",
                column: "DepartmentDeptNo",
                principalTable: "Departments",
                principalColumn: "DeptNo");

            migrationBuilder.AddForeignKey(
                name: "FK_DeptEmp_Employees_EmployeeEmpNo",
                table: "DeptEmp",
                column: "EmployeeEmpNo",
                principalTable: "Employees",
                principalColumn: "EmpNo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeptEmp_Departments_DepartmentDeptNo",
                table: "DeptEmp");

            migrationBuilder.DropForeignKey(
                name: "FK_DeptEmp_Employees_EmployeeEmpNo",
                table: "DeptEmp");

            migrationBuilder.DropIndex(
                name: "IX_DeptEmp_DepartmentDeptNo",
                table: "DeptEmp");

            migrationBuilder.DropIndex(
                name: "IX_DeptEmp_EmployeeEmpNo",
                table: "DeptEmp");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptNo",
                table: "DeptEmp");

            migrationBuilder.DropColumn(
                name: "EmployeeEmpNo",
                table: "DeptEmp");

            migrationBuilder.AlterColumn<string>(
                name: "DeptNo",
                table: "DeptEmp",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmp_DeptNo",
                table: "DeptEmp",
                column: "DeptNo");

            migrationBuilder.CreateIndex(
                name: "IX_DeptEmp_EmpNo",
                table: "DeptEmp",
                column: "EmpNo");

            migrationBuilder.AddForeignKey(
                name: "FK_DeptEmp_Departments_DeptNo",
                table: "DeptEmp",
                column: "DeptNo",
                principalTable: "Departments",
                principalColumn: "DeptNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeptEmp_Employees_EmpNo",
                table: "DeptEmp",
                column: "EmpNo",
                principalTable: "Employees",
                principalColumn: "EmpNo",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
