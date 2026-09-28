using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.InfraStructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedIsStockCompensatedBoolToOrderEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStockCompensated",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStockCompensated",
                table: "Orders");
        }
    }
}
