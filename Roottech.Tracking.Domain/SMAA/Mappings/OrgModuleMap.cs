using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAA.Entities;

namespace Roottech.Tracking.Domain.SMAA.Mappings {
    
    public class OrgModuleMap : ClassMapping<OrgModule> {
        
        public OrgModuleMap()
        {
			Table("SMAA_OrgModules_ST");
			Lazy(false);
            ComponentAsId(x => x.Id,
                map =>
                {
                    map.Class<OrgModuleIdentifier>();
                    map.Property(x => x.AaModNo, colMap => colMap.Column("AAMod#"));
                    map.Property(x => x.AaOrgModNo, colMap => colMap.Column("AAORgMod#"));
                });
            Property(x => x.OrgCode);
            Property(x => x.Allow);
			Property(x => x.ValidFrom);
			Property(x => x.ValidTo);
			Property(x => x.PagePath, map => map.Column("Page_Path"));
        }
    }
}
