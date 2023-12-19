using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseAllocationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_allocations_expense_expense_temp_id",
                table: "allocations");

            migrationBuilder.AddForeignKey(
                name: "fk_allocations_expense_expense_temp_id1",
                table: "allocations",
                column: "expense_id",
                principalTable: "expenses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_allocations_expense_expense_temp_id1",
                table: "allocations");

            migrationBuilder.AddForeignKey(
                name: "fk_allocations_expense_expense_temp_id",
                table: "allocations",
                column: "expense_id",
                principalTable: "expenses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
