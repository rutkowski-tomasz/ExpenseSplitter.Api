using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SettlementCreatorCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "creator_user_id",
                table: "settlements",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_settlements_creator_user_id",
                table: "settlements",
                column: "creator_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_settlements_user_user_temp_id",
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
                name: "fk_settlements_user_user_temp_id",
                table: "settlements");

            migrationBuilder.DropIndex(
                name: "ix_settlements_creator_user_id",
                table: "settlements");

            migrationBuilder.DropColumn(
                name: "creator_user_id",
                table: "settlements");
        }
    }
}
