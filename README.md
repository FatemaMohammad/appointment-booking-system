# Appointment Booking System

A full-stack appointment booking system built with **.NET (C#)** and **React (TypeScript)**.

The system allows users to:

* Log in securely using JWT authentication
* View available services
* Check available time slots
* Create bookings
* Cancel bookings

---

## Architecture

This project follows a **clean architecture / domain-driven design approach**:

* **Domain** – Core business logic and entities
* **Application** – Use cases and business rules
* **Infrastructure** – Database (EF Core + SQLite)
* **API** – REST endpoints (ASP.NET Core)
* **Frontend** – React (TypeScript + Vite)

---

## Authentication

* JWT-based authentication
* Secure API endpoints using `[Authorize]`
* Token is used for all protected requests

---

## Features

### User Features

* Login
* View services
* Load availability for a selected date
* Book available slots
* Cancel bookings

### Booking Logic

* Cannot book a slot in the past
* Only "Available" slots can be booked
* Cancelled bookings do not block the slot
* Booking status:

  * Active
  * Cancelled

### Business Rules

* Time slots are generated based on service duration
* Slot conflicts are prevented in backend logic
* Cancellation updates booking status instead of deleting

---

## Database

* SQLite database using **Entity Framework Core**
* Seeded data includes:

  * Services (Consultation, Teeth Cleaning, Follow-up)
  * Business hours
  * Example time slots

---

## Getting Started

### 1. Run database migration

```bash
dotnet ef database update -p .\src\BookingSystem.Infrastructure\BookingSystem.Infrastructure.csproj -s .\src\BookingSystem.Api\BookingSystem.Api.csproj
```

### 2. Run backend

```bash
dotnet run --project src/BookingSystem.Api
```

Backend will run on:

```
http://localhost:5000
```

### 3. Run frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend will run on:

```
http://localhost:5173
```

---

## API Endpoints (examples)

* `POST /api/auth/login`
* `GET /api/services`
* `GET /api/availability`
* `POST /api/bookings`
* `POST /api/bookings/{id}/cancel`
* `GET /api/bookings/me`

---

## UI Overview

The frontend includes:

* Login page
* Dashboard
* Service selection
* Available slots list
* Booking management

---

## Example Flow

1. Login
2. Select a service
3. Choose a date
4. Load available slots
5. Book a slot
6. View booking in "My Bookings"
7. Cancel booking

---

## Notes

This project focuses on:

* Clean backend architecture
* Real-world booking logic
* Full frontend-backend integration

---

## Author

Fatema Mohammad


