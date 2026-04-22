# Appointment Booking System

A full-stack inspired backend booking system built with **ASP.NET Core Web API**, **Entity Framework Core**, **SQLite**, and **JWT authentication**.  
The system allows users to register, log in, view services, check availability, create bookings, and lets admins generate slots.

---

## Features

### User features
- Register account
- Login with JWT
- View available services
- View available time slots for a selected service and date
- Book a time slot
- View own bookings
- Cancel own bookings

### Admin features
- Generate time slots for a service within a date range
- Access admin-only endpoints using role-based authorization

---

## Tech Stack

- **Backend:** ASP.NET Core Web API
- **Database:** SQLite
- **ORM:** Entity Framework Core
- **Authentication:** JWT Bearer Token
- **API testing:** Swagger / OpenAPI

---

## Project Structure

```text
appointment-booking-system
│
├── src
│   ├── BookingSystem.Api
│   ├── BookingSystem.Application
│   ├── BookingSystem.Domain
│   └── BookingSystem.Infrastructure
│
└── tests

## Business Rules
A user must be authenticated to create and manage bookings
A slot cannot be booked more than once
A blocked slot cannot be booked
A booking cannot be made in the past
Admin endpoints require the Admin role
Time slots are stored in UTC
Business hours define when slots can be generated

Her er en **kort og simpel README**, som du kan gemme som `README.md` og bruge direkte på GitHub 👇

---

````md
# Appointment Booking System

A simple booking system built with ASP.NET Core Web API, EF Core, SQLite, and JWT authentication.

---

## Features

- Register and login (JWT)
- View services
- See available time slots
- Book a time slot
- View own bookings
- Cancel booking
- Admin can generate slots

---

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- JWT Authentication
- Swagger (API testing)

---

## How to Run

```bash
cd src/BookingSystem.Api
dotnet run
````

Open Swagger:

```
http://localhost:5000/swagger
```

---

## How to Test (Swagger Flow)

1. **Register**
   POST `/api/auth/register`

2. **Login**
   POST `/api/auth/login`
   → Copy token

3. **Authorize**
   Click 🔒 and paste:

```
Bearer <token>
```

4. **Get services**
   GET `/api/services`

5. **Check availability**
   GET `/api/availability`

6. **Book slot**
   POST `/api/bookings`

7. **View bookings**
   GET `/api/bookings/me`

8. **Cancel booking**
   POST `/api/bookings/{id}/cancel`

---

## Admin

* Generate slots:
  POST `/api/admin/slots/generate`

* Normal user → `403 Forbidden`

* Admin → `200 OK`

---

## Important Rules

* Cannot book same slot twice
* Cannot book blocked slots
* Authentication required
* Admin endpoints require Admin role

---

## Status

Project demonstrates:

* REST API design
* JWT authentication
* Role-based authorization
* Booking logic

```

---

## Sådan gemmer du den

1. Opret fil i din root:
```

README.md

```

2. Paste teksten
3. Gem
4. Push til GitHub

---

Hvis du vil, kan jeg bagefter:
- gøre den **mere professionel til job/interview**
- eller hjælpe dig med **GitHub upload step-by-step** 🚀
```
