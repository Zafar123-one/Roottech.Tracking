using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings {
    public class PoiImageMap : ClassMapping<PoiImage> {
        
        public PoiImageMap() {
			Table("RSGI_POI_Image_TR");
			Schema("dbo");
			Lazy(true);
            Id(x => x.Id, map => { map.Column("POIImage#"); map.Generator(Generators.Identity); });
			Property(x => x.PoiNo, map => map.Column("POI#"));
			Property(x => x.Title);
			Property(x => x.Name);
			Property(x => x.ImagePath);
			Property(x => x.ForAll, map => map.Column("For_All"));
			Property(x => x.AddedByUserCode, map => map.Column("AddedBy_User_Code"));
            ManyToOne(x => x.AddedByUser, map =>
            {
                map.Column("AddedBy_User_Code");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            Property(x => x.UserCode, map => map.Column("User_Code"));
            Property(x => x.DmlType, map => map.Column("DML_Type"));
            Property(x => x.DmlDate, map => map.Column("DML_Date"));
        }
    }
}
