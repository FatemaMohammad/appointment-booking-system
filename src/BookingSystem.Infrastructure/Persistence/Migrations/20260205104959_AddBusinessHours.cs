using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BusinessHours",
                columns: new[] { "Id", "CloseTime", "DayOfWeek", "OpenTime" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 16, 0, 0, 0), 1, new TimeSpan(0, 9, 0, 0, 0) },
                    { 2, new TimeSpan(0, 16, 0, 0, 0), 2, new TimeSpan(0, 9, 0, 0, 0) },
                    { 3, new TimeSpan(0, 16, 0, 0, 0), 3, new TimeSpan(0, 9, 0, 0, 0) },
                    { 4, new TimeSpan(0, 16, 0, 0, 0), 4, new TimeSpan(0, 9, 0, 0, 0) },
                    { 5, new TimeSpan(0, 16, 0, 0, 0), 5, new TimeSpan(0, 9, 0, 0, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessHours");
        }
    }
}
