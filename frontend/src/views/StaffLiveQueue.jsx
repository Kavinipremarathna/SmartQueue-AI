import QueueList from "../components/QueueList";

export default function StaffLiveQueue({
  waitingTickets,
  updateStatus,
  serveNext,
  loading,
}) {
  return (
    <>
      <h2 className="section-heading">Staff live queue</h2>
      <div className="toolbar">
        <button
          className="primary"
          type="button"
          onClick={serveNext}
          disabled={waitingTickets.length === 0}
        >
          Serve next
        </button>
      </div>

      <QueueList
        waitingTickets={waitingTickets}
        canManageQueue
        updateStatus={updateStatus}
        loading={loading}
      />
    </>
  );
}
