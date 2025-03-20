using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSMT.Entities;

namespace Roottech.Tracking.Domain.RSMT.Mappings {
    
    public class ContractMstMap : ClassMapping<ContractMst> {
        
        public ContractMstMap() {
			Table("RSMT_ContractMst_TR");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("Contract#"); map.Generator(Generators.Identity); });
			Property(x => x.ContractDate);
			Property(x => x.Title);
			Property(x => x.ContrStatusNo, map => map.Column("ContrStatus#"));
			Property(x => x.MasterContractNo, map => map.Column("MasterContract#"));
			Property(x => x.ContDscr, map => map.Column("Cont_DSCR"));
			Property(x => x.DateFrom);
			Property(x => x.OrgCode);
			Property(x => x.DateTo);
			Property(x => x.Comdtycd);
			Property(x => x.VendorCode);
			Property(x => x.ContrType);
			Property(x => x.ToVendorCode);
			Property(x => x.CompanyCode, map => map.Column("company_Code"));
			Property(x => x.UserCode, map => map.Column("User_Code"));
			Property(x => x.ToLeranceType);
			Property(x => x.ToLeranceQty);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
			Property(x => x.RWithFuel);
			Property(x => x.FuelRateType);
			Property(x => x.FixedRate);
			Property(x => x.ServChargeType);
			Property(x => x.ServChargeRate);
			Property(x => x.EmerChargeType);
			Property(x => x.EmerChargeRate);
			Property(x => x.ExpiryAlertB4);
			Property(x => x.ExpiryAlertDt);

            Bag(x => x.ContSites, colmap =>
            {
                colmap.Key(x => x.Column("Contract#"));
                colmap.Inverse(true);
            }, map => { map.OneToMany(); });

            Bag(x => x.ProjectMsts, colmap =>
            {
                colmap.Key(x => x.Column("Contract#"));
                colmap.Inverse(true);
            }, map => { map.OneToMany(); });
            
            ManyToOne(x => x.Vendor, map =>
            {
                map.Column("VendorCode");
                map.Cascade(Cascade.None);
                map.Insert(false);
                map.Update(false);
            });
        }
    }
}
