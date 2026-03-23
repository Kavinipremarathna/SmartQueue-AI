export default function QueueList({
  waitingTickets,
  canManageQueue,
  updateStatus,
  loading,
}) {
  return (
    <>
      <h2 className="section-heading">Live queue screen</h2>
      <ul className="queue-list">
        {waitingTickets.map((ticket, index) => (
          <li key={ticket.id} className="queue-item">
            <div>
              <p className="name">{ticket.customerName}</p>
              <p className="meta">
                Est wait: {ticket.estimatedWaitMinutes} min
              </p>
            </div>
            <div className="priority-pill">P{ticket.priority}</div>
            <div className="position">#{index + 1}</div>
            {canManageQueue && (
              <div className="actions">
                <button
                  className="mini"
                  type="button"
                  onClick={() => updateStatus(ticket.id, "Served")}
                >
                  Serve
                </button>
                <button
                  className="mini danger"
                  type="button"
                  onClick={() => updateStatus(ticket.id, "Cancelled")}
                >
                  Cancel
                </button>
              </div>
            )}
          </li>
        ))}
        {!loading && waitingTickets.length === 0 && (
          <li className="empty">No waiting tickets.</li>
        )}
      </ul>
    </>
  );
}
