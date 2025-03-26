using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALTW.Migrations
{
    /// <inheritdoc />
    public partial class Thu2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Documents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
