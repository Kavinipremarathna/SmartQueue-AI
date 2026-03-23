export default function AnalyticsPanel({ analytics }) {
  return (
    <>
      <h2 className="section-heading">Admin analytics panel</h2>
      <ul className="queue-list history">
        <li className="queue-item compact">
          <div>
            <p className="name">Average wait</p>
            <p className="meta">{analytics?.averageWaitMinutes ?? 0} min</p>
          </div>
          <div className="priority-pill">
            Peak hour: {analytics?.peakHour ?? 0}
          </div>
          <div className="position">
            Eff: {analytics?.serviceEfficiencyPercent ?? 0}%
          </div>
        </li>
      </ul>
    </>
  );
}
