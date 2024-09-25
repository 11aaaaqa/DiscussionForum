using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class RecreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5fb75f19-d886-41bf-acf8-75da33808ae7", "f5f02ba2-edc0-4e4a-915f-6591e172de41", "User", "USER" },
                    { "616af394-5c26-4013-9893-963aed90eaa3", "a3d4bf48-c903-4b0c-bc18-712692e8e4c2", "Moderator", "MODERATOR" },
                    { "bbfe99e1-2e0a-4b21-8392-873ba3484f14", "71a8de79-6823-44c2-88d9-3fb71a0ee032", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5fb75f19-d886-41bf-acf8-75da33808ae7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "616af394-5c26-4013-9893-963aed90eaa3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbfe99e1-2e0a-4b21-8392-873ba3484f14");

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
    }
}
