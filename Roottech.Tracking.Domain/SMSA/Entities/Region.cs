using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {
    
    public class Region : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string Regiontype { get; set; }
        public virtual string Regionname { get; set; }
        public virtual string Regiontitle { get; set; }
        public virtual double? Orgcode { get; set; }
    }
}
