using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityCateringSystem.Migrations
{
    public partial class updatedb5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "OtpLoginRequests",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "OtpLoginRequests");
        }
    }
}
