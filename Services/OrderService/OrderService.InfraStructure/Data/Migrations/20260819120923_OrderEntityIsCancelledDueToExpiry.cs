using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.InfraStructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderEntityIsCancelledDueToExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentExpiresAt",
                table: "Orders",
                newName: "ExpiresAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelledDueToExpiry",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancelledDueToExpiry",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "Orders",
                newName: "PaymentExpiresAt");
        }
    }
}
