using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities
{
    public class Company : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual int BusinessCat_Code { get; set; }
        public virtual string OrgCode { get; set; }
    }
}