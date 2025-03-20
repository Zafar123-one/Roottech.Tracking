using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class SensorInfo : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string Description { get; set; }
        public virtual string DiStatus { get; set; }

    }
}