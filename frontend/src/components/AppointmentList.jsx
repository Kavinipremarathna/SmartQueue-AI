export default function AppointmentList({ appointments, loading }) {
  return (
    <>
      <h2 className="section-heading">Appointments</h2>
      <ul className="queue-list history">
        {appointments.map((appointment) => (
          <li key={appointment.id} className="queue-item compact">
            <div>
              <p className="name">{appointment.customerName}</p>
              <p className="meta">
                {new Date(appointment.slotStartUtc).toLocaleString()}
              </p>
            </div>
            <div className="priority-pill">{appointment.status}</div>
            <div className="position">#{appointment.id}</div>
          </li>
        ))}
        {!loading && appointments.length === 0 && (
          <li className="empty">No appointments.</li>
        )}
      </ul>
    </>
  );
}
