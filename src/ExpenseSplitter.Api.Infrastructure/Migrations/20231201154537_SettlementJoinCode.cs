using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseSplitter.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SettlementJoinCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "invite_code",
                table: "settlements",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_settlements_invite_code",
                table: "settlements",
                column: "invite_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_settlements_invite_code",
                table: "settlements");

            migrationBuilder.DropColumn(
                name: "invite_code",
                table: "settlements");
        }
    }
}
