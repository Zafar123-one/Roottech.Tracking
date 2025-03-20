using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class LastCdr : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string Plateid { get; set; }
        public virtual string UnitId { get; set; }
        public virtual string RTCDTTM { get; set; }
        public virtual string Angle { get; set; }
        public virtual string Latitude { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string Speed { get; set; }
        public virtual string Ignition { get; set; }
        public virtual string DI { get; set; }
        public virtual string DI1 { get; set; }
        public virtual string GSMSignals { get; set; }
        public virtual string Qty { get; set; }
    }
}
