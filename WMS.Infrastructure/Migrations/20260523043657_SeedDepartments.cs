using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "CreatedOn", "DepartmentName", "Description" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5175), "Human Resources", "HR Department" },
                    { 2, new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5177), "Engineering", "Engineering Department" },
                    { 3, new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5178), "Finance", "Finance Department" },
                    { 4, new DateTime(2026, 5, 23, 4, 36, 56, 676, DateTimeKind.Utc).AddTicks(5179), "Operations", "Operations Department" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4);
        }
    }
}
