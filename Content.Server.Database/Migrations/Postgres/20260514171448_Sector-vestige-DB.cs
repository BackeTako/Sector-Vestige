using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class SectorvestigeDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sv_profiles",
                columns: table => new
                {
                    profile_id = table.Column<int>(type: "integer", nullable: false),
                    player_name = table.Column<string>(type: "text", nullable: false),
                    character_name = table.Column<string>(type: "text", nullable: false),
                    character_document_general = table.Column<JsonDocument>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sv_profiles", x => x.profile_id);
                    table.ForeignKey(
                        name: "FK_sv_profiles_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sv_character_document_entries",
                columns: table => new
                {
                    doc_i_d = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    doc_type = table.Column<int>(type: "integer", nullable: false),
                    doc_date_last_edited = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    doc_title = table.Column<string>(type: "text", nullable: false),
                    doc_author = table.Column<string>(type: "text", nullable: false),
                    doc_last_edited_by = table.Column<string>(type: "text", nullable: false),
                    doc_content = table.Column<string>(type: "text", nullable: false),
                    doc_stamps = table.Column<string>(type: "text", nullable: false),
                    profile_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sv_character_document_entries", x => x.doc_i_d);
                    table.ForeignKey(
                        name: "FK_sv_character_document_entries_sv_profiles__sv_profile_temp_id",
                        column: x => x.profile_id,
                        principalTable: "sv_profiles",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sv_character_document_entries_doc_i_d",
                table: "sv_character_document_entries",
                column: "doc_i_d");

            migrationBuilder.CreateIndex(
                name: "IX_sv_character_document_entries_profile_id",
                table: "sv_character_document_entries",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sv_character_document_entries");

            migrationBuilder.DropTable(
                name: "sv_profiles");
        }
    }
}
