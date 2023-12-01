using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseCreateAndGetList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_identity_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "identity_id",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "expenses",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "expense_allocations",
                newName: "amount");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                table: "expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "payment_date",
                table: "expenses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amount",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "payment_date",
                table: "expenses");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "expenses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "expense_allocations",
                newName: "value");

            migrationBuilder.AddColumn<string>(
                name: "identity_id",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_users_identity_id",
                table: "users",
                column: "identity_id",
                unique: true);
        }
    }
}
