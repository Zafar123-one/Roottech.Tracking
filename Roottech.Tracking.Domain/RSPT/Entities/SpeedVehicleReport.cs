using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class SpeedVehicleReport : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual double Perc { get; set; }
    }
}