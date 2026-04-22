# Cancel Booking Flow (Swagger)

1. **Login**
   POST /api/auth/login → kopiér token

2. **Authorize**
   Swagger → klik "Authorize" → indsæt:
   Bearer <token>

3. **Generate slots (Admin)**
   POST /api/admin/slots/generate
   (serviceId, from, to i fremtiden)

4. **Find available slot**
   GET /api/availability
   → kopiér `timeSlotId`

5. **Opret booking**
   POST /api/bookings
   {
   "serviceId": X,
   "timeSlotId": Y
   }

6. **Hent booking**
   GET /api/bookings/me
   → kopiér `bookingId`

7. **Cancel booking**
   POST /api/bookings/{bookingId}/cancel

8. **Verify**
   GET /api/bookings/me
   → status = "Cancelled"
