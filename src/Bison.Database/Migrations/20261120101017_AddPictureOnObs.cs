using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPictureOnObs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PicturePath",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PicturePath",
                table: "Posts");
        }
    }
}
