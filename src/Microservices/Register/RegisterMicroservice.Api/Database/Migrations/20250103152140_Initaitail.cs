using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initaitail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "140316ef-040d-4a68-81bd-f92ca3888fa7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9108c75a-9b89-4a01-8560-875954357ce3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8f9bc2d-3c1a-4732-b243-ed7426214d01");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6a7b9649-4e61-4865-b08b-4394b034e68d", "562960ce-521b-41f7-87cf-68acfe085ce7", "User", "USER" },
                    { "8db313ff-9e39-4780-ab21-c0a3f5a4bd83", "ff2f146e-78ff-4376-b4b2-f7c83b5a6309", "Admin", "ADMIN" },
                    { "9e633119-e97f-4992-a4b6-1c84736a1d96", "53857455-7b4c-4735-86af-37d2ca3d45bd", "Moderator", "MODERATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6a7b9649-4e61-4865-b08b-4394b034e68d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8db313ff-9e39-4780-ab21-c0a3f5a4bd83");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9e633119-e97f-4992-a4b6-1c84736a1d96");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "140316ef-040d-4a68-81bd-f92ca3888fa7", "470c36ba-a7fe-481e-9b1c-17a0ed962263", "Moderator", "MODERATOR" },
                    { "9108c75a-9b89-4a01-8560-875954357ce3", "deef963f-ac3e-4e50-9f05-e6a5cae4e237", "Admin", "ADMIN" },
                    { "a8f9bc2d-3c1a-4732-b243-ed7426214d01", "26e80c55-2a21-4028-9984-58db809f0c6d", "User", "USER" }
                });
        }
    }
}
