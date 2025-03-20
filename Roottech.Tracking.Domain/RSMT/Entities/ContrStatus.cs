using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSMT.Entities {
    
    public class ContrStatus : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual string OrgCode { get; set; }
        public virtual string Title { get; set; }
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
    }
}
