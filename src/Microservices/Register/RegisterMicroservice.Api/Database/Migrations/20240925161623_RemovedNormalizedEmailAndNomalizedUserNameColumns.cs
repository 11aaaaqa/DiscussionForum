using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNormalizedEmailAndNomalizedUserNameColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                keyValue: "5fb75f19-d886-41bf-acf8-75da33808ae7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "616af394-5c26-4013-9893-963aed90eaa3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bbfe99e1-2e0a-4b21-8392-873ba3484f14");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "5fb75f19-d886-41bf-acf8-75da33808ae7", "f5f02ba2-edc0-4e4a-915f-6591e172de41", "User", "USER" },
                    { "616af394-5c26-4013-9893-963aed90eaa3", "a3d4bf48-c903-4b0c-bc18-712692e8e4c2", "Moderator", "MODERATOR" },
                    { "bbfe99e1-2e0a-4b21-8392-873ba3484f14", "71a8de79-6823-44c2-88d9-3fb71a0ee032", "Admin", "ADMIN" }
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
    }
}
