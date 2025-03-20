using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.SMSE.Mappings
{
    public class OrganizationMap : ClassMapping<Organization>
    {
        public OrganizationMap()
        {
            Table("SMSE_Organization_MST_ST");
            Lazy(false);
            Id(x => x.Id, map => map.Column("ORGCODE"));
            Property(x => x.DFLT);
            Property(x => x.DSCR);
            Property(x => x.CONTACT);
            Property(x => x.CalendarID);
            Property(x => x.ParentOrg);
            Property(x => x.Title);
        }
    }
}