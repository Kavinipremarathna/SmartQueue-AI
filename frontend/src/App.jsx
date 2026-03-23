import { useEffect, useMemo, useState } from "react";

const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5055";

function App() {
  const [name, setName] = useState("");
  const [priority, setPriority] = useState(1);
  const [tickets, setTickets] = useState([]);
  const [summary, setSummary] = useState(null);
  const [search, setSearch] = useState("");
  const [autoRefresh, setAutoRefresh] = useState(true);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [servingNext, setServingNext] = useState(false);
  const [error, setError] = useState("");

  const waitingTickets = useMemo(() => {
    return tickets.filter((ticket) => ticket.status === "Waiting");
  }, [tickets]);

  const historyTickets = useMemo(() => {
    return tickets.filter((ticket) => ticket.status !== "Waiting");
  }, [tickets]);

  const filteredWaiting = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) {
      return waitingTickets;
    }

    return waitingTickets.filter((ticket) =>
      ticket.customerName.toLowerCase().includes(term),
    );
  }, [waitingTickets, search]);

  async function fetchJson(url, options) {
    const response = await fetch(url, options);
    if (!response.ok) {
      throw new Error(`Request failed (${response.status})`);
    }

    if (response.status === 204) {
      return null;
    }

    return await response.json();
  }

  async function loadDashboard() {
    setLoading(true);
    setError("");
    try {
      const [allTickets, queueSummary] = await Promise.all([
        fetchJson(`${API_BASE}/api/queue/all`),
        fetchJson(`${API_BASE}/api/queue/summary`),
      ]);
      setTickets(Array.isArray(allTickets) ? allTickets : []);
      setSummary(queueSummary);
    } catch (fetchError) {
      setError(fetchError.message);
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");

    if (!name.trim()) {
      setError("Customer name is required.");
      return;
    }

    setSubmitting(true);
    try {
      const params = new URLSearchParams({
        name: name.trim(),
        priority: String(priority),
      });

      const response = await fetch(`${API_BASE}/api/queue/add?${params}`, {
        method: "POST",
      });

      if (!response.ok) {
        throw new Error(`Add ticket failed (${response.status})`);
      }

      setName("");
      setPriority(1);
      await loadDashboard();
    } catch (submitError) {
      setError(submitError.message);
    } finally {
      setSubmitting(false);
    }
  }

  async function handleServeNext() {
    setServingNext(true);
    setError("");
    try {
      await fetchJson(`${API_BASE}/api/queue/serve-next`, { method: "POST" });
      await loadDashboard();
    } catch (serveError) {
      setError(serveError.message);
    } finally {
      setServingNext(false);
    }
  }

  async function handleStatusChange(id, status) {
    setError("");
    try {
      const params = new URLSearchParams({ status });
      await fetchJson(`${API_BASE}/api/queue/${id}/status?${params}`, {
        method: "PATCH",
      });
      await loadDashboard();
    } catch (statusError) {
      setError(statusError.message);
    }
  }

  async function handleDelete(id) {
    setError("");
    try {
      await fetchJson(`${API_BASE}/api/queue/${id}`, { method: "DELETE" });
      await loadDashboard();
    } catch (deleteError) {
      setError(deleteError.message);
    }
  }

  useEffect(() => {
    loadDashboard();
  }, []);

  useEffect(() => {
    if (!autoRefresh) {
      return;
    }

    const timer = setInterval(() => {
      loadDashboard();
    }, 8000);

    return () => clearInterval(timer);
  }, [autoRefresh]);

  return (
    <main className="page">
      <div className="orb orb-a" aria-hidden="true" />
      <div className="orb orb-b" aria-hidden="true" />

      <section className="panel">
        <header className="panel-header">
          <p className="eyebrow">SmartQueue Control</p>
          <h1>Live Queue Desk</h1>
          <p className="subtitle">
            Advanced operations for real desk usage: serve next, cancel, monitor
            load, and search live queue.
          </p>
        </header>

        <div className="stats" role="status" aria-live="polite">
          <div>
            <span>Total tickets</span>
            <strong>{summary?.total ?? 0}</strong>
          </div>
          <div>
            <span>Waiting now</span>
            <strong>{summary?.waiting ?? 0}</strong>
          </div>
          <div>
            <span>Served</span>
            <strong>{summary?.served ?? 0}</strong>
          </div>
          <div>
            <span>Cancelled</span>
            <strong>{summary?.cancelled ?? 0}</strong>
          </div>
          <div>
            <span>Oldest wait (min)</span>
            <strong>{summary?.oldestWaitingMinutes ?? 0}</strong>
          </div>
          <button
            className="ghost"
            onClick={loadDashboard}
            disabled={loading || submitting}
          >
            {loading ? "Refreshing..." : "Refresh"}
          </button>
        </div>

        <div className="toolbar">
          <button
            className="primary"
            type="button"
            onClick={handleServeNext}
            disabled={servingNext || loading || waitingTickets.length === 0}
          >
            {servingNext ? "Serving..." : "Serve Next"}
          </button>
          <label className="toggle">
            <input
              type="checkbox"
              checked={autoRefresh}
              onChange={(event) => setAutoRefresh(event.target.checked)}
            />
            Auto refresh
          </label>
          <input
            className="search"
            type="text"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search waiting customer"
          />
        </div>

        <form className="ticket-form" onSubmit={handleSubmit}>
          <label>
            Customer name
            <input
              type="text"
              value={name}
              onChange={(event) => setName(event.target.value)}
              placeholder="e.g. Sarah"
              maxLength={60}
            />
          </label>

          <label>
            Priority
            <input
              type="number"
              min="0"
              max="10"
              value={priority}
              onChange={(event) => setPriority(Number(event.target.value))}
            />
          </label>

          <button className="primary" type="submit" disabled={submitting}>
            {submitting ? "Adding..." : "Add Ticket"}
          </button>
        </form>

        {error && <p className="error">{error}</p>}

        <h2 className="section-heading">Waiting Queue</h2>
        <ul className="queue-list">
          {filteredWaiting.map((ticket, index) => (
            <li key={ticket.id} className="queue-item">
              <div>
                <p className="name">{ticket.customerName}</p>
                <p className="meta">Status: {ticket.status}</p>
              </div>
              <div className="priority-pill">P{ticket.priority}</div>
              <div className="position">#{index + 1}</div>
              <div className="actions">
                <button
                  className="mini"
                  type="button"
                  onClick={() => handleStatusChange(ticket.id, "Served")}
                >
                  Serve
                </button>
                <button
                  className="mini danger"
                  type="button"
                  onClick={() => handleStatusChange(ticket.id, "Cancelled")}
                >
                  Cancel
                </button>
              </div>
            </li>
          ))}

          {!loading && filteredWaiting.length === 0 && (
            <li className="empty">
              No matching waiting tickets. Add one or clear search.
            </li>
          )}
        </ul>

        <h2 className="section-heading">History</h2>
        <ul className="queue-list history">
          {historyTickets.map((ticket) => (
            <li key={ticket.id} className="queue-item compact">
              <div>
                <p className="name">{ticket.customerName}</p>
                <p className="meta">Status: {ticket.status}</p>
              </div>
              <div className="priority-pill">P{ticket.priority}</div>
              <button
                className="mini ghost-danger"
                type="button"
                onClick={() => handleDelete(ticket.id)}
              >
                Remove
              </button>
            </li>
          ))}
          {!loading && historyTickets.length === 0 && (
            <li className="empty">No served or cancelled tickets yet.</li>
          )}
        </ul>
      </section>
    </main>
  );
}

export default App;
