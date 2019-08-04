using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Residuum.Services.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildMembers",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Realm = table.Column<string>(nullable: true),
                    Class = table.Column<string>(nullable: true),
                    Rank = table.Column<int>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMembers", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Mythic",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DungeonShortName = table.Column<string>(nullable: true),
                    DungeonName = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    Upgrades = table.Column<int>(nullable: false),
                    ProfileUri = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mythic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgressionDetails",
                columns: table => new
                {
                    Summary = table.Column<string>(nullable: false),
                    TotalBosses = table.Column<int>(nullable: false),
                    NormalBossesKilled = table.Column<int>(nullable: false),
                    HeroicBossesKilled = table.Column<int>(nullable: false),
                    MythicBossesKilled = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressionDetails", x => x.Summary);
                });

            migrationBuilder.CreateTable(
                name: "RaidProgress",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Faction = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Realm = table.Column<string>(nullable: true),
                    URL = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaidProgress", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "BestMythicRuns",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    MythicRunId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BestMythicRuns", x => x.Name);
                    table.ForeignKey(
                        name: "FK_BestMythicRuns_Mythic_MythicRunId",
                        column: x => x.MythicRunId,
                        principalTable: "Mythic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Progression",
                columns: table => new
                {
                    RaidName = table.Column<string>(nullable: false),
                    DetailsSummary = table.Column<string>(nullable: true),
                    RaidProgressName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progression", x => x.RaidName);
                    table.ForeignKey(
                        name: "FK_Progression_ProgressionDetails_DetailsSummary",
                        column: x => x.DetailsSummary,
                        principalTable: "ProgressionDetails",
                        principalColumn: "Summary",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Progression_RaidProgress_RaidProgressName",
                        column: x => x.RaidProgressName,
                        principalTable: "RaidProgress",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BestMythicRuns_MythicRunId",
                table: "BestMythicRuns",
                column: "MythicRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Progression_DetailsSummary",
                table: "Progression",
                column: "DetailsSummary");

            migrationBuilder.CreateIndex(
                name: "IX_Progression_RaidProgressName",
                table: "Progression",
                column: "RaidProgressName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BestMythicRuns");

            migrationBuilder.DropTable(
                name: "GuildMembers");

            migrationBuilder.DropTable(
                name: "Progression");

            migrationBuilder.DropTable(
                name: "Mythic");

            migrationBuilder.DropTable(
                name: "ProgressionDetails");

            migrationBuilder.DropTable(
                name: "RaidProgress");
        }
    }
}
