using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoutNet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StoreFullApiFootballData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_ExternalId_TeamId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "DribblesSuccessPercentage",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "ExpectedGoals",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Goals",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "InterceptionsPerGame",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "MatchesPlayed",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PassAccuracyPercentage",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "TacklesPerGame",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "ContractUntil",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "Players",
                newName: "ExternalTeamId");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "Players",
                newName: "ExternalLeagueId");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStatistics_PlayerId_SeasonYear_LeagueId",
                table: "PlayerStatistics");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                table: "PlayerStatistics",
                newName: "LeagueExternalId");

            migrationBuilder.AddColumn<Guid>(
                name: "LeagueId",
                table: "PlayerStatistics",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Assists",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Appearances",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Blocks",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Captain",
                table: "PlayerStatistics",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DribblesAttempts",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DribblesPast",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DribblesSuccess",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuelsTotal",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DuelsWon",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoulsCommitted",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoulsDrawn",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalsConceded",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoalsTotal",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interceptions",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KeyPasses",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Lineups",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Minutes",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassAccuracy",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassesTotal",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PenaltyCommitted",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PenaltyMissed",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PenaltySaved",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PenaltyScored",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PenaltyWon",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "PlayerStatistics",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "PlayerStatistics",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedCards",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Saves",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShirtNumber",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShotsOn",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShotsTotal",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubstitutesBench",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubstitutesIn",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubstitutesOut",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TacklesTotal",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "PlayerStatistics",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "YellowCards",
                table: "PlayerStatistics",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Players",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "BirthCountry",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "Players",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthPlace",
                table: "Players",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                table: "Players",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Injured",
                table: "Players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "Players",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LeagueProfileId",
                table: "Players",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeamProfileId",
                table: "Players",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Weight",
                table: "Players",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Logo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Flag = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Logo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_LeagueId",
                table: "PlayerStatistics",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_TeamId",
                table: "PlayerStatistics",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ExternalId",
                table: "Players",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_LeagueProfileId",
                table: "Players",
                column: "LeagueProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamProfileId",
                table: "Players",
                column: "TeamProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_ExternalId",
                table: "Leagues",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ExternalId",
                table: "Teams",
                column: "ExternalId",
                unique: true);

            migrationBuilder.Sql(
                """
                UPDATE "PlayerStatistics" AS ps
                SET "LeagueId" = l."Id"
                FROM "Leagues" AS l
                WHERE ps."LeagueExternalId" = l."ExternalId";
                """);

            migrationBuilder.DropColumn(
                name: "LeagueExternalId",
                table: "PlayerStatistics");

            migrationBuilder.AlterColumn<Guid>(
                name: "LeagueId",
                table: "PlayerStatistics",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_PlayerId_SeasonYear_LeagueId",
                table: "PlayerStatistics",
                columns: new[] { "PlayerId", "SeasonYear", "LeagueId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Leagues_LeagueProfileId",
                table: "Players",
                column: "LeagueProfileId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamProfileId",
                table: "Players",
                column: "TeamProfileId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStatistics_Leagues_LeagueId",
                table: "PlayerStatistics",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStatistics_Teams_TeamId",
                table: "PlayerStatistics",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Leagues_LeagueProfileId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamProfileId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStatistics_Leagues_LeagueId",
                table: "PlayerStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStatistics_Teams_TeamId",
                table: "PlayerStatistics");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStatistics_LeagueId",
                table: "PlayerStatistics");

            migrationBuilder.DropIndex(
                name: "IX_PlayerStatistics_TeamId",
                table: "PlayerStatistics");

            migrationBuilder.DropIndex(
                name: "IX_Players_ExternalId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_LeagueProfileId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_TeamProfileId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Appearances",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Blocks",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Captain",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "DribblesAttempts",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "DribblesPast",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "DribblesSuccess",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "DuelsTotal",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "DuelsWon",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "FoulsCommitted",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "FoulsDrawn",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "GoalsConceded",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "GoalsTotal",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Interceptions",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "KeyPasses",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Lineups",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Minutes",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PassAccuracy",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PassesTotal",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PenaltyCommitted",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PenaltyMissed",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PenaltySaved",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PenaltyScored",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "PenaltyWon",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "RedCards",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "Saves",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "ShirtNumber",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "ShotsOn",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "ShotsTotal",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "SubstitutesBench",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "SubstitutesIn",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "SubstitutesOut",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "TacklesTotal",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "YellowCards",
                table: "PlayerStatistics");

            migrationBuilder.DropColumn(
                name: "BirthCountry",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "BirthPlace",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Injured",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LeagueProfileId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TeamProfileId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "ExternalTeamId",
                table: "Players",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "ExternalLeagueId",
                table: "Players",
                newName: "LeagueId");

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                table: "PlayerStatistics",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Assists",
                table: "PlayerStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DribblesSuccessPercentage",
                table: "PlayerStatistics",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ExpectedGoals",
                table: "PlayerStatistics",
                type: "double precision",
                precision: 8,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Goals",
                table: "PlayerStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "InterceptionsPerGame",
                table: "PlayerStatistics",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "MatchesPlayed",
                table: "PlayerStatistics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PassAccuracyPercentage",
                table: "PlayerStatistics",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TacklesPerGame",
                table: "PlayerStatistics",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContractUntil",
                table: "Players",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Players_ExternalId_TeamId",
                table: "Players",
                columns: new[] { "ExternalId", "TeamId" },
                unique: true);
        }
    }
}
