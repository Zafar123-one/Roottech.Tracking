using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.OMOP.Entities;

namespace Roottech.Tracking.Domain.OMOP.Mappings {
    
    public class ProjectMstMap : ClassMapping<ProjectMst> {
        
        public ProjectMstMap() {
			Schema("dbo");
            Table("OMOP_ProjectMST_TR");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("Project#"); map.Generator(Generators.Identity); });
			Property(x => x.ContrType);
			Property(x => x.CreateDate);
			Property(x => x.ORGCODE);
			Property(x => x.Company_code);
			Property(x => x.ComdtyCD);
			Property(x => x.CalendarID);
			Property(x => x.Contract, map => map.Column("Contract#"));
			Property(x => x.PRJMGREmp);
			Property(x => x.Title);
			Property(x => x.Description);
			Property(x => x.OPMGREmp, map => map.Column("OPMGREmp#"));
			Property(x => x.FromDate);
			Property(x => x.Todate);
			Property(x => x.User_Code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
        }
    }
}
