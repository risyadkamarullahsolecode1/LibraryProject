using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatetableborrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Penalty",
                table: "BookBorrows",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Penalty",
                table: "BookBorrows");
        }
    }
}
