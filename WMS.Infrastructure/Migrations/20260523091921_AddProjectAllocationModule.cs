using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectAllocationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeProjects_EmpId",
                table: "EmployeeProjects");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 9, 19, 21, 295, DateTimeKind.Utc).AddTicks(3368));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 9, 19, 21, 295, DateTimeKind.Utc).AddTicks(3371));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 9, 19, 21, 295, DateTimeKind.Utc).AddTicks(3372));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 9, 19, 21, 295, DateTimeKind.Utc).AddTicks(3373));

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjects_EmpId_ProjectId_Status",
                table: "EmployeeProjects",
                columns: new[] { "EmpId", "ProjectId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeProjects_EmpId_ProjectId_Status",
                table: "EmployeeProjects");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 7, 22, 29, 153, DateTimeKind.Utc).AddTicks(6635));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 7, 22, 29, 153, DateTimeKind.Utc).AddTicks(6637));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 7, 22, 29, 153, DateTimeKind.Utc).AddTicks(6639));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 7, 22, 29, 153, DateTimeKind.Utc).AddTicks(6640));

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProjects_EmpId",
                table: "EmployeeProjects",
                column: "EmpId");
        }
    }
}
