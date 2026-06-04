using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoutNet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamIdAndStatisticsLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerStatistics_PlayerId_SeasonYear",
                table: "PlayerStatistics");

            migrationBuilder.DropIndex(
                name: "IX_Players_ExternalId_LeagueId",
                table: "Players");

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "PlayerStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_PlayerId_SeasonYear_LeagueId",
                table: "PlayerStatistics",
                columns: new[] { "PlayerId", "SeasonYear", "LeagueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ExternalId_TeamId",
                table: "Players",
                columns: new[] { "ExternalId", "TeamId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerStatistics_PlayerId_SeasonYear_LeagueId",
                table: "PlayerStatistics");

            migrationBuilder.DropIndex(
                name: "IX_Players_ExternalId_TeamId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Players");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_PlayerId_SeasonYear",
                table: "PlayerStatistics",
                columns: new[] { "PlayerId", "SeasonYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ExternalId_LeagueId",
                table: "Players",
                columns: new[] { "ExternalId", "LeagueId" },
                unique: true);
        }
    }
}
