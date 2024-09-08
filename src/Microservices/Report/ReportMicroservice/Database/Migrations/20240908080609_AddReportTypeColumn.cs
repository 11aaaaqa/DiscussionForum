using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReportTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "Reports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "Reports");
        }
    }
}
