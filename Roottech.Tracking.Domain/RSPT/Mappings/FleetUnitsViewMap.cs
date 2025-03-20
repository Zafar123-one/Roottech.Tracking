using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings
{
    public class FleetUnitsViewMap : ClassMapping<FleetUnitsView> {
        
        public FleetUnitsViewMap() {
			Table("rspt_FleetUnits_vw");
			Schema("dbo");
			Lazy(true);
			Property(x => x.OrgCode);
            Id(x => x.Id, map => { map.Column("Unitid"); });
			Property(x => x.Fleetunit, map => map.Column("FleetUnit#"));
			Property(x => x.Plateid);
			Property(x => x.SiteCode, map => { map.Column("SITE_Code"); map.NotNullable(true); });
			Property(x => x.Dgcap);
			Property(x => x.SiteName, map => map.Column("site_name"));
			Property(x => x.Title);
			Property(x => x.Resource, map => map.Column("Resource#"));
			Property(x => x.Asset, map => map.Column("Asset#"));
			Property(x => x.Assetname);
			Property(x => x.Specifications);
			Property(x => x.PkgCode, map => map.Column("PKG_Code"));
			Property(x => x.Minqty);
			Property(x => x.Maxqty);
			Property(x => x.Reorderqty);
			Property(x => x.Llscapacity);
			Property(x => x.Resourcetype);
			Property(x => x.Ofchambers, map => map.Column("#ofChambers"));
			Property(x => x.Di1);
			Property(x => x.Di22);
			Property(x => x.Di3);
			Property(x => x.Di4);
			Property(x => x.Di5);
			Property(x => x.Di6);
			Property(x => x.Engineumake);
			Property(x => x.Ustatus, map => map.Column("UStatus#"));
			Property(x => x.Activationdt);
        }
    }
}
