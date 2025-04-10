using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DALTW.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Documents");
        }
    }
}
