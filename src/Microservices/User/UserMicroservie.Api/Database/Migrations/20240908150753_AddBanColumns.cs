using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBanColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BanType",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BannedFor",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedUntil",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedFor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannedUntil",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");
        }
    }
}
