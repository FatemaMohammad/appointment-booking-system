import { useEffect, useMemo, useState } from "react";
import { apiFetch } from "./api";

type Service = {
  id: number;
  name: string;
  durationMinutes: number;
  price: number;
  isActive: boolean;
};

type TimeSlot = {
  id: number;
  startUtc: string;
  endUtc: string;
  status: string;
};

type Booking = {
  id: number;
  serviceId: number;
  timeSlotId: number;
  startUtc: string;
  endUtc: string;
  status: string;
  createdAtUtc: string;
  cancelledAtUtc: string | null;
};

export default function App() {
  const [email, setEmail] = useState("hello111@gmail.com");
  const [password, setPassword] = useState("hello111");
  const [message, setMessage] = useState("");
  const [token, setToken] = useState("");

  const [services, setServices] = useState<Service[]>([]);
  const [selectedServiceId, setSelectedServiceId] = useState<number | "">("");
  const [selectedDate, setSelectedDate] = useState("");
  const [slots, setSlots] = useState<TimeSlot[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);

  const [loadingSlots, setLoadingSlots] = useState(false);
  const [loadingBookings, setLoadingBookings] = useState(false);
  const [loggingIn, setLoggingIn] = useState(false);
  const [bookingSlotId, setBookingSlotId] = useState<number | null>(null);
  const [cancellingBookingId, setCancellingBookingId] = useState<number | null>(null);

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setMessage("");

    try {
      setLoggingIn(true);

      const data = await apiFetch<{ token: string }>(
        `/api/auth/login?email=${encodeURIComponent(email)}&password=${encodeURIComponent(password)}`,
        "",
        { method: "POST" }
      );

      setToken(data.token);
      setMessage("✅ Login successful.");
    } catch (error) {
      setMessage(error instanceof Error ? `❌ ${error.message}` : "❌ Login failed.");
    } finally {
      setLoggingIn(false);
    }
  }

  function handleLogout() {
    setToken("");
    setServices([]);
    setSlots([]);
    setBookings([]);
    setSelectedServiceId("");
    setSelectedDate("");
    setMessage("Logged out.");
  }

  async function loadServices() {
    try {
      const servicesData = await apiFetch<Service[]>("/api/services", token);
      setServices(servicesData);
    } catch (error) {
      setMessage(error instanceof Error ? `❌ ${error.message}` : "❌ Failed to load services.");
    }
  }

  async function loadBookings() {
    try {
      setLoadingBookings(true);
      const bookingsData = await apiFetch<Booking[]>("/api/bookings/me", token);
      setBookings(bookingsData);
    } catch (error) {
      setMessage(error instanceof Error ? `❌ ${error.message}` : "❌ Failed to load bookings.");
    } finally {
      setLoadingBookings(false);
    }
  }

  useEffect(() => {
    if (!token) return;

    loadServices();
    loadBookings();
  }, [token]);

  async function handleLoadAvailability() {
    if (!selectedServiceId || !selectedDate) {
      setMessage("Choose a service and a date first.");
      return;
    }

    try {
      setLoadingSlots(true);
      setMessage("");

      const data = await apiFetch<TimeSlot[]>(
        `/api/availability?serviceId=${selectedServiceId}&date=${selectedDate}`,
        token
      );

      setSlots(data);
    } catch (error) {
      setMessage(
        error instanceof Error ? `❌ ${error.message}` : "❌ Failed to load availability."
      );
    } finally {
      setLoadingSlots(false);
    }
  }

  async function book(timeSlotId: number) {
    if (!selectedServiceId) {
      setMessage("Choose a service first.");
      return;
    }

    try {
      setBookingSlotId(timeSlotId);
      setMessage("");

      const res = await fetch("http://localhost:5000/api/Bookings", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          serviceId: selectedServiceId,
          timeSlotId,
        }),
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Booking failed.");
      }

      setMessage("✅ Booking created successfully.");
      await handleLoadAvailability();
      await loadBookings();
    } catch (error) {
      setMessage(error instanceof Error ? `❌ ${error.message}` : "❌ Booking failed.");
    } finally {
      setBookingSlotId(null);
    }
  }

  async function cancelBooking(bookingId: number) {
    try {
      setCancellingBookingId(bookingId);
      setMessage("");

      const res = await fetch(`http://localhost:5000/api/Bookings/${bookingId}/cancel`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Cancel failed.");
      }

      setMessage("✅ Booking cancelled successfully.");
      await loadBookings();

      if (selectedServiceId && selectedDate) {
        await handleLoadAvailability();
      }
    } catch (error) {
      setMessage(error instanceof Error ? `❌ ${error.message}` : "❌ Cancel failed.");
    } finally {
      setCancellingBookingId(null);
    }
  }

  const serviceMap = useMemo(() => {
    return new Map(services.map((service) => [service.id, service]));
  }, [services]);

  const sortedBookings = useMemo(() => {
    return [...bookings].sort(
      (a, b) => new Date(a.startUtc).getTime() - new Date(b.startUtc).getTime()
    );
  }, [bookings]);

  function formatDateTime(value: string) {
    return new Date(value).toLocaleString(undefined, {
      dateStyle: "medium",
      timeStyle: "short",
    });
  }

  function isAvailableStatus(status: string) {
    const normalized = status.toLowerCase();
    return normalized === "available" || normalized === "0";
  }

  function isBlockedStatus(status: string) {
    const normalized = status.toLowerCase();
    return normalized === "blocked" || normalized === "2";
  }

  function isActiveStatus(status: string) {
    return status.toLowerCase() === "active";
  }

  function isCancelledStatus(status: string) {
    return status.toLowerCase() === "cancelled";
  }

  function isPastDate(dateValue: string) {
    return new Date(dateValue).getTime() <= Date.now();
  }

  function getStatusBadgeStyle(status: string): React.CSSProperties {
    if (isAvailableStatus(status)) {
      return {
        background: "#dcfce7",
        color: "#166534",
      };
    }

    if (isBlockedStatus(status)) {
      return {
        background: "#fee2e2",
        color: "#991b1b",
      };
    }

    if (isActiveStatus(status)) {
      return {
        background: "#dbeafe",
        color: "#1d4ed8",
      };
    }

    if (isCancelledStatus(status)) {
      return {
        background: "#e5e7eb",
        color: "#374151",
      };
    }

    return {
      background: "#f3f4f6",
      color: "#374151",
    };
  }

  function getStatusLabel(status: string) {
    if (isAvailableStatus(status)) return "Available";
    if (isBlockedStatus(status)) return "Blocked";
    if (isActiveStatus(status)) return "Active";
    if (isCancelledStatus(status)) return "Cancelled";
    return status;
  }

  const styles = {
    page: {
      minHeight: "100vh",
      background: "linear-gradient(180deg, #f8fafc 0%, #eef2ff 100%)",
      fontFamily: "Inter, Arial, sans-serif",
      color: "#111827",
      paddingBottom: 40,
    } as React.CSSProperties,
    nav: {
      maxWidth: 1150,
      margin: "0 auto",
      padding: "24px 20px 12px",
      display: "flex",
      justifyContent: "space-between",
      alignItems: "center",
    } as React.CSSProperties,
    brand: {
      fontSize: 22,
      fontWeight: 800,
      letterSpacing: "-0.4px",
    } as React.CSSProperties,
    navRight: {
      color: "#6b7280",
      fontSize: 14,
    } as React.CSSProperties,
    container: {
      maxWidth: 1150,
      margin: "0 auto",
      padding: "0 20px",
    } as React.CSSProperties,
    hero: {
      textAlign: "center" as const,
      padding: "24px 0 28px",
    } as React.CSSProperties,
    title: {
      fontSize: "56px",
      lineHeight: 1.05,
      fontWeight: 800,
      margin: 0,
      letterSpacing: "-1.5px",
    } as React.CSSProperties,
    subtitle: {
      marginTop: 16,
      color: "#6b7280",
      fontSize: 18,
    } as React.CSSProperties,
    loginCard: {
      maxWidth: 480,
      margin: "0 auto",
      background: "#ffffff",
      borderRadius: 24,
      padding: 28,
      boxShadow: "0 20px 60px rgba(15, 23, 42, 0.08)",
      border: "1px solid rgba(226,232,240,0.8)",
    } as React.CSSProperties,
    dashboardHeader: {
      background: "#ffffff",
      borderRadius: 24,
      padding: 24,
      boxShadow: "0 20px 60px rgba(15, 23, 42, 0.08)",
      border: "1px solid rgba(226,232,240,0.8)",
      marginBottom: 20,
      display: "flex",
      justifyContent: "space-between",
      alignItems: "center",
      flexWrap: "wrap" as const,
      gap: 12,
    } as React.CSSProperties,
    dashboardTitle: {
      fontSize: 26,
      fontWeight: 800,
      margin: 0,
    } as React.CSSProperties,
    dashboardSub: {
      marginTop: 6,
      color: "#6b7280",
    } as React.CSSProperties,
    grid: {
      display: "grid",
      gridTemplateColumns: "1.05fr 0.95fr",
      gap: 20,
    } as React.CSSProperties,
    column: {
      display: "flex",
      flexDirection: "column" as const,
      gap: 20,
    } as React.CSSProperties,
    card: {
      background: "#ffffff",
      borderRadius: 24,
      padding: 24,
      boxShadow: "0 20px 60px rgba(15, 23, 42, 0.08)",
      border: "1px solid rgba(226,232,240,0.8)",
    } as React.CSSProperties,
    cardTitle: {
      fontSize: 24,
      fontWeight: 800,
      margin: "0 0 18px 0",
    } as React.CSSProperties,
    label: {
      display: "block",
      marginBottom: 8,
      fontWeight: 600,
      color: "#374151",
    } as React.CSSProperties,
    input: {
      width: "100%",
      padding: "14px 16px",
      borderRadius: 14,
      border: "1px solid #d1d5db",
      fontSize: 16,
      outline: "none",
      boxSizing: "border-box" as const,
      background: "#fff",
    } as React.CSSProperties,
    row: {
      display: "flex",
      gap: 12,
      flexWrap: "wrap" as const,
      alignItems: "center",
    } as React.CSSProperties,
    select: {
      padding: "14px 16px",
      borderRadius: 14,
      border: "1px solid #d1d5db",
      fontSize: 16,
      minWidth: 250,
      background: "#fff",
    } as React.CSSProperties,
    primaryButton: {
      padding: "14px 18px",
      borderRadius: 14,
      border: "none",
      background: "linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%)",
      color: "#fff",
      fontWeight: 700,
      fontSize: 14,
      cursor: "pointer",
      boxShadow: "0 10px 24px rgba(37, 99, 235, 0.22)",
    } as React.CSSProperties,
    secondaryButton: {
      padding: "12px 16px",
      borderRadius: 14,
      border: "1px solid #d1d5db",
      background: "#fff",
      color: "#111827",
      fontWeight: 700,
      fontSize: 14,
      cursor: "pointer",
    } as React.CSSProperties,
    message: {
      marginTop: 14,
      padding: "14px 16px",
      borderRadius: 14,
      background: "#eef2ff",
      color: "#3730a3",
      fontWeight: 600,
      border: "1px solid #c7d2fe",
    } as React.CSSProperties,
    empty: {
      color: "#6b7280",
      margin: 0,
    } as React.CSSProperties,
    items: {
      display: "grid",
      gap: 14,
    } as React.CSSProperties,
    itemCard: {
      background: "#f8fafc",
      border: "1px solid #e5e7eb",
      borderRadius: 18,
      padding: 18,
    } as React.CSSProperties,
    itemTitle: {
      fontWeight: 800,
      marginBottom: 10,
      fontSize: 17,
    } as React.CSSProperties,
    badge: {
      display: "inline-block",
      padding: "6px 12px",
      borderRadius: 999,
      fontSize: 12,
      fontWeight: 800,
      marginTop: 12,
    } as React.CSSProperties,
    fieldLine: {
      marginBottom: 6,
      color: "#374151",
    } as React.CSSProperties,
    mutedText: {
      color: "#6b7280",
      fontSize: 14,
      marginTop: 8,
    } as React.CSSProperties,
  };

  return (
    <div style={styles.page}>
      <div style={styles.nav}>
        <div style={styles.brand}>Appointment Booking</div>
        <div style={styles.navRight}>{token ? "Authenticated session" : "Please sign in"}</div>
      </div>

      <div style={styles.container}>
        <div style={styles.hero}>
          <h1 style={styles.title}>Appointment Booking System</h1>
          <p style={styles.subtitle}>
            Manage services, find free slots, create bookings, and cancel them easily.
          </p>
        </div>

        {!token ? (
          <div style={styles.loginCard}>
            <h2 style={styles.cardTitle}>Login</h2>

            <form onSubmit={handleLogin}>
              <div style={{ marginBottom: 16 }}>
                <label style={styles.label}>Email</label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  style={styles.input}
                />
              </div>

              <div style={{ marginBottom: 20 }}>
                <label style={styles.label}>Password</label>
                <input
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  style={styles.input}
                />
              </div>

              <button
                type="submit"
                style={{ ...styles.primaryButton, opacity: loggingIn ? 0.7 : 1 }}
                disabled={loggingIn}
              >
                {loggingIn ? "Logging in..." : "Login"}
              </button>
            </form>

            {message && <div style={styles.message}>{message}</div>}
          </div>
        ) : (
          <>
            <div style={styles.dashboardHeader}>
              <div>
                <h2 style={styles.dashboardTitle}>Dashboard</h2>
                <div style={styles.dashboardSub}>
                  You are logged in and ready to manage bookings.
                </div>
              </div>

              <button onClick={handleLogout} style={styles.secondaryButton}>
                Logout
              </button>
            </div>

            {message && (
              <div
                style={{
                  ...styles.card,
                  padding: 0,
                  background: "transparent",
                  boxShadow: "none",
                  border: "none",
                }}
              >
                <div style={styles.message}>{message}</div>
              </div>
            )}

            <div style={styles.grid}>
              <div style={styles.column}>
                <div style={styles.card}>
                  <h2 style={styles.cardTitle}>Find Available Slots</h2>

                  <div style={styles.row}>
                    <select
                      value={selectedServiceId}
                      onChange={(e) =>
                        setSelectedServiceId(e.target.value ? Number(e.target.value) : "")
                      }
                      style={styles.select}
                    >
                      <option value="">Choose service</option>
                      {services.map((service) => (
                        <option key={service.id} value={service.id}>
                          {service.name} ({service.durationMinutes} min) - {service.price} DKK
                        </option>
                      ))}
                    </select>

                    <input
                      type="date"
                      value={selectedDate}
                      onChange={(e) => setSelectedDate(e.target.value)}
                      style={{ ...styles.input, maxWidth: 220 }}
                    />

                    <button
                      onClick={handleLoadAvailability}
                      disabled={loadingSlots}
                      style={{ ...styles.primaryButton, opacity: loadingSlots ? 0.7 : 1 }}
                    >
                      {loadingSlots ? "Loading..." : "Load availability"}
                    </button>
                  </div>
                </div>

                <div style={styles.card}>
                  <h2 style={styles.cardTitle}>Available Slots</h2>

                  {slots.length === 0 ? (
                    <p style={styles.empty}>No slots loaded yet.</p>
                  ) : (
                    <div style={styles.items}>
                      {slots.map((slot) => {
                        const isAvailable = isAvailableStatus(slot.status);
                        const isPast = isPastDate(slot.startUtc);

                        return (
                          <div key={slot.id} style={styles.itemCard}>
                            <div style={styles.itemTitle}>Slot #{slot.id}</div>

                            <div style={styles.fieldLine}>
                              <strong>Start:</strong> {formatDateTime(slot.startUtc)}
                            </div>
                            <div style={styles.fieldLine}>
                              <strong>End:</strong> {formatDateTime(slot.endUtc)}
                            </div>

                            <span style={{ ...styles.badge, ...getStatusBadgeStyle(slot.status) }}>
                              {isPast && isAvailable ? "Past" : getStatusLabel(slot.status)}
                            </span>

                            {isPast && (
                              <div style={styles.mutedText}>
                                This slot is in the past and cannot be booked.
                              </div>
                            )}

                            <div style={{ marginTop: 14 }}>
                              <button
                                onClick={() => book(slot.id)}
                                disabled={!isAvailable || isPast || bookingSlotId === slot.id}
                                style={{
                                  ...styles.primaryButton,
                                  opacity:
                                    !isAvailable || isPast || bookingSlotId === slot.id ? 0.6 : 1,
                                }}
                              >
                                {bookingSlotId === slot.id
                                  ? "Booking..."
                                  : isPast
                                  ? "Past slot"
                                  : "Book slot"}
                              </button>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  )}
                </div>
              </div>

              <div style={styles.column}>
                <div style={styles.card}>
                  <h2 style={styles.cardTitle}>My Bookings</h2>

                  {loadingBookings ? (
                    <p style={styles.empty}>Loading bookings...</p>
                  ) : sortedBookings.length === 0 ? (
                    <p style={styles.empty}>No bookings yet.</p>
                  ) : (
                    <div style={styles.items}>
                      {sortedBookings.map((booking) => {
                        const service = serviceMap.get(booking.serviceId);
                        const isPast = isPastDate(booking.startUtc);
                        const canCancel = isActiveStatus(booking.status) && !isPast;

                        return (
                          <div key={booking.id} style={styles.itemCard}>
                            <div style={styles.itemTitle}>Booking #{booking.id}</div>

                            <div style={styles.fieldLine}>
                              <strong>Service:</strong>{" "}
                              {service?.name ?? `Service #${booking.serviceId}`}
                            </div>
                            <div style={styles.fieldLine}>
                              <strong>Start:</strong> {formatDateTime(booking.startUtc)}
                            </div>
                            <div style={styles.fieldLine}>
                              <strong>End:</strong> {formatDateTime(booking.endUtc)}
                            </div>

                            <span
                              style={{ ...styles.badge, ...getStatusBadgeStyle(booking.status) }}
                            >
                              {getStatusLabel(booking.status)}
                            </span>

                            {isPast && isActiveStatus(booking.status) && (
                              <div style={styles.mutedText}>
                                Past bookings can no longer be cancelled.
                              </div>
                            )}

                            <div style={{ marginTop: 14 }}>
                              <button
                                onClick={() => cancelBooking(booking.id)}
                                disabled={!canCancel || cancellingBookingId === booking.id}
                                style={{
                                  ...styles.secondaryButton,
                                  opacity:
                                    !canCancel || cancellingBookingId === booking.id ? 0.6 : 1,
                                }}
                              >
                                {cancellingBookingId === booking.id
                                  ? "Cancelling..."
                                  : "Cancel booking"}
                              </button>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  )}
                </div>
              </div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}