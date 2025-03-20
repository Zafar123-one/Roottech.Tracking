using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    
    public class RsptUStatusMap : ClassMapping<RsptUStatus> {
        
        public RsptUStatusMap() {
			Table("RSPT_UStatus_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("UStatus#"); map.Generator(Generators.Identity); });
            Property(x => x.Status, map => map.Column("UStatus"));
            Property(x => x.StatusDescription);
            Property(x => x.StatusType, map => map.Column("UStatusType"));
        }
    }
}
