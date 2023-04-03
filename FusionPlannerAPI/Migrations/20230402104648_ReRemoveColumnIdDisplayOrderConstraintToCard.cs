using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FusionPlannerAPI.Migrations
{
    public partial class ReRemoveColumnIdDisplayOrderConstraintToCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cards_column_id_display_order",
                table: "cards");

            migrationBuilder.CreateIndex(
                name: "ix_cards_column_id",
                table: "cards",
                column: "column_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cards_column_id",
                table: "cards");

            migrationBuilder.CreateIndex(
                name: "ix_cards_column_id_display_order",
                table: "cards",
                columns: new[] { "column_id", "display_order" },
                unique: true);
        }
    }
}
