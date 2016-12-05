using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MangaSurvWebApi.Migrations
{
    public partial class UserMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Mangas",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Chapters",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_UserId",
                table: "Mangas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_UserId",
                table: "Chapters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Users_UserId",
                table: "Chapters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mangas_Users_UserId",
                table: "Mangas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Users_UserId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_Mangas_Users_UserId",
                table: "Mangas");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Mangas_UserId",
                table: "Mangas");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_UserId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Chapters");
        }
    }
}
