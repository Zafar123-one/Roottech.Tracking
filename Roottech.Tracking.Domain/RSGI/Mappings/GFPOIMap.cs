using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings {
    
    public class GFPOIMap : ClassMapping<GFPOI> {

        public GFPOIMap()
        {
			Table("RSGI_GFPOI_TR");
			Lazy(false);
            ComponentAsId(x => x.Id,
                         map =>
                         {
                             map.Class<GFPOIIdentifier>();
                             map.Property(x => x.GeoFenceNo, colMap => colMap.Column("GeoFence#"));
                             map.Property(x => x.PoiNo, colMap => colMap.Column("Poi#"));
                         });
			Property(x => x.ORGCODE);
            Property(x => x.Company_code);
            Property(x => x.User_Code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
			/*ManyToOne<RSGI_POI_TR>(x => x.RSGI_POI_TR, map => { map.Column("POI#"); });
			ManyToOne<RSGI_GFMst_TR>(x => x.RSGI_GFMst_TR, map => { map.Column("GeoFence#"); });
			ManyToOne<SMSE_Organization_Mst_ST>(x => x.SMSE_Organization_Mst_ST, map => { map.Column("ORGCODE"); });
			ManyToOne<SMSA_Companies_ST>(x => x.SMSA_Companies_ST, map => { map.Column("Company_code"); });
			ManyToOne<SMIM_UserProfiile_ST>(x => x.SMIM_UserProfiile_ST, map => { map.Column("User_Code"); });*/
        }
    }
}