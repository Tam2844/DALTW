using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALTW.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Competition_CompetitionID",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Competition",
                table: "Competition");

            migrationBuilder.RenameTable(
                name: "Competition",
                newName: "Competitions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Competitions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Competitions",
                table: "Competitions",
                column: "CompetitionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Competitions_CompetitionID",
                table: "Documents",
                column: "CompetitionID",
                principalTable: "Competitions",
                principalColumn: "CompetitionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Competitions_CompetitionID",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Competitions",
                table: "Competitions");

            migrationBuilder.RenameTable(
                name: "Competitions",
                newName: "Competition");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Competition",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Competition",
                table: "Competition",
                column: "CompetitionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Competition_CompetitionID",
                table: "Documents",
                column: "CompetitionID",
                principalTable: "Competition",
                principalColumn: "CompetitionID");
        }
    }
}
