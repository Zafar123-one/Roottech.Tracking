using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings
{
    public class PoiMap : ClassMapping<Poi>
    {
        public PoiMap()
        {
            Table("RSGI_POI_TR");
            Lazy(false);
            Id(x => x.Id, map =>
                              {
                                  map.Column("POI#");
                                  map.Generator(Generators.Identity);
                              });
            Property(x => x.OrgCode);
            Property(x => x.Company_Code);
            Property(x => x.PoiName);
            Property(x => x.PoiTypeNo, map => map.Column("POIType#"));
            ManyToOne(x => x.PoiType, map =>
            {
                map.Column("POIType#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            Property(x => x.PostalAddr);
            Property(x => x.Block_Code);
            Property(x => x.AreaId);
            Property(x => x.CityId);
            Property(x => x.StateId);
            Property(x => x.CountryId);
            Property(x => x.Lati, map =>
            {
                map.Precision(9);
                map.Scale(6);
            });
            Property(x => x.Longi, map =>
            {
                map.Precision(9);
                map.Scale(6);
            });
            Property(x => x.GF_YN);
            Property(x => x.GF_Val);
            Property(x => x.CustomerRefNo, map => map.Column("CustomerRef#"));
            Property(x => x.ContactPerson);
            Property(x => x.Phone);
            Property(x => x.Cell);
            Property(x => x.Email);
            Property(x => x.User_Code);
        }
    }
}