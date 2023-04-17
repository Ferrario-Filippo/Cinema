using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateshowù : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is3D",
                table: "Shows",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is3D",
                table: "Shows");
        }
    }
}
