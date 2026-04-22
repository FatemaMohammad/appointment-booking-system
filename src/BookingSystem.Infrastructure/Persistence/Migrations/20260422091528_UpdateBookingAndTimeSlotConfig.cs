using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingAndTimeSlotConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_ServiceId",
                table: "TimeSlots");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_ServiceId_StartUtc",
                table: "TimeSlots",
                columns: new[] { "ServiceId", "StartUtc" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_ServiceId_StartUtc",
                table: "TimeSlots");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_ServiceId",
                table: "TimeSlots",
                column: "ServiceId");
        }
    }
}
