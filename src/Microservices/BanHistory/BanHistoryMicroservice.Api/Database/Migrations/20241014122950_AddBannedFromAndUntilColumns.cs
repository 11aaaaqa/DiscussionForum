using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHistoryMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBannedFromAndUntilColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BannedFrom",
                table: "Bans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedUntil",
                table: "Bans",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannedFrom",
                table: "Bans");

            migrationBuilder.DropColumn(
                name: "BannedUntil",
                table: "Bans");
        }
    }
}
