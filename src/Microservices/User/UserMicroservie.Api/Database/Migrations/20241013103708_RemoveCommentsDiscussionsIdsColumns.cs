using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMicroservice.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCommentsDiscussionsIdsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentsIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDiscussionsIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuggestedCommentsIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SuggestedDiscussionsIds",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "CommentsIds",
                table: "Users",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "CreatedDiscussionsIds",
                table: "Users",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "SuggestedCommentsIds",
                table: "Users",
                type: "uuid[]",
                nullable: false);

            migrationBuilder.AddColumn<List<Guid>>(
                name: "SuggestedDiscussionsIds",
                table: "Users",
                type: "uuid[]",
                nullable: false);
        }
    }
}
