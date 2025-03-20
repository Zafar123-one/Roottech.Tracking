using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.MRAT.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.MRAT.Mappings {
    
    public class AssetsMap : ClassMapping<Asset> {
        
        public AssetsMap() {
			Table("MRAT_Assets_TR");
			Lazy(false);
			Id(x => x.Id, map => map.Column("Asset#"));
			Property(x => x.AssetGroupNo ,map => map.Column("AssetGroup#"));
			Property(x => x.OMMT_LvlNo ,map => map.Column("OMMT_Lvl#"));
			Property(x => x.AssetName);
			Property(x => x.specifications);
			Property(x => x.inventoryNo, map => map.Column("inventory#"));
            Property(x => x.AssetStatusNo, map => map.Column("AssetStatus#"));
            Property(x => x.ATConditionNo, map => map.Column("ATCondition#"));
            Property(x => x.UmakeNo, map => map.Column("Umake#"));
            Property(x => x.UModelNo, map => map.Column("UModel#"));
            Property(x => x.PONo, map => map.Column("PO#"));
            Property(x => x.PUItemMstNo, map => map.Column("PUItemMst#"));
            Property(x => x.VendorNo, map => map.Column("Vendor#"));
            Property(x => x.OrgCode);
            Property(x => x.User_Code);
            Property(x => x.RotatingTag);
			Property(x => x.PurchaseDate);
			Property(x => x.LastPMDate);
			Property(x => x.LastCMDate);
			Property(x => x.NONSYSPORefNo, map => map.Column("NONSYSPORef#"));
			Property(x => x.NONSYSPODate);
			Property(x => x.PurchasePrice);
			Property(x => x.AssetSNo, map => map.Column("AssetS#"));
			Property(x => x.StartDate);
			Property(x => x.StopDate);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
			Property(x => x.Company_code);
			Property(x => x.PlateId);
            Property(x => x.FuelCapacity);
            Property(x => x.EngineUMake);
            Property(x => x.Dgcap);
            Property(x => x.AlternatorMake);
            Property(x => x.InstalYear);

            ManyToOne(x => x.CompleteUnitVw, map =>
            {
                map.Column("Asset#");
                //map.ForeignKey("Asset#");
                map.PropertyRef("AssetNo");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            ManyToOne(x => x.AssetGroup, map =>
            {
                map.Column("AssetGroup#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            ManyToOne(x => x.AssetStatus, map =>
            {
                map.Column("AssetStatus#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            ManyToOne(x => x.Condition, map =>
            {
                map.Column("ATCondition#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            ManyToOne(x => x.Condition, map =>
            {
                map.Column("ATCondition#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });

            ManyToOne(x => x.DashboardStationaryV2, map =>
            {
                map.Column("Asset#");
                map.PropertyRef("AssetNo");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });


            Bag(x => x.SiteResources, map =>
            {
                map.Table("RSPT_SiteResource_TR");
                map.Cascade(Cascade.None);
                map.Key(k => { k.Column("Asset#"); k.NotNullable(true); });
                map.Lazy(CollectionLazy.NoLazy);
                map.Fetch(CollectionFetchMode.Join);
                map.Inverse(true);
            }, colMap => colMap.OneToMany(otm => otm.Class(typeof(SiteResource))));

            //ManyToOne<MRAT_AssetGroup_ST>(x => x.MRAT_AssetGroup_ST, map => { map.Column("AssetGroup#"); });
            //ManyToOne<MRAT_AssetLvl_ST>(x => x.MRAT_AssetLvl_ST, map => { map.Column("OMMT_Lvl#"); });
            //ManyToOne<SMSE_Organization_Mst_ST>(x => x.SMSE_Organization_Mst_ST, map => { map.Column("OrgCode"); });
            //ManyToOne<MRAT_AssetStatus_ST>(x => x.MRAT_AssetStatus_ST, map => { map.Column("AssetStatus#"); });
            //ManyToOne<User_Code_SMIM_UserProfiile_ST>(x => x.User_Code_SMIM_UserProfiile_ST, map => { map.Column("User_Code"); });
            //ManyToOne<User_Code_SMIM_UserProfiile_ST>(x => x.User_Code_SMIM_UserProfiile_ST, map => { map.Column("User_Code"); });
            //ManyToOne<MRAT_Conditions_ST>(x => x.MRAT_Conditions_ST, map => { map.Column("ATCondition#"); });
            //ManyToOne<MRAT_UnitMakes_ST>(x => x.MRAT_UnitMakes_ST, map => { map.Column("Umake#"); });
            //ManyToOne<MRPU_PODtl_TR>(x => x.MRPU_PODtl_TR, map => { map.Column("PO#"); });
            //ManyToOne<MRAT_UnitModels_ST>(x => x.MRAT_UnitModels_ST, map => { map.Column("UModel#"); });
            //ManyToOne<MRPU_Vendors_ST>(x => x.MRPU_Vendors_ST, map => { map.Column("Vendor#"); });
            //ManyToOne<SMSA_Companies_ST>(x => x.SMSA_Companies_ST, map => { map.Column("Company_code"); });
            //Bag<MRAT_AssetInsur_TR>(x => x.MRAT_AssetInsur_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_AssetInsur_TR))); });
            //Bag<MRAT_AssetLeased_TR>(x => x.MRAT_AssetLeased_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_AssetLeased_TR))); });
            //Bag<MRAT_AssetMeters_TR>(x => x.MRAT_AssetMeters_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_AssetMeters_TR))); });
            //Bag<MRAT_AssetMeters_TR>(x => x.MRAT_AssetMeters_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_AssetMeters_TR))); });
            //Bag<MRAT_AssetProp_TR>(x => x.MRAT_AssetProp_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_AssetProp_TR))); });
            //Bag<MRAT_AssetProp_TR>(x => x.MRAT_AssetProp_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_AssetProp_TR))); });
            //Bag<MRAT_FilePhoto_TR>(x => x.MRAT_FilePhoto_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(MRAT_FilePhoto_TR))); });
            //Bag<OMFT_FleetUnits_TR>(x => x.OMFT_FleetUnits_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(OMFT_FleetUnits_TR))); });
            //Bag<RSPT_SITEResource_TR>(x => x.RSPT_SITEResource_TRs, colmap =>  { colmap.Key(x => x.Column("Asset#"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_SITEResource_TR))); });
        }
    }
}
