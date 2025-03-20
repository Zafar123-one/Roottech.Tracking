using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class BusLocWisePlateIdLov : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string AssetName { get; set; }
        public virtual string ResourceNo { get; set; }
    }
}