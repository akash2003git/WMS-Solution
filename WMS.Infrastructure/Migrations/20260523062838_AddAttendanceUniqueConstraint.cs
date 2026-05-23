using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_EmpId",
                table: "Attendances");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 28, 37, 874, DateTimeKind.Utc).AddTicks(5763));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 28, 37, 874, DateTimeKind.Utc).AddTicks(5765));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 28, 37, 874, DateTimeKind.Utc).AddTicks(5766));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 28, 37, 874, DateTimeKind.Utc).AddTicks(5767));

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmpId_AttendanceDate",
                table: "Attendances",
                columns: new[] { "EmpId", "AttendanceDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_EmpId_AttendanceDate",
                table: "Attendances");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5175));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5177));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5178));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5179));

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EmpId",
                table: "Attendances",
                column: "EmpId");
        }
    }
}
