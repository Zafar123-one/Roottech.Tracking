using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities
{
    public class CompleteLocationByOrgCode : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
    }
}