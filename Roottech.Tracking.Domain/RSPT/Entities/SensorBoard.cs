using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class SensorBoard : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string InputValues { get; set; }
        public virtual string ActualValue { get; set; }
    }
}