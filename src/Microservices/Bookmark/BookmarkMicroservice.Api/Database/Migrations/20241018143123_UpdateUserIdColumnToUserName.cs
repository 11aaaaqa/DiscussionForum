using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookmarkMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserIdColumnToUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookmarks");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Bookmarks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Bookmarks");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Bookmarks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
