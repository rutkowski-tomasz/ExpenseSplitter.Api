using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpenseTotalAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "settlement_id1",
                table: "participants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_participants_settlement_id1",
                table: "participants",
                column: "settlement_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_participants_settlements_settlement_id1",
                table: "participants",
                column: "settlement_id1",
                principalTable: "settlements",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlements_settlement_id1",
                table: "participants");

            migrationBuilder.DropIndex(
                name: "ix_participants_settlement_id1",
                table: "participants");

            migrationBuilder.DropColumn(
                name: "settlement_id1",
                table: "participants");
        }
    }
}
