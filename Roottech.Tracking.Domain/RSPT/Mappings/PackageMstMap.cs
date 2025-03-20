using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings{
    
    public class PackageMstMap : ClassMapping<PackageMst> {
        
        public PackageMstMap() {
			Table("RSPT_TPCKMst_TR");
			Lazy(false);
            Id(x => x.Id, map => map.Column("PKG_Code"));
            Property(x=> x.Item_code);
            Property(x => x.PKG_DSCR);
			Property(x => x.Consume);
			Property(x => x.UnitID);
			Property(x => x.IMEINo, map => { map.Column("IMEI#"); map.Length(20);});
			Property(x => x.IMSINo, map => {map.Column("IMSI#"); map.Length(20);});
			Property(x => x.CDRBaseType);
			Property(x => x.LLSQty);
			Property(x => x.LLSCDRType);
			Property(x => x.TotalCapacity);
            Property(x => x.imsiNo2, map => { map.Column("imsi#2"); map.Length(20);});
            Property(x => x.FuelTankType);
            Property(x => x.ProductNo, map => { map.Column("Product#"); map.Length(20); });
            Property(x => x.TransNo, map => { map.Column("Trans#"); map.Length(20); });
            Property(x => x.ItemSNo, map => { map.Column("ItemSno#"); map.Length(20); });
            Property(x => x.InsrtId, map => { map.Column("INSRTID"); map.Length(20); });
            Property(x => x.OrgCode);
			Property(x => x.Company_Code);
			Property(x => x.User_Code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);

			/*
            ManyToOne<RSMT_Items_TR>(x => x.RSMT_Items_TR, map => { map.Column("Item_code"); });
			ManyToOne<SMSE_Organization_Mst_ST>(x => x.SMSE_Organization_Mst_ST, map => { map.Column("ORGCODE"); });
			ManyToOne<SRSD_Products_ST>(x => x.SRSD_Products_ST, map => { map.Column("Product#"); });
			ManyToOne<MRSI_ItemDtl_TR>(x => x.MRSI_ItemDtl_TR, map => { map.Column("Trans#"); });
			ManyToOne<SRCC_InstrPool_TR>(x => x.SRCC_InstrPool_TR, map => { map.Column("INSRTID"); });
			ManyToOne<SMSA_Companies_ST>(x => x.SMSA_Companies_ST, map => { map.Column("Company_code"); });
			ManyToOne<SMIM_UserProfiile_ST>(x => x.SMIM_UserProfiile_ST, map => { map.Column("User_Code"); });
			Bag<RSPT_SITEResource_TR>(x => x.RSPT_SITEResource_TRs, colmap =>  { colmap.Key(x => x.Column("PKG_Code"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_SITEResource_TR))); });
			Bag<RSPT_TPCKDtl_TR>(x => x.RSPT_TPCKDtl_TRs, colmap =>  { colmap.Key(x => x.Column("PKG_Code"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_TPCKDtl_TR))); });
			Bag<RSPT_TPCKDtl_TR>(x => x.RSPT_TPCKDtl_TRs, colmap =>  { colmap.Key(x => x.Column("PKG_Code"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_TPCKDtl_TR))); });
             */
        }
    }
}
