using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SettlementUserCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "settlement_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    settlement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    participant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settlement_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_settlement_users_participants_participant_id1",
                        column: x => x.participant_id,
                        principalTable: "participants",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_settlement_users_settlements_settlement_id1",
                        column: x => x.settlement_id,
                        principalTable: "settlements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_settlement_users_user_user_temp_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_settlement_users_participant_id",
                table: "settlement_users",
                column: "participant_id");

            migrationBuilder.CreateIndex(
                name: "ix_settlement_users_settlement_id",
                table: "settlement_users",
                column: "settlement_id");

            migrationBuilder.CreateIndex(
                name: "ix_settlement_users_user_id",
                table: "settlement_users",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settlement_users");
        }
    }
}
