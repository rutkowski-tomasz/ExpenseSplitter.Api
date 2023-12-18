using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdFromParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_participants_user_user_temp_id",
                table: "participants");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_user_user_temp_id",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlements_user_user_temp_id",
                table: "settlements");

            migrationBuilder.DropIndex(
                name: "ix_participants_user_id",
                table: "participants");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "participants");

            migrationBuilder.AddForeignKey(
                name: "fk_settlement_users_users_user_temp_id",
                table: "settlement_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_settlements_users_user_temp_id",
                table: "settlements",
                column: "creator_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_users_user_temp_id",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlements_users_user_temp_id",
                table: "settlements");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "participants",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_participants_user_id",
                table: "participants",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_participants_user_user_temp_id",
                table: "participants",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_settlement_users_user_user_temp_id",
                table: "settlement_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_settlements_user_user_temp_id",
                table: "settlements",
                column: "creator_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
