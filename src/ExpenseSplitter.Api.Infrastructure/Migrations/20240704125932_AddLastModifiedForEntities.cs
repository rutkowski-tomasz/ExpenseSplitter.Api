using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLastModifiedForEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_allocations_expense_expense_temp_id1",
                table: "allocations");

            migrationBuilder.DropForeignKey(
                name: "fk_allocations_participant_participant_temp_id",
                table: "allocations");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_participant_participant_temp_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_settlement_settlement_temp_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlement_settlement_temp_id",
                table: "participants");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_participants_participant_id1",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_settlements_settlement_id1",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_users_user_temp_id",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlements_users_user_temp_id",
                table: "settlements");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "settlements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "settlement_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "participants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "expenses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "last_modified",
                table: "allocations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "fk_allocations_expenses_expense_id",
                table: "allocations",
                column: "expense_id",
                principalTable: "expenses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_allocations_participants_participant_id",
                table: "allocations",
                column: "participant_id",
                principalTable: "participants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_participants_paying_participant_id",
                table: "expenses",
                column: "paying_participant_id",
                principalTable: "participants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
                name: "fk_settlement_users_participants_participant_id",
                table: "settlement_users",
                column: "participant_id",
                principalTable: "participants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_settlement_users_settlements_settlement_id",
                table: "settlement_users",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_settlement_users_users_user_id",
                table: "settlement_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_settlements_users_creator_user_id",
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
                name: "fk_allocations_expenses_expense_id",
                table: "allocations");

            migrationBuilder.DropForeignKey(
                name: "fk_allocations_participants_participant_id",
                table: "allocations");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_participants_paying_participant_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_settlements_settlement_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_participants_settlements_settlement_id",
                table: "participants");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_participants_participant_id",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_settlements_settlement_id",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlement_users_users_user_id",
                table: "settlement_users");

            migrationBuilder.DropForeignKey(
                name: "fk_settlements_users_creator_user_id",
                table: "settlements");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "settlements");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "settlement_users");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "participants");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "expenses");

            migrationBuilder.DropColumn(
                name: "last_modified",
                table: "allocations");

            migrationBuilder.AddForeignKey(
                name: "fk_allocations_expense_expense_temp_id1",
                table: "allocations",
                column: "expense_id",
                principalTable: "expenses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_allocations_participant_participant_temp_id",
                table: "allocations",
                column: "participant_id",
                principalTable: "participants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_participant_participant_temp_id",
                table: "expenses",
                column: "paying_participant_id",
                principalTable: "participants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_settlement_settlement_temp_id",
                table: "expenses",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_participants_settlement_settlement_temp_id",
                table: "participants",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_settlement_users_participants_participant_id1",
                table: "settlement_users",
                column: "participant_id",
                principalTable: "participants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_settlement_users_settlements_settlement_id1",
                table: "settlement_users",
                column: "settlement_id",
                principalTable: "settlements",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
