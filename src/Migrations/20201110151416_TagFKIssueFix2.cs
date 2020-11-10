using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoApi.Migrations
{
    public partial class TagFKIssueFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TodoItems_TodoItemId",
                table: "Tags");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TodoItems_TodoItemId",
                table: "Tags",
                column: "TodoItemId",
                principalTable: "TodoItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TodoItems_TodoItemId",
                table: "Tags");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TodoItems_TodoItemId",
                table: "Tags",
                column: "TodoItemId",
                principalTable: "TodoItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
