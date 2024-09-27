using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class NoChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12c7b679-8f6e-4d88-b63e-88b91a235325");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89db8b28-ac0a-465d-9c49-29d88b052494");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "efccbfe5-8fbf-4016-89a9-af1a0d0ca1cb");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "12c7b679-8f6e-4d88-b63e-88b91a235325", "d108294b-cdff-44c4-8f51-2fa79c3f57c9", "Admin", "ADMIN" },
                    { "89db8b28-ac0a-465d-9c49-29d88b052494", "bd0c2d75-ffb1-4a43-b6f5-3155f7cf6eec", "User", "USER" },
                    { "efccbfe5-8fbf-4016-89a9-af1a0d0ca1cb", "335b4476-3c00-4567-8757-3fc0ff87f27e", "Moderator", "MODERATOR" }
                });
        }
    }
}
