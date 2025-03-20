using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings
{
    public class RouteDtlMap : ClassMapping<RouteDtl>
    {
        public RouteDtlMap()
        {
            Table("RSGI_RouteDtl_TR");
            Lazy(false);
            ComponentAsId(x => x.Id,
                          map =>
                              {
                                  map.Class<RouteDtlIdentifier>();
                                  map.Property(x => x.RouteNo, colMap => colMap.Column("Route#"));
                                  map.Property(x => x.PoiNo, colMap => colMap.Column("Poi#"));
                              });
            Property(x => x.PoiSeq);

            //Property(x => x.myPoi, colMap => colMap.Column("Poi#"));
            ManyToOne(x => x.Poi, map =>
            {
                map.Column("Poi#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}