using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class AssetsImage : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string UnitImage { get; set; }
    }
}