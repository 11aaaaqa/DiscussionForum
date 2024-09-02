using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAvatarFromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "Users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
