using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbOperationsEFCore.Migrations
{
    /// <inheritdoc />
    public partial class addednewcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[] { 5, "from Japan", "Yen" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
