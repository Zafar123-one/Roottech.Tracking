using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptMonthwiseStatistics : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string CityName { get; set; }
        public virtual string RegionName { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string AssetName { get; set; }
        public virtual decimal DgCap { get; set; }
        public virtual string OpenDt { get; set; }
        public virtual decimal FuelOpening { get; set; }
        public virtual decimal Refuel { get; set; }
        public virtual decimal Consume { get; set; }
        public virtual decimal Theft { get; set; }
        public virtual decimal FuelClosing { get; set; }
        public virtual Int64 RunningHour { get; set; }
        public virtual decimal AvgRun { get; set; }
    }
}