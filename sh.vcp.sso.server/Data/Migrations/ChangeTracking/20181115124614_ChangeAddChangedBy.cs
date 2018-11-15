using Microsoft.EntityFrameworkCore.Migrations;

namespace sh.vcp.sso.server.Migrations
{
    public partial class ChangeAddChangedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "changed_by",
                table: "Changes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "changed_by",
                table: "Changes");
        }
    }
}
