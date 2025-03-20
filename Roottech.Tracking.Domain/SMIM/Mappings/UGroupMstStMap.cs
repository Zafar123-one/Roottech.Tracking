using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.SMIM.Mappings {
    
    public class UGroupMstStMap : ClassMapping<UGroupMstSt> {
        
        public UGroupMstStMap() {
			Table("SMIM_UGroupMST_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("UGRP_Code"); map.Generator(Generators.Identity); });
			Property(x => x.UgrpName, map => map.Column("UGRP_Name"));
			Property(x => x.UgrpStatus, map => map.Column("UGRP_Status"));
			Property(x => x.Superadmin);
			Property(x => x.Admin);
			Property(x => x.Orgcode);
			Property(x => x.Regionwise);
            Property(x => x.RegionId, map => map.Column("RegionID"));
            Property(x => x.CompanyCode, map => map.Column("Company_Code"));
            Property(x => x.UserCode, map => map.Column("User_Code"));
			Property(x => x.Clientwise);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
			/*ManyToOne(x => x.SmsaCompaniesSt, map => 
			{
				map.Column("Company_code");
				map.PropertyRef("CompanyCode");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.SmsaRegionsSt, map => 
			{
				map.Column("RegionID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.SmimUserprofiileSt, map => 
			{
				map.Column("User_Code");
				map.PropertyRef("UserCode");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			Bag(x => x.SmamAlertescTr, colmap =>  { colmap.Key(x => x.Column("UGRP_Code")); colmap.Inverse(true); }, map => { map.OneToMany(); });*/
        }
    }
}
