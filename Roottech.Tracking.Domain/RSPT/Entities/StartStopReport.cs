using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class StartStopReport : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string InactivityTime { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime StopTime { get; set; }
        public virtual string TravelTime { get; set; }
        public virtual string TravelDistance { get; set; }
    }
}