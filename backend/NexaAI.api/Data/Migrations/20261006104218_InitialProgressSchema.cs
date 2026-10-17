using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexaAI.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialProgressSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgressEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntryDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    WorkCompleted = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Blockers = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgressPriorities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProgressEntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressPriorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressPriorities_ProgressEntries_ProgressEntryId",
                        column: x => x.ProgressEntryId,
                        principalTable: "ProgressEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProgressEntryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressTasks_ProgressEntries_ProgressEntryId",
                        column: x => x.ProgressEntryId,
                        principalTable: "ProgressEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressEntries_UserId_EntryDate",
                table: "ProgressEntries",
                columns: new[] { "UserId", "EntryDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgressPriorities_ProgressEntryId",
                table: "ProgressPriorities",
                column: "ProgressEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressTasks_ProgressEntryId",
                table: "ProgressTasks",
                column: "ProgressEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressPriorities");

            migrationBuilder.DropTable(
                name: "ProgressTasks");

            migrationBuilder.DropTable(
                name: "ProgressEntries");
        }
    }
}
