using System;
using System.Collections.Generic;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class EDRFuelRun : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual string UnitID { get; set; }
        public virtual char EventType { get; set; }
        public virtual string EventTypeName { get; set; }
        public virtual DateTime? OpenDt { get; set; }
        public virtual DateTime? FRTCDTTM { get; set; }
        public virtual int DurationHh { get; set; }
        public virtual int DurationMi { get; set; }
        public virtual int DurationSs { get; set; }
        public virtual string TotDuration { get; set; }
        public virtual double BalanceQty { get; set; }
        public virtual double NQty { get; set; }
        public virtual int PoiNo { get; set; }
        public virtual double CdrNo { get; set; }
        public virtual DateTime? CloseDt { get; set; }
        public virtual string LevelType { get; set; }
        public virtual EDRFuel EdrFuel { get; set; }

        public virtual string AI1 { get; set; }
        public virtual string AI2 { get; set; }
        public virtual string DI { get; set; }
        public virtual IList<DevActivity> DevActivities { get; set; }
    }
}