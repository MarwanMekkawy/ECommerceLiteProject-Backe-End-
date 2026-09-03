using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedProcessedStripeEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FailureReason",
                table: "Payments",
                newName: "RefundFailureReason");

            migrationBuilder.RenameColumn(
                name: "FailedAt",
                table: "Payments",
                newName: "RefundInitiatedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsOrderCancellationDueToRefundConfirmed",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextOrderCancellationAttemptAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCancellationReattempts",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentFailedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentFailureReason",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundFailedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcessedStripeEvents",
                columns: table => new
                {
                    StripeEventId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedStripeEvents", x => x.StripeEventId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedStripeEvents");

            migrationBuilder.DropColumn(
                name: "IsOrderCancellationDueToRefundConfirmed",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "NextOrderCancellationAttemptAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderCancellationReattempts",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentFailedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentFailureReason",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundFailedAt",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "RefundInitiatedAt",
                table: "Payments",
                newName: "FailedAt");

            migrationBuilder.RenameColumn(
                name: "RefundFailureReason",
                table: "Payments",
                newName: "FailureReason");
        }
    }
}
