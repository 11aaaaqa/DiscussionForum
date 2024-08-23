using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegisterMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveThreeFieldsFromUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Posts",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Answers",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Posts",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateOnly>(
                name: "RegisteredAt",
                table: "Users",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
