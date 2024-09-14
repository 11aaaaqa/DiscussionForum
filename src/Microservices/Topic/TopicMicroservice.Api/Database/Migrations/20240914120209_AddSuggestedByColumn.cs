using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopicMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestedByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SuggestedBy",
                table: "SuggestedTopics",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuggestedBy",
                table: "SuggestedTopics");
        }
    }
}
