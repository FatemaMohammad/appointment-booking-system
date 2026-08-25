using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingTimeSlotActiveUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TimeSlots",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Available");

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Blocked");

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "Available");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId_ActiveOnly",
                table: "Bookings",
                column: "TimeSlotId",
                unique: true,
                filter: "\"Status\" = 'Active'");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TimeSlotId_ActiveOnly",
                table: "Bookings");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TimeSlots",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 2);

            migrationBuilder.UpdateData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TimeSlots_TimeSlotId",
                table: "Bookings",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
