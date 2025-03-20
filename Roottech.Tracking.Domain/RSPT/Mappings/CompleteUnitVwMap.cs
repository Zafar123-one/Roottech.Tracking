using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class CompleteUnitVwMap : ClassMapping<CompleteUnitVw> {
        
        public CompleteUnitVwMap() {
			Table("RSPT_CompleteUnit_Vw");
			Lazy(false);
            Id(x => x.Id , map => map.Column("UnitID"));
			Property(x => x.ORGCODE, map => map.Column("ORGCODE"));
			Property(x => x.FleetUnitNo, map => map.Column("FleetUnit#"));
			Property(x => x.PlateID, map => map.Column("PlateID"));
			Property(x => x.SITE_Code, map => map.Column("SITE_Code"));
			Property(x => x.site_name, map => map.Column("site_name"));
			Property(x => x.ResourceNo, map => map.Column("Resource#"));
			Property(x => x.AssetNo, map => map.Column("Asset#"));
			Property(x => x.AssetName, map => map.Column("AssetName"));
			Property(x => x.specifications, map => map.Column("specifications"));
			Property(x => x.PKG_Code, map => map.Column("PKG_Code"));
			Property(x => x.MinQty, map => map.Column("MinQty"));
			Property(x => x.MaxQty, map => map.Column("MaxQty"));
			Property(x => x.ReorderQty, map => map.Column("ReorderQty"));
			Property(x => x.llscapacity, map => map.Column("llscapacity"));
			Property(x => x.ResourceType, map => map.Column("ResourceType"));
			Property(x => x.NoofChambers, map => map.Column("#ofChambers"));
			Property(x => x.di1, map => map.Column("di1"));
			Property(x => x.di22, map => map.Column("di22"));
			Property(x => x.di3, map => map.Column("di3"));
			Property(x => x.di4, map => map.Column("di4"));
			Property(x => x.di5, map => map.Column("di5"));
			Property(x => x.di6, map => map.Column("di6"));
        }
    }
}
