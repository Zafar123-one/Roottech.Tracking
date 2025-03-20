using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities
{
    public class CompaniesByUserAndOrg : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string CompanyName { get; set; }
    }
}