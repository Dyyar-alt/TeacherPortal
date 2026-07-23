using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherPortal.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddChatIdToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "Students",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Students");
        }
    }
}
