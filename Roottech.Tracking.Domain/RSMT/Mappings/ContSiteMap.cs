using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSMT.Entities;

namespace Roottech.Tracking.Domain.RSMT.Mappings {
    
    public class ContSiteMap : ClassMapping<ContSite> {
        
        public ContSiteMap() {
			Table("RSMT_ContSites_TR");
			Schema("dbo");
			Lazy(true);

            ComponentAsId(x => x.Id,
             map =>
             {
                 map.Class<ContSiteIdentifier>();
                 map.Property(x => x.ContractNo, m => m.Column("Contract#"));
                 map.Property(x => x.SiteCode, m => m.Column("SITE_Code"));
             });
			Property(x => x.StartDt);
			Property(x => x.StopDt);
			Property(x => x.StopRemarks);
			Property(x => x.UserCode, map => map.Column("User_Code"));
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
        }
    }
}
