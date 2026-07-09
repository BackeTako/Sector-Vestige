using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class RemoveCDProfile_Postgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add height to profile with the application's default (1.0) so any
            //    profiles missing a cdprofile row (shouldn't happen, but defensively)
            //    still get a sensible value rather than 0-height characters.
            migrationBuilder.AddColumn<float>(
                name: "height",
                table: "profile",
                type: "real",
                nullable: false,
                defaultValue: 1f);

            // 2. Backfill height from cdprofile.height before dropping the table.
            migrationBuilder.Sql("""
                UPDATE profile
                SET height = cdprofile.height
                FROM cdprofile
                WHERE cdprofile.profile_id = profile.profile_id;
                """);

            // 3. Drop dependents first (record entries have an FK to cdprofile).
            migrationBuilder.DropTable(
                name: "cd_character_record_entries");

            migrationBuilder.DropTable(
                name: "cdprofile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate cdprofile first so we can backfill it from profile.height before
            // dropping that column. Records data is lost on downgrade (the CD records
            // subsystem was ripped; tables are recreated empty for schema parity only).
            migrationBuilder.CreateTable(
                name: "cdprofile",
                columns: table => new
                {
                    cdprofile_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    character_records = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    height = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cdprofile", x => x.cdprofile_id);
                    table.ForeignKey(
                        name: "FK_cdprofile_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Backfill: give every existing profile a cdprofile row with the height we have.
            migrationBuilder.Sql("""
                INSERT INTO cdprofile (profile_id, height, character_records)
                SELECT profile_id, height, NULL FROM profile;
                """);

            migrationBuilder.DropColumn(
                name: "height",
                table: "profile");

            migrationBuilder.CreateTable(
                name: "cd_character_record_entries",
                columns: table => new
                {
                    cd_character_record_entries_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cdprofile_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    involved = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cd_character_record_entries", x => x.cd_character_record_entries_id);
                    table.ForeignKey(
                        name: "FK_cd_character_record_entries_cdprofile_cdprofile_id",
                        column: x => x.cdprofile_id,
                        principalTable: "cdprofile",
                        principalColumn: "cdprofile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cd_character_record_entries_cd_character_record_entries_id",
                table: "cd_character_record_entries",
                column: "cd_character_record_entries_id");

            migrationBuilder.CreateIndex(
                name: "IX_cd_character_record_entries_cdprofile_id",
                table: "cd_character_record_entries",
                column: "cdprofile_id");

            migrationBuilder.CreateIndex(
                name: "IX_cdprofile_profile_id",
                table: "cdprofile",
                column: "profile_id",
                unique: true);
        }
    }
}
