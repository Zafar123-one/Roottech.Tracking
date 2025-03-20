using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings
{
    public class PoiTypeMap : ClassMapping<PoiType>
    {
        public PoiTypeMap()
        {
            Table("RSGI_POITypes_ST");
            Lazy(false);
            Id(x => x.Id, map => map.Column("POItype#"));
            Property(x => x.OrgCode);
            Property(x => x.Company_Code);
            Property(x => x.Title);
            Property(x => x.TypeName);
            Property(x => x.GImageNo, map => map.Column("GImage#"));
            ManyToOne(x => x.GImage, map =>
            {
                map.Column("GImage#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            Property(x => x.User_Code);
        }
    }
}