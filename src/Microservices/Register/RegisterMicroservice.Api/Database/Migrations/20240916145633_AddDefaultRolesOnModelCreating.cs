using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultRolesOnModelCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fd648c2-a503-4cfa-8a55-b6f96255a820", "1ebe364e-43c7-449f-8997-cc5b3e87e66a", "User", "USER" },
                    { "91ec7d09-4972-4c50-96ef-a3df3edb0d43", "c0980939-7990-4c45-826f-8c94001b2d45", "Admin", "ADMIN" },
                    { "9712ecb3-9112-4617-a8be-71aef627ff36", "e58d6b76-99bc-46f9-93f3-90049d6bee3b", "Moderator", "MODERATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fd648c2-a503-4cfa-8a55-b6f96255a820");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "91ec7d09-4972-4c50-96ef-a3df3edb0d43");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9712ecb3-9112-4617-a8be-71aef627ff36");
        }
    }
}
