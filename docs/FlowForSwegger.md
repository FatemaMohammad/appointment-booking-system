Her er et simpelt flow i **Markdown**, som du kan gemme i en fil, fx `SWAGGER-TEST-FLOW.md`.

````md
# Swagger Test Flow – Appointment Booking System

## Start backend
Gå til API-mappen og kør projektet:

```bash
cd src/BookingSystem.Api
dotnet run
````

Åbn derefter Swagger:

```text
http://localhost:5000/swagger
```

---

## 1. Opret en bruger

Kør:

**POST** `/api/auth/register`

Eksempel:

```json
{
  "email": "user1@example.com",
  "password": "123456"
}
```

Forventet:

* `200 OK`

---

## 2. Log ind

Kør:

**POST** `/api/auth/login`

Brug samme email og password.

Forventet:

```json
{
  "token": "eyJhbGciOi..."
}
```

Kopiér token.

---

## 3. Authorize i Swagger

Klik på **Authorize** i Swagger.

Indsæt:

```text
Bearer <din_token>
```

Klik:

* **Authorize**
* **Close**

Nu er du logget ind i Swagger.

---

## 4. Hent services

Kør:

**GET** `/api/services`

Forventet:

* liste med services

Eksempel:

* `1 = Consultation`
* `2 = Teeth Cleaning`
* `3 = Follow-up`

Notér den `id`, du vil bruge som `serviceId`.

---

## 5. Tjek availability

Kør:

**GET** `/api/availability`

Eksempel:

* `serviceId = 2`
* `date = 2026-05-01`

Forventet:

* liste med slots
* find et slot med:

  * `"status": "Available"`

Notér slot `id` som `timeSlotId`.

---

## 6. Book en tid

Kør:

**POST** `/api/bookings`

Eksempel:

```json
{
  "serviceId": 2,
  "timeSlotId": 15
}
```

Forventet:

* `200` eller `201`

Hvis du får fejl:

* `401` = ikke logget ind
* `404` = slot matcher ikke service
* `400` = slot er i fortiden eller ikke tilgængelig
* `409` = slot er allerede booket

---

## 7. Se mine bookinger

Kør:

**GET** `/api/bookings/me`

Forventet:

* liste med dine bookinger

Notér booking `id`, hvis du vil aflyse senere.

---

## 8. Aflys booking

Kør:

**POST** `/api/bookings/{id}/cancel`

Eksempel:

```text
/api/bookings/1/cancel
```

Forventet:

* `200 OK`

Mulige fejl:

* `400` = mindre end 24 timer til start
* `404` = booking findes ikke

---

## 9. Test dobbelt-booking

Prøv at sende samme booking-request igen:

**POST** `/api/bookings`

Eksempel:

```json
{
  "serviceId": 2,
  "timeSlotId": 15
}
```

Forventet:

* conflict / fejl

Det beviser, at dobbelt-booking ikke er tilladt.

---

# Admin Flow

## 10. Test admin-endpoint som normal bruger

Kør:

**POST** `/api/admin/slots/generate`

Eksempel:

* `serviceId = 2`
* `from = 2026-05-01`
* `to = 2026-05-01`

Forventet:

* `403 Forbidden`

Det betyder:

* token virker
* men brugeren er ikke admin

---

## 11. Gør bruger til admin

Kør:

**POST** `/api/admin/users/promote?email=user1@example.com`

Forventet:

* `200 OK`

---

## 12. Log ind igen

Kør:

**POST** `/api/auth/login`

igen med samme bruger.

Kopiér den **nye token**.

Vigtigt:

* den gamle token har stadig gammel rolle
* du skal bruge en ny token efter promotion

---

## 13. Authorize igen med ny token

Klik **Authorize** og indsæt:

```text
Bearer <ny_admin_token>
```

---

## 14. Generér slots som admin

Kør:

**POST** `/api/admin/slots/generate`

Eksempel:

* `serviceId = 2`
* `from = 2026-05-01`
* `to = 2026-05-01`

Forventet:

```json
{
  "created": 5
}
```

eller et andet tal større end 0.

Hvis du får `403`, så bruger du stadig ikke en admin-token.

---

## 15. Tjek availability igen

Kør:

**GET** `/api/availability`

Eksempel:

* `serviceId = 2`
* `date = 2026-05-01`

Forventet:

* nu kommer der slots i listen

---

## 16. Book et nyt genereret slot

Kør:

**POST** `/api/bookings`

Brug:

* samme `serviceId`
* et `timeSlotId` fra availability-listen
* kun et slot med `"status": "Available"`

Forventet:

* success

---

# Fejlkoder – hurtig huskeliste

* `200 OK` = virker
* `201 Created` = oprettet
* `400 Bad Request` = ugyldig request / business rule brudt
* `401 Unauthorized` = ikke logget ind / token mangler
* `403 Forbidden` = logget ind, men ikke tilladt
* `404 Not Found` = resource findes ikke
* `409 Conflict` = fx dobbelt-booking

---

# Kort version

## User flow

1. register
2. login
3. authorize
4. services
5. availability
6. bookings
7. bookings/me
8. cancel

## Admin flow

1. promote user
2. login igen
3. authorize med ny token
4. admin generate
5. availability
6. bookings

---

# Vigtigt at huske

* `serviceId` får du fra `/api/services`
* `timeSlotId` får du fra `/api/availability`
* efter promotion til admin skal du altid **login igen**
* hvis du får `401`, mangler token
* hvis du får `403`, er rollen forkert

```

