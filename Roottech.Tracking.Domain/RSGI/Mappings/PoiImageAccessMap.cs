using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings {
    
    
    public class PoiImageAccessMap : ClassMapping<PoiImageAccess> {
        
        public PoiImageAccessMap() {
			Table("RSGI_POI_Image_Access_TR");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("POIImageAccess#"); map.Generator(Generators.Identity); });
			Property(x => x.PoiImageNo, map => map.Column("POIImage#"));
			Property(x => x.UserCode, map => map.Column("User_Code"));
            ManyToOne(x => x.User, map =>
            {
                map.Column("User_Code");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
        }
    }
}
