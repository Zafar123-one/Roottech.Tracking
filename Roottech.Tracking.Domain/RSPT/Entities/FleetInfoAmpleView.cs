using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class FleetInfoAmpleView : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string Specification { get; set; }
        public virtual string RegDt { get; set; }
        public virtual string TagNo { get; set; }
        public virtual string UnitUsageDesc { get; set; }
        public virtual string UnitTypeDesc { get; set; }
        public virtual string DriverName { get; set; }
        public virtual string DriverCellNo { get; set; }
        public virtual string SiteName { get; set; }
        
    }
}