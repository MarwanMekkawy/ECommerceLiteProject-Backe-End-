using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedPaymentCompletionConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "Payments",
                newName: "SucceededAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsOrderCompletionConfirmed",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextOrderCompletionAttemptAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCompletionReattempts",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeRefundId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOrderCompletionConfirmed",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "NextOrderCompletionAttemptAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderCompletionReattempts",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripeRefundId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "SucceededAt",
                table: "Payments",
                newName: "CompletedAt");
        }
    }
}
