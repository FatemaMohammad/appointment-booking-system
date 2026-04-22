import { useEffect, useState } from "react";
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

export default function App() {
  const [email, setEmail] = useState("hello111@gmail.com");
  const [password, setPassword] = useState("hello111");
  const [message, setMessage] = useState("");
  const [token, setToken] = useState(localStorage.getItem("token") || "");

  const [services, setServices] = useState<Service[]>([]);
  const [selectedServiceId, setSelectedServiceId] = useState<number | "">("");
  const [selectedDate, setSelectedDate] = useState("");
  const [slots, setSlots] = useState<TimeSlot[]>([]);

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setMessage("");

    try {
      const data = await apiFetch<{ token: string }>(
        `/api/auth/login?email=${encodeURIComponent(email)}&password=${encodeURIComponent(password)}`,
        {
          method: "POST",
        }
      );

      localStorage.setItem("token", data.token);
      setToken(data.token);
      setMessage("Login successful");
    } catch (error) {
      setMessage(error instanceof Error ? error.message : "Login failed");
    }
  }

  function handleLogout() {
    localStorage.removeItem("token");
    setToken("");
    setServices([]);
    setSlots([]);
    setSelectedServiceId("");
    setSelectedDate("");
    setMessage("Logged out");
  }

  useEffect(() => {
    async function loadServices() {
      try {
        const data = await apiFetch<Service[]>("/api/services");
        setServices(data);
      } catch (error) {
        setMessage(error instanceof Error ? error.message : "Failed to load services");
      }
    }

    if (token) {
      loadServices();
    }
  }, [token]);

  async function handleLoadAvailability() {
    if (!selectedServiceId || !selectedDate) {
      setMessage("Choose a service and a date first.");
      return;
    }

    try {
      setMessage("");
      const data = await apiFetch<TimeSlot[]>(
        `/api/availability?serviceId=${selectedServiceId}&date=${selectedDate}`
      );
      setSlots(data);
    } catch (error) {
      setMessage(error instanceof Error ? error.message : "Failed to load availability");
    }
  }

  return (
    <div style={{ maxWidth: 800, margin: "40px auto", fontFamily: "Arial" }}>
      <h1>Appointment Booking System</h1>

      {!token ? (
        <form onSubmit={handleLogin}>
          <div style={{ marginBottom: 12 }}>
            <label>Email</label>
            <br />
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              style={{ width: "100%", padding: 8 }}
            />
          </div>

          <div style={{ marginBottom: 12 }}>
            <label>Password</label>
            <br />
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              style={{ width: "100%", padding: 8 }}
            />
          </div>

          <button type="submit">Login</button>
        </form>
      ) : (
        <div>
          <p>You are logged in.</p>
          <button onClick={handleLogout}>Logout</button>

          <h2 style={{ marginTop: 30 }}>Services</h2>

          {services.length === 0 ? (
            <p>No services found.</p>
          ) : (
            <div>
              <select
                value={selectedServiceId}
                onChange={(e) => setSelectedServiceId(Number(e.target.value))}
                style={{ padding: 8, marginRight: 12 }}
              >
                <option value="">Choose service</option>
                {services.map((service) => (
                  <option key={service.id} value={service.id}>
                    {service.name} ({service.durationMinutes} min)
                  </option>
                ))}
              </select>

              <input
                type="date"
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                style={{ padding: 8, marginRight: 12 }}
              />

              <button onClick={handleLoadAvailability}>Load availability</button>
            </div>
          )}

          <h2 style={{ marginTop: 30 }}>Available Slots</h2>

          {slots.length === 0 ? (
            <p>No slots loaded yet.</p>
          ) : (
            <ul>
              {slots.map((slot) => (
                <li key={slot.id} style={{ marginBottom: 10 }}>
                  <strong>Slot #{slot.id}</strong> - {slot.startUtc} → {slot.endUtc} - {slot.status}
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      {message && <p style={{ marginTop: 20 }}>{message}</p>}
    </div>
  );
}