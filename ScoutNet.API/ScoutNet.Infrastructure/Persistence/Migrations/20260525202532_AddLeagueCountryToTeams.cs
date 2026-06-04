using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoutNet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueCountryToTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Teams",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalLeagueId",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                WITH ranked_leagues AS (
                    SELECT
                        p."TeamProfileId" AS team_id,
                        p."ExternalLeagueId" AS external_league_id,
                        COUNT(*) AS appearances_count,
                        ROW_NUMBER() OVER (
                            PARTITION BY p."TeamProfileId"
                            ORDER BY COUNT(*) DESC, p."ExternalLeagueId"
                        ) AS row_num
                    FROM "Players" p
                    GROUP BY p."TeamProfileId", p."ExternalLeagueId"
                )
                UPDATE "Teams" t
                SET "ExternalLeagueId" = rl.external_league_id
                FROM ranked_leagues rl
                WHERE rl.team_id = t."Id" AND rl.row_num = 1;
                """);

            migrationBuilder.Sql("""
                UPDATE "Teams" t
                SET "Country" = l."Country"
                FROM "Leagues" l
                WHERE l."ExternalId" = t."ExternalLeagueId";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ExternalLeagueId",
                table: "Teams",
                column: "ExternalLeagueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teams_ExternalLeagueId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "ExternalLeagueId",
                table: "Teams");
        }
    }
}
