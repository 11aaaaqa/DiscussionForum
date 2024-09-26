using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "HangfireDelayedJobId",
                table: "Users",
                type: "text",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "HangfireDelayedJobId",
                table: "Users");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "98245b0f-bf67-4496-8913-9c50d586e4df", "45764778-7b67-4dc0-ab0d-8e3b8e81e5e4", "Moderator", "MODERATOR" },
                    { "9a0a7568-c50b-4eeb-9944-1d69925882d5", "7976a498-8b5a-4712-bc10-0f4b3bedc2b2", "User", "USER" },
                    { "e5729c21-dd40-4cb2-8dac-1c0a9f1756ae", "23b464c7-e0e2-4f39-8d93-6cf4d98157ec", "Admin", "ADMIN" }
                });
        }
    }
}
