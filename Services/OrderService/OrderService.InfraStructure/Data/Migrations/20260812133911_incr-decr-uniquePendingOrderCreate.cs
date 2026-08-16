using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.InfraStructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class incrdecruniquePendingOrderCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId",
                unique: true,
                filter: "[Status] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");
        }
    }
}
