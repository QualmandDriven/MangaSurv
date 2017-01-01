using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MangaSurvWebApi.Migrations
{
    public partial class datemigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EnterDate",
                table: "Mangas",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Mangas",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Episodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Chapters",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnterDate",
                table: "Animes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                table: "Animes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "UserFollowAnimes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AnimeId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollowAnimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFollowAnimes_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFollowAnimes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNewEpisodes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EpisodeId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNewEpisodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNewEpisodes_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNewEpisodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowAnimes_AnimeId",
                table: "UserFollowAnimes",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollowAnimes_UserId",
                table: "UserFollowAnimes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNewEpisodes_EpisodeId",
                table: "UserNewEpisodes",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNewEpisodes_UserId",
                table: "UserNewEpisodes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollowAnimes");

            migrationBuilder.DropTable(
                name: "UserNewEpisodes");

            migrationBuilder.DropColumn(
                name: "EnterDate",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "EnterDate",
                table: "Animes");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "Animes");
        }
    }
}
