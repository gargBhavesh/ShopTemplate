using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopTemplate.Migrations
{
    public partial class AddProfilePicToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicName",
                table: "Users");
        }
    }
}
