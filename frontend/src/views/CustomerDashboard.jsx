export default function CustomerDashboard({
  name,
  setName,
  priority,
  setPriority,
  slotStartUtc,
  setSlotStartUtc,
  createTicket,
  bookAppointment,
}) {
  return (
    <>
      <h2 className="section-heading">Customer dashboard</h2>
      <form className="ticket-form" onSubmit={createTicket}>
        <label>
          Customer name
          <input
            type="text"
            value={name}
            onChange={(event) => setName(event.target.value)}
            placeholder="e.g. Sarah"
            maxLength={80}
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
        <button className="primary" type="submit">
          Take token
        </button>
      </form>

      <form className="ticket-form" onSubmit={bookAppointment}>
        <label>
          Appointment time (UTC)
          <input
            type="datetime-local"
            value={slotStartUtc}
            onChange={(event) => setSlotStartUtc(event.target.value)}
          />
        </label>
        <label>
          Appointment type
          <input value="Standard visit" readOnly />
        </label>
        <button className="primary" type="submit">
          Book appointment
        </button>
      </form>
    </>
  );
}
