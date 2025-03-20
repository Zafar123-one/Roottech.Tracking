using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings
{
    public class CompanyMap : ClassMapping<Company>
    {
        public CompanyMap()
        {
            Table("SMSA_Companies_ST");
            Lazy(false);
            Id(x => x.Id, map => map.Column("Company_Code"));
            Property(x => x.CompanyName);
            Property(x => x.BusinessCat_Code);
            Property(x => x.OrgCode);
        }
    }
}