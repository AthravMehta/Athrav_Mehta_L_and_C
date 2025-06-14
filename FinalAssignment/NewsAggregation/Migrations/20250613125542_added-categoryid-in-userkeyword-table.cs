using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Migrations
{
    /// <inheritdoc />
    public partial class addedcategoryidinuserkeywordtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "UserKeywords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserKeywords_CategoryId",
                table: "UserKeywords",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserKeywords_Categories_CategoryId",
                table: "UserKeywords",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserKeywords_Categories_CategoryId",
                table: "UserKeywords");

            migrationBuilder.DropIndex(
                name: "IX_UserKeywords_CategoryId",
                table: "UserKeywords");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "UserKeywords");
        }
    }
}
