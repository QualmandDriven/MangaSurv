using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MangaSurvWebApi.Migrations
{
    public partial class RequiredMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Mangas",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FileSystemName",
                table: "Mangas",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Files",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Files",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Chapters",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Mangas",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileSystemName",
                table: "Mangas",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Files",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Files",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Chapters",
                nullable: true);
        }
    }
}
