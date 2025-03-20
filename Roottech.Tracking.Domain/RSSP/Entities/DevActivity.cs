using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities {
    
    public class DevActivity : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual double? BatchNo { get; set; }
        public virtual double? IoNo { get; set; }
        public virtual DateTime? Frtcdttm { get; set; }
        public virtual DateTime? Trtcdttm { get; set; }
        public virtual string Totduration { get; set; }
        public virtual DateTime? Trdclosedt { get; set; }
        public virtual string DiState { get; set; }
        public virtual EDRFuelRun EdrFuelRun { get; set; }

/*        public virtual string Leveltype { get; set; }
        public virtual double? Resource { get; set; }
        public virtual string Eventttype { get; set; }
        public virtual double? Device { get; set; }
        public virtual int? Sensorsource { get; set; }
        public virtual double? Devsensor { get; set; }
        public virtual double? Cdrcount { get; set; }
        public virtual double? Sensorcode { get; set; }
        public virtual string UsTitle { get; set; }
        public virtual double? Trckactivity { get; set; }
        public virtual double? Durationss { get; set; }
        public virtual double? Durationhh { get; set; }
        public virtual double? Durationmi { get; set; }
        public virtual double? Fmiles { get; set; }
        public virtual double? Tmiles { get; set; }
        public virtual double? Totmiles { get; set; }
        public virtual double? Fkmtrs { get; set; }
        public virtual double? Tkmtrs { get; set; }
        public virtual double? Totkmtrs { get; set; }
        public virtual double? Fmtrs { get; set; }
        public virtual double? Tmtrs { get; set; }
        public virtual double? Totmtrs { get; set; }*/
    }
}
