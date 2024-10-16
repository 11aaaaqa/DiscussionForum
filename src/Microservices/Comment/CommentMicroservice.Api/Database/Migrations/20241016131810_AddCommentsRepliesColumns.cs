using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsRepliesColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RepliedOnCommentContent",
                table: "SuggestedComments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepliedOnCommentCreatedBy",
                table: "SuggestedComments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RepliedOnCommentId",
                table: "SuggestedComments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepliedOnCommentContent",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepliedOnCommentCreatedBy",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RepliedOnCommentId",
                table: "Comments",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepliedOnCommentContent",
                table: "SuggestedComments");

            migrationBuilder.DropColumn(
                name: "RepliedOnCommentCreatedBy",
                table: "SuggestedComments");

            migrationBuilder.DropColumn(
                name: "RepliedOnCommentId",
                table: "SuggestedComments");

            migrationBuilder.DropColumn(
                name: "RepliedOnCommentContent",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "RepliedOnCommentCreatedBy",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "RepliedOnCommentId",
                table: "Comments");
        }
    }
}
