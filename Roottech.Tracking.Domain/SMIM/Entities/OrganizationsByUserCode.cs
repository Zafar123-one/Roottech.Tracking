using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities
{
    public class OrganizationsByUserCode : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string Description { get; set; }
    }
}