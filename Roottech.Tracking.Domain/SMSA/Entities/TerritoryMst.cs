using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {
    
    public class TerritoryMst : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Company Company { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string CityId { get; set; }
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
    }
}
