Day 2 – Core Booking Logic & Slot Generation

This day focused on implementing the core domain logic of the booking system:
services, availability, time slots, bookings, and automatic slot generation.

1️⃣ Services
Changes

Added Service domain entity

Fields:

Name

DurationMinutes

Price

IsActive

Seeded initial services in the database:

Consultation (30 min)

Teeth Cleaning (45 min)

Follow-up (15 min)

API
GET /api/services


Returns all active services, ordered by name.

2️⃣ Time Slots & Availability
Changes

Added TimeSlot entity

Added TimeSlotStatus enum:

Available

Blocked

All times stored in UTC

Seeded example time slots for testing

API
GET /api/availability?serviceId={id}&date=YYYY-MM-DD

Behavior

Converts requested date from Europe/Copenhagen to UTC

Returns all slots for the selected service and date

Indicates whether a slot is available or blocked

3️⃣ Bookings
Changes

Added Booking entity

Added BookingStatus enum:

Active

Cancelled

Added database-level unique constraint on TimeSlotId

Mocked UserId for now (authentication comes later)

Business Rules

Cannot book a slot in the past

Cannot book blocked slots

Cannot book the same slot twice

Database enforces booking uniqueness

Race conditions handled safely

APIs
Create booking
POST /api/bookings

{
  "serviceId": 1,
  "timeSlotId": 1
}


Returns 201 Created on success

Returns 409 Conflict if slot is already booked

Get my bookings
GET /api/bookings/me

Cancel booking
POST /api/bookings/{id}/cancel


Cancellation must happen at least 24 hours before slot start

4️⃣ Business Hours
Changes

Added BusinessHours entity

Fields:

DayOfWeek

OpenTime

CloseTime

Seeded weekday opening hours (Monday–Friday, 09:00–16:00)

5️⃣ Automatic Slot Generation (Admin)
Changes

Implemented slot generation logic based on:

Service duration

Business hours

Date range

Prevents duplicate slots (idempotent generation)

Logic extracted into a dedicated SlotGenerator service

API
POST /api/admin/slots/generate?serviceId=1&from=YYYY-MM-DD&to=YYYY-MM-DD

Result
{
  "created": <number of slots>
}


Generated slots become immediately available through the availability endpoint.

6️⃣ Database & Migrations
Changes

Added multiple EF Core migrations for:

Services

TimeSlots

Bookings

BusinessHours

Used SQLite with EF Core

Seed data applied via migrations

Handled:

Pending model changes

Migration conflicts

EF Core version mismatches

✅ Day 2 Result

After Day 2, the system supports:

Viewing services

Viewing available time slots

Booking appointments safely

Preventing double bookings

Cancelling bookings with business rules

Automatically generating slots from business hours

This completes the core booking workflow.