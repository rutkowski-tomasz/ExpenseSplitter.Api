using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantToSettlementId2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expenses_settlements_settlement_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlements_settlement_id",
                table: "participants");

            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlements_settlement_id1",
                table: "participants");

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_settlement_settlement_id",
                table: "expenses",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_participants_settlement_settlement_id",
                table: "participants",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_participants_settlement_settlement_id1",
                table: "participants",
                column: "settlement_id1",
                principalTable: "settlements",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expenses_settlement_settlement_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlement_settlement_id",
                table: "participants");

            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlement_settlement_id1",
                table: "participants");

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_settlements_settlement_id",
                table: "expenses",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_participants_settlements_settlement_id",
                table: "participants",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_participants_settlements_settlement_id1",
                table: "participants",
                column: "settlement_id1",
                principalTable: "settlements",
                principalColumn: "id");
        }
    }
}
