using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace billingservice.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeterReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MeterId = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousDay = table.Column<double>(type: "REAL", nullable: false),
                    PreviousNight = table.Column<double>(type: "REAL", nullable: false),
                    CurrentDay = table.Column<double>(type: "REAL", nullable: false),
                    CurrentNight = table.Column<double>(type: "REAL", nullable: false),
                    ReadingDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_MeterId",
                table: "MeterReadings",
                column: "MeterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeterReadings");
        }
    }
}
