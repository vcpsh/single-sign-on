using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace sh.vcp.sso.server.Migrations
{
    public partial class AddChangeContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "change_context",
                table: "Changes",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "change_context",
                table: "Changes");
        }
    }
}
