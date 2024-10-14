using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanHistoryMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBannedByColumnInBanHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannedBy",
                table: "Bans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannedBy",
                table: "Bans");
        }
    }
}
