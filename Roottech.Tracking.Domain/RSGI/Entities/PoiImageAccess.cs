using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities {
    
    public class PoiImageAccess : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int PoiImageNo { get; set; }
        public virtual string UserCode { get; set; }
        public virtual UserProfile User { get; set; }
        public virtual string UserCodeName { get; set; }
    }
}
