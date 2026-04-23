Perfekt. Så er næste bedste skridt nu **README til GitHub**.

Her er en god og enkel version, du kan gemme som `README.md` i roden af projektet.

````md
# Appointment Booking System

A full-stack appointment booking system built with ASP.NET Core, Entity Framework Core, SQLite, React, and TypeScript.

## Features

- User login with JWT
- View services
- View available time slots
- Book an appointment
- Cancel a booking
- Admin slot generation
- Role-based authorization
- Business rules for past slots and cancellations

## Tech Stack

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Authentication
- Swagger

### Frontend
- React
- TypeScript
- Vite

## Project Structure

```text
appointment-booking-system
├── src
│   ├── BookingSystem.Api
│   ├── BookingSystem.Application
│   ├── BookingSystem.Domain
│   └── BookingSystem.Infrastructure
├── frontend
└── README.md
````

## Backend Features

* Authentication with login and token generation
* Service listing
* Availability lookup
* Booking creation
* Booking cancellation
* Admin-only slot generation
* Validation against booking past slots
* Status handling for bookings and time slots

## Frontend Features

* Login screen
* Dashboard UI
* Service selection
* Date selection
* View available slots
* Book slot from frontend
* View my bookings
* Cancel active bookings
* Friendly status messages

## Business Rules

* A user cannot book a slot in the past
* Only available slots can be booked
* Cancelled bookings remain in history
* Cancelled bookings do not block future rebooking
* Old bookings cannot be cancelled
* Admin endpoints require authorization

## How to Run the Backend

Go to the API project:

```bash
cd src/BookingSystem.Api
dotnet run
```

Swagger will open at:

```text
http://localhost:5000/swagger
```

## How to Run the Frontend

Go to the frontend project:

```bash
cd frontend
npm install
npm run dev
```

Frontend runs at something like:

```text
http://localhost:5173
```

or

```text
http://localhost:5174
```

## Database Setup

Run migrations from the project root:

```bash
dotnet ef database update -p .\src\BookingSystem.Infrastructure\BookingSystem.Infrastructure.csproj -s .\src\BookingSystem.Api\BookingSystem.Api.csproj
```

## Example Flow

### User Flow

1. Login
2. Choose a service
3. Choose a date
4. Load availability
5. Book a slot
6. View booking
7. Cancel booking

### Admin Flow

1. Login as admin
2. Generate slots
3. Open frontend
4. Choose date and service
5. View newly generated slots

## Screenshots

Add screenshots here, for example:

* Login page
* Dashboard
* Available slots
* My bookings

## Future Improvements

* Better admin panel
* Profile page
* Better notifications
* Integration tests
* AI assistant for booking help
* Natural language booking requests

## Author

Created as a software engineering project to demonstrate full-stack development, authentication, authorization, booking logic, and frontend-backend integration.

```


