using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptVehicleRefuelDetail : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string LevelType { get; set; }
        public virtual string EventType { get; set; }
        public virtual string Location { get; set; }
        public virtual DateTime OpenDate { get; set; }
        public virtual DateTime CloseDate { get; set; }
        public virtual int DurationHH { get; set; }
        public virtual int DurationMI { get; set; }
        public virtual int DurationSS { get; set; }
        public virtual string TotDuration { get; set; }
        public virtual string TotIdleTime { get; set; }
        public virtual string FromMileage { get; set; }
        public virtual string ToMileage { get; set; }
        //public virtual int AverageSpeed { get; set; }
        //public virtual string MaxSpeed { get; set; }
        public virtual decimal StartQty { get; set; }
        public virtual decimal EndQty { get; set; }
        public virtual decimal Increase { get; set; }
        public virtual decimal Decrease { get; set; }
        public virtual string Description { get; set; }
    }
}