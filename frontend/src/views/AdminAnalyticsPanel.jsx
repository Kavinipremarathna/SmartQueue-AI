import AnalyticsPanel from "../components/AnalyticsPanel";
import AppointmentList from "../components/AppointmentList";

export default function AdminAnalyticsPanel({
  staffCount,
  setStaffCount,
  updateStaff,
  analytics,
  appointments,
  loading,
}) {
  return (
    <>
      <h2 className="section-heading">Admin controls</h2>
      <form
        className="ticket-form"
        onSubmit={(event) => event.preventDefault()}
      >
        <label>
          Staff allocation
          <input
            type="number"
            min="1"
            max="20"
            value={staffCount}
            onChange={(event) => setStaffCount(Number(event.target.value))}
          />
        </label>
        <label>
          Effect
          <input value="Lower wait times with higher staff" readOnly />
        </label>
        <button className="primary" type="button" onClick={updateStaff}>
          Update staff allocation
        </button>
      </form>

      <AnalyticsPanel analytics={analytics} />
      <AppointmentList appointments={appointments} loading={loading} />
    </>
  );
}
