using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALTW.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionAndSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Grades_GradeID",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Topics_TopicID",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "TopicID",
                table: "Documents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GradeID",
                table: "Documents",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CompetitionID",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemesterID",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Competition",
                columns: table => new
                {
                    CompetitionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competition", x => x.CompetitionID);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    SemesterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.SemesterID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompetitionID",
                table: "Documents",
                column: "CompetitionID");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_SemesterID",
                table: "Documents",
                column: "SemesterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Competition_CompetitionID",
                table: "Documents",
                column: "CompetitionID",
                principalTable: "Competition",
                principalColumn: "CompetitionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Grades_GradeID",
                table: "Documents",
                column: "GradeID",
                principalTable: "Grades",
                principalColumn: "GradeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Semesters_SemesterID",
                table: "Documents",
                column: "SemesterID",
                principalTable: "Semesters",
                principalColumn: "SemesterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Topics_TopicID",
                table: "Documents",
                column: "TopicID",
                principalTable: "Topics",
                principalColumn: "TopicID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Competition_CompetitionID",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Grades_GradeID",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Semesters_SemesterID",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Topics_TopicID",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Competition");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CompetitionID",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_SemesterID",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CompetitionID",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "SemesterID",
                table: "Documents");

            migrationBuilder.AlterColumn<int>(
                name: "TopicID",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GradeID",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Grades_GradeID",
                table: "Documents",
                column: "GradeID",
                principalTable: "Grades",
                principalColumn: "GradeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Topics_TopicID",
                table: "Documents",
                column: "TopicID",
                principalTable: "Topics",
                principalColumn: "TopicID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
