using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlots_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Id", "EndUtc", "ServiceId", "StartUtc", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 10, 8, 30, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 2, 10, 8, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 2, new DateTime(2026, 2, 10, 9, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 2, 10, 8, 30, 0, 0, DateTimeKind.Utc), 2 },
                    { 3, new DateTime(2026, 2, 10, 9, 45, 0, 0, DateTimeKind.Utc), 2, new DateTime(2026, 2, 10, 9, 0, 0, 0, DateTimeKind.Utc), 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_ServiceId",
                table: "TimeSlots",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeSlots");
        }
    }
}
