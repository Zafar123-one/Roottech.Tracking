using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings
{
    public class GfMstMap : ClassMapping<GFMst>
    {
        public GfMstMap()
        {
            Table("RSGI_GFMst_TR");
            Lazy(false);
            Id(x => x.Id, map => map.Column("GeoFence#"));
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.OrgCode);
            Property(x => x.Company_Code);
            Property(x => x.GFType);
            Property(x => x.GFTitle);
            Property(x => x.GFName);
            Property(x => x.GFMargin);
            Property(x => x.Comment);
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}
