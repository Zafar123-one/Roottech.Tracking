using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings
{
    public class RouteMstMap : ClassMapping<RouteMst>
    {
        public RouteMstMap()
        {
            Table("RSGI_RouteMst_TR");
            Lazy(false);
            Id(x => x.Id, map => map.Column("Route#"));
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.OrgCode);
            Property(x => x.Company_Code);
            Property(x => x.Title);
            Property(x => x.RouteName);
            Property(x => x.Comments);
            Property(x => x.LineColor);
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}