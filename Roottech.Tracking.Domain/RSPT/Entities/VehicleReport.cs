using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class VehicleReport : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string PlateId { get; set; }
        public virtual double Hours { get; set; }
    }
}
