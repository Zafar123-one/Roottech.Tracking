using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities {

    public class EdrLastHi : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual double CdrNo { get; set; }
        public virtual EDRFuel EdrFuel { get; set; }
        public virtual DateTime? Rtcdttm { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string Battery { get; set; }
        public virtual string Ai1 { get; set; }
        public virtual string Ai2 { get; set; }
        public virtual float? Tempr1 { get; set; }
        public virtual double? Devstate { get; set; }
        public virtual double? Ustatus { get; set; }
        public virtual string Msgoord { get; set; }
        public virtual DateTime? Moortcdttm { get; set; }
        public virtual double? NrCount { get; set; }
        public virtual DateTime? NrDate { get; set; }
    }
}
