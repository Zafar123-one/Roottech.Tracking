using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAM.Entities;

namespace Roottech.Tracking.Domain.SMAM.Mappings {
    
    public class DashboardMap : ClassMapping<Dashboard> {
        
        public DashboardMap() {
			Table("SMAM_DashBoards_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map =>
			{
                map.Column("DashBoard");
			    map.Generator(Generators.Assigned);
			});
			Property(x => x.Comdtycd);
			Property(x => x.Orgcode);
			Property(x => x.Name);
            Property(x => x.ObjectName);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
        }
    }
}
