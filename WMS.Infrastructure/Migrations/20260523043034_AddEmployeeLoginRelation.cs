using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeLoginRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "UserLogins",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "UserLogins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "UserLogins",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "EmployeeId", "MustChangePassword" },
                values: new object[] { null, true });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_EmployeeId",
                table: "UserLogins",
                column: "EmployeeId",
                unique: true,
                filter: "[EmployeeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Employees_EmployeeId",
                table: "UserLogins",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Employees_EmployeeId",
                table: "UserLogins");

            migrationBuilder.DropIndex(
                name: "IX_UserLogins_EmployeeId",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "UserLogins");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "UserLogins");
        }
    }
}
