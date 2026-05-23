using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAttendanceTotalHoursType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalHours",
                table: "Attendances",
                type: "float",
                nullable: true,
                computedColumnSql: "CAST(DATEDIFF(MINUTE, CheckIn, CheckOut) AS FLOAT) / 60.0",
                stored: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldComputedColumnSql: "DATEDIFF(MINUTE, CheckIn, CheckOut) / 60.0",
                oldStored: true);

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 38, 13, 32, DateTimeKind.Utc).AddTicks(5107));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 38, 13, 32, DateTimeKind.Utc).AddTicks(5109));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 38, 13, 32, DateTimeKind.Utc).AddTicks(5110));

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentId",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 5, 23, 6, 38, 13, 32, DateTimeKind.Utc).AddTicks(5111));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalHours",
                table: "Attendances",
                type: "float",
                nullable: true,
                computedColumnSql: "DATEDIFF(MINUTE, CheckIn, CheckOut) / 60.0",
                stored: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldComputedColumnSql: "CAST(DATEDIFF(MINUTE, CheckIn, CheckOut) AS FLOAT) / 60.0",
                oldStored: true);

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
        }
    }
}
