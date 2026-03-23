import { useEffect, useMemo, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { NavLink, Navigate, Route, Routes } from "react-router-dom";
import { API_BASE, authHeaders, fetchJson } from "./api/client";
import IntroScreen from "./components/IntroScreen";
import LoginView from "./components/LoginView";
import CustomerDashboard from "./views/CustomerDashboard";
import StaffLiveQueue from "./views/StaffLiveQueue";
import AdminAnalyticsPanel from "./views/AdminAnalyticsPanel";

function App() {
  const [showIntro, setShowIntro] = useState(true);
  const [username, setUsername] = useState("admin");
  const [password, setPassword] = useState("admin123");
  const [token, setToken] = useState("");
  const [role, setRole] = useState("");
  const [name, setName] = useState("");
  const [priority, setPriority] = useState(1);
  const [staffCount, setStaffCount] = useState(2);
  const [slotStartUtc, setSlotStartUtc] = useState("");
  const [tickets, setTickets] = useState([]);
  const [queueState, setQueueState] = useState(null);
  const [summary, setSummary] = useState(null);
  const [analytics, setAnalytics] = useState(null);
  const [appointments, setAppointments] = useState([]);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const waitingTickets = useMemo(
    () => tickets.filter((ticket) => ticket.status === "Waiting"),
    [tickets],
  );

  const canManageQueue = role === "Admin" || role === "Staff";
  const isAdmin = role === "Admin";

  async function login(event) {
    event.preventDefault();
    setError("");

    try {
      const response = await fetchJson(`${API_BASE}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      setToken(response.token);
      setRole(response.role);
      setName("");
    } catch (loginError) {
      setError(loginError.message);
    }
  }

  async function loadDashboard() {
    if (!token) {
      return;
    }

    setLoading(true);
    setError("");
    try {
      const queue = await fetchJson(`${API_BASE}/api/queue/current`, {
        headers: authHeaders(token),
      });
      setQueueState(queue);

      const requests = [
        fetchJson(`${API_BASE}/api/tickets`, {
          headers: authHeaders(token),
        }),
      ];

      if (canManageQueue) {
        requests.push(
          fetchJson(`${API_BASE}/api/queue/summary`, {
            headers: authHeaders(token),
          }),
          fetchJson(`${API_BASE}/api/analytics`, {
            headers: authHeaders(token),
          }),
          fetchJson(`${API_BASE}/api/appointments`, {
            headers: authHeaders(token),
          }),
        );
      }

      const results = await Promise.all(requests);
      setTickets(results[0] ?? []);
      setSummary(results[1] ?? null);
      setAnalytics(results[2] ?? null);
      setAppointments(results[3] ?? []);
    } catch (fetchError) {
      setError(fetchError.message);
    } finally {
      setLoading(false);
    }
  }

  async function createTicket(event) {
    event.preventDefault();
    setError("");

    try {
      await fetchJson(`${API_BASE}/api/tickets`, {
        method: "POST",
        headers: authHeaders(token),
        body: JSON.stringify({ customerName: name, priority }),
      });
      setName("");
      setPriority(1);
      await loadDashboard();
    } catch (createError) {
      setError(createError.message);
    }
  }

  async function serveNext() {
    setError("");
    try {
      await fetchJson(`${API_BASE}/api/queue/serve-next`, {
        method: "POST",
        headers: authHeaders(token),
      });
      await loadDashboard();
    } catch (serveError) {
      setError(serveError.message);
    }
  }

  async function updateStatus(id, status) {
    setError("");
    try {
      await fetchJson(`${API_BASE}/api/tickets/${id}/status`, {
        method: "PATCH",
        headers: authHeaders(token),
        body: JSON.stringify({ status }),
      });
      await loadDashboard();
    } catch (statusError) {
      setError(statusError.message);
    }
  }

  async function bookAppointment(event) {
    event.preventDefault();
    setError("");

    try {
      const start = slotStartUtc
        ? new Date(slotStartUtc).toISOString()
        : new Date().toISOString();
      await fetchJson(`${API_BASE}/api/appointments`, {
        method: "POST",
        headers: authHeaders(token),
        body: JSON.stringify({
          customerName: name || username,
          slotStartUtc: start,
          durationMinutes: 20,
        }),
      });
      await loadDashboard();
    } catch (appointmentError) {
      setError(appointmentError.message);
    }
  }

  async function updateStaff() {
    setError("");
    try {
      await fetchJson(`${API_BASE}/api/admin/staff-allocation`, {
        method: "POST",
        headers: authHeaders(token),
        body: JSON.stringify({ staffCount }),
      });
      await loadDashboard();
    } catch (adminError) {
      setError(adminError.message);
    }
  }

  function logout() {
    setToken("");
    setRole("");
    setError("");
    setShowIntro(true);
  }

  useEffect(() => {
    loadDashboard();
  }, [token]);

  useEffect(() => {
    if (!token) {
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE}/hubs/queue`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    connection.on("queue-updated", () => {
      loadDashboard();
    });

    connection.start().catch(() => {
      setError("Live updates disconnected. Manual refresh still works.");
    });

    return () => {
      connection.stop();
    };
  }, [token]);

  if (showIntro) {
    return <IntroScreen onReady={() => setShowIntro(false)} />;
  }

  if (!token) {
    return (
      <LoginView
        username={username}
        password={password}
        setUsername={setUsername}
        setPassword={setPassword}
        login={login}
        error={error}
      />
    );
  }

  const defaultPath =
    role === "Admin" ? "/admin" : role === "Staff" ? "/staff" : "/customer";

  return (
    <main className="page">
      <div className="orb orb-a" aria-hidden="true" />
      <div className="orb orb-b" aria-hidden="true" />

      <section className="panel">
        <header className="panel-header">
          <p className="eyebrow">Smart Queue {role}</p>
          <h1>Queue Operations Center</h1>
          <p className="subtitle">
            Predicted wait: {queueState?.predictedWaitMinutes ?? 0} min | Staff:{" "}
            {queueState?.staffCount ?? 0}
          </p>
        </header>

        <nav className="role-nav" aria-label="Role views">
          <NavLink
            to="/customer"
            className={({ isActive }) =>
              `role-link${isActive ? " active" : ""}`
            }
          >
            Customer
          </NavLink>
          {canManageQueue && (
            <NavLink
              to="/staff"
              className={({ isActive }) =>
                `role-link${isActive ? " active" : ""}`
              }
            >
              Staff
            </NavLink>
          )}
          {isAdmin && (
            <NavLink
              to="/admin"
              className={({ isActive }) =>
                `role-link${isActive ? " active" : ""}`
              }
            >
              Admin
            </NavLink>
          )}
          <button className="ghost" onClick={loadDashboard}>
            {loading ? "Refreshing..." : "Refresh"}
          </button>
          <button className="ghost" type="button" onClick={logout}>
            Logout
          </button>
        </nav>

        <div className="stats" role="status" aria-live="polite">
          <div>
            <span>Total waiting</span>
            <strong>{queueState?.totalWaiting ?? 0}</strong>
          </div>
          <div>
            <span>Avg service (min)</span>
            <strong>{queueState?.averageServiceMinutes ?? 0}</strong>
          </div>
          <div>
            <span>Predicted wait</span>
            <strong>{queueState?.predictedWaitMinutes ?? 0}</strong>
          </div>
          {canManageQueue && (
            <>
              <div>
                <span>Served</span>
                <strong>{summary?.served ?? 0}</strong>
              </div>
              <div>
                <span>Cancelled</span>
                <strong>{summary?.cancelled ?? 0}</strong>
              </div>
            </>
          )}
        </div>

        {error && <p className="error">{error}</p>}

        <Routes>
          <Route
            path="/customer"
            element={
              <CustomerDashboard
                name={name}
                setName={setName}
                priority={priority}
                setPriority={setPriority}
                slotStartUtc={slotStartUtc}
                setSlotStartUtc={setSlotStartUtc}
                createTicket={createTicket}
                bookAppointment={bookAppointment}
              />
            }
          />
          {canManageQueue && (
            <Route
              path="/staff"
              element={
                <StaffLiveQueue
                  waitingTickets={waitingTickets}
                  updateStatus={updateStatus}
                  serveNext={serveNext}
                  loading={loading}
                />
              }
            />
          )}
          {isAdmin && (
            <Route
              path="/admin"
              element={
                <AdminAnalyticsPanel
                  staffCount={staffCount}
                  setStaffCount={setStaffCount}
                  updateStaff={updateStaff}
                  analytics={analytics}
                  appointments={appointments}
                  loading={loading}
                />
              }
            />
          )}
          <Route path="*" element={<Navigate to={defaultPath} replace />} />
        </Routes>
      </section>
    </main>
  );
}

export default App;
