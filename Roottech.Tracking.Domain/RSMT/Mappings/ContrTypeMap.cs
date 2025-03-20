using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSMT.Entities;

namespace Roottech.Tracking.Domain.RSMT.Mappings {
    public class ContrTypeMap : ClassMapping<ContrType> {
        public ContrTypeMap() {
            Table("RSMT_ContrTypes_ST");
			Schema("dbo");
			Lazy(true);
            Id(x => x.Id, map => { map.Column("ContrType"); map.Generator(Generators.Assigned); });
            Property(x => x.Title, map => map.Length(15));
			Property(x => x.OrgCode, map => map.Precision(18));
			Property(x => x.Name, map => map.Length(40));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
			Property(x => x.UserCode, map => { map.Column("User_Code"); map.Length(10); });
			Property(x => x.DmlType, map => { map.Column("DML_Type"); map.Length(1); });
        }
    }
}
