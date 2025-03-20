using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptDayWiseSummary : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string AssetName { get; set; }
        public virtual string PlateID { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Day { get; set; }
        public virtual string StartPoint { get; set; }
        public virtual string EndPoint { get; set; }
        public virtual string MonthlyDuration { get; set; }
        public virtual string MonthlyParking { get; set; }
        public virtual string MonthlyIdle { get; set; }
        public virtual decimal Opening { get; set; }
        public virtual decimal Refuel { get; set; }
        public virtual decimal Consume { get; set; }
        public virtual decimal Theft { get; set; }
        public virtual decimal AvgLtrhr { get; set; }
        public virtual string RunningHour { get; set; }
        public virtual string Idle { get; set; }
        public virtual string Parking { get; set; }
        public virtual decimal ClosingFormula { get; set; }
        public virtual int TotMiles { get; set; }
        public virtual decimal Closing { get; set; }
        public virtual decimal Kms { get; set; }
        public virtual decimal TotMile { get; set; }
    }
}