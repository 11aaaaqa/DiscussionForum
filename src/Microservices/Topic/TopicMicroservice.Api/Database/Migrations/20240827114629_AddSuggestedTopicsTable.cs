using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopicMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSuggestedTopicsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Topic",
                table: "Topic");

            migrationBuilder.RenameTable(
                name: "Topic",
                newName: "Topics");

            migrationBuilder.RenameIndex(
                name: "IX_Topic_Name",
                table: "Topics",
                newName: "IX_Topics_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topics",
                table: "Topics",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SuggestedTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PostsCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuggestedTopics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuggestedTopics_Name",
                table: "SuggestedTopics",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuggestedTopics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Topics",
                table: "Topics");

            migrationBuilder.RenameTable(
                name: "Topics",
                newName: "Topic");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_Name",
                table: "Topic",
                newName: "IX_Topic_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topic",
                table: "Topic",
                column: "Id");
        }
    }
}
