using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class GeoFence : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int GeofenceNo { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string Company_code { get; set; }
        public virtual string GFType { get; set; }
        public virtual string GFTitle { get; set; }
        public virtual string GFName { get; set; }
        public virtual string GFMargin { get; set; }
        public virtual string Comment { get; set; }
        public virtual double? CRadius { get; set; }
        public virtual double? CLongi { get; set; }
        public virtual double? CLati { get; set; }
        public virtual double? PLongi { get; set; }
        public virtual double? PLati { get; set; }
        public virtual double? PSeq { get; set; }
        public virtual double? SLeftTopLongi { get; set; }
        public virtual double? SLeftTopLati { get; set; }
        public virtual double? SRightBotLongi { get; set; }
        public virtual double? SRightBotLati { get; set; }
    }
}
