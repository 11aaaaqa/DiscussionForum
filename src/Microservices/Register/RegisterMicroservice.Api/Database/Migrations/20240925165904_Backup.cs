using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Backup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "078de902-98b3-43c7-8ea1-450f2c75666f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e3f07da-647c-449a-a4ae-0d76ecbb5425");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ffe5ea99-128b-4504-a464-8f34964e69bb");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "98245b0f-bf67-4496-8913-9c50d586e4df", "45764778-7b67-4dc0-ab0d-8e3b8e81e5e4", "Moderator", "MODERATOR" },
                    { "9a0a7568-c50b-4eeb-9944-1d69925882d5", "7976a498-8b5a-4712-bc10-0f4b3bedc2b2", "User", "USER" },
                    { "e5729c21-dd40-4cb2-8dac-1c0a9f1756ae", "23b464c7-e0e2-4f39-8d93-6cf4d98157ec", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98245b0f-bf67-4496-8913-9c50d586e4df");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9a0a7568-c50b-4eeb-9944-1d69925882d5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5729c21-dd40-4cb2-8dac-1c0a9f1756ae");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Users");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "078de902-98b3-43c7-8ea1-450f2c75666f", "4eb408af-93cf-4330-b93c-6c36ba76d04a", "Moderator", "MODERATOR" },
                    { "1e3f07da-647c-449a-a4ae-0d76ecbb5425", "f30e51a5-1be3-4d28-a8f5-187118b2c5ad", "Admin", "ADMIN" },
                    { "ffe5ea99-128b-4504-a464-8f34964e69bb", "043fd737-0e57-4099-a872-2f7ac0f7a98c", "User", "USER" }
                });
        }
    }
}
