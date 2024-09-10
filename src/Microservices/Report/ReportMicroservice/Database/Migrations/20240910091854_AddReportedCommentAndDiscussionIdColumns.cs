using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReportedCommentAndDiscussionIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportedCommentId",
                table: "Reports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReportedDiscussionId",
                table: "Reports",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportedCommentId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportedDiscussionId",
                table: "Reports");
        }
    }
}
