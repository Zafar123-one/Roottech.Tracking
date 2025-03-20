using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class RsptUStatus : IKeyed<double> {
        public virtual double Id { get; set; }
        public virtual string Status { get; set; }
        public virtual string StatusDescription { get; set; }
        public virtual string StatusType { get; set; }
    }
}
