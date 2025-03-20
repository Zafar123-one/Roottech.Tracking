using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSMT.Entities;

namespace Roottech.Tracking.Domain.RSMT.Mappings {
    public class ContrStatusMap : ClassMapping<ContrStatus> {
        public ContrStatusMap() {
			Table("RSMT_ContrStatus_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("ContrStatus#"); map.Generator(Generators.Identity); });
			Property(x => x.OrgCode, map => map.Precision(18));
			Property(x => x.Title, map => map.Length(15));
			Property(x => x.Name, map => map.Length(40));
			Property(x => x.Type, map => map.Length(1));
        }
    }
}
