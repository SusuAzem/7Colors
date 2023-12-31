using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _7Colors.Migrations
{
    /// <inheritdoc />
    public partial class Cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartLine_Products_ProductId",
                table: "ShoppingCartLine");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartLine_Users_UserNameIdentifier",
                table: "ShoppingCartLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingCartLine",
                table: "ShoppingCartLine");

            migrationBuilder.RenameTable(
                name: "ShoppingCartLine",
                newName: "ShoppingCartLines");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCartLine_UserNameIdentifier",
                table: "ShoppingCartLines",
                newName: "IX_ShoppingCartLines_UserNameIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCartLine_ProductId",
                table: "ShoppingCartLines",
                newName: "IX_ShoppingCartLines_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingCartLines",
                table: "ShoppingCartLines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartLines_Products_ProductId",
                table: "ShoppingCartLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartLines_Users_UserNameIdentifier",
                table: "ShoppingCartLines",
                column: "UserNameIdentifier",
                principalTable: "Users",
                principalColumn: "NameIdentifier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartLines_Products_ProductId",
                table: "ShoppingCartLines");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCartLines_Users_UserNameIdentifier",
                table: "ShoppingCartLines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingCartLines",
                table: "ShoppingCartLines");

            migrationBuilder.RenameTable(
                name: "ShoppingCartLines",
                newName: "ShoppingCartLine");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCartLines_UserNameIdentifier",
                table: "ShoppingCartLine",
                newName: "IX_ShoppingCartLine_UserNameIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCartLines_ProductId",
                table: "ShoppingCartLine",
                newName: "IX_ShoppingCartLine_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingCartLine",
                table: "ShoppingCartLine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartLine_Products_ProductId",
                table: "ShoppingCartLine",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCartLine_Users_UserNameIdentifier",
                table: "ShoppingCartLine",
                column: "UserNameIdentifier",
                principalTable: "Users",
                principalColumn: "NameIdentifier",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
