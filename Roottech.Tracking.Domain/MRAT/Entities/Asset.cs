using System;
using System.Collections.Generic;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.MRAT.Entities {
    
    public class Asset : IKeyed<double>
    {
        public virtual double Id { get; private set; }
        public virtual double AssetGroupNo { get; set; }
        public virtual string AssetGroupName { get; set; }
        public virtual double OMMT_LvlNo { get; set; }
        public virtual string AssetName { get; set; }
        public virtual string specifications { get; set; }
        public virtual string inventoryNo { get; set; }
        public virtual string RotatingTag { get; set; }
        public virtual DateTime? PurchaseDate { get; set; }
        public virtual DateTime? LastPMDate { get; set; }
        public virtual DateTime? LastCMDate { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual double NoOfTanks { get; set; }
        public virtual double AssetStatusNo { get; set; }
        public virtual string AssetStatusName { get; set; }
        public virtual string User_Code { get; set; }
        public virtual double ATConditionNo { get; set; }
        public virtual string ATConditionName { get; set; }
        public virtual double UmakeNo { get; set; }
        public virtual double PONo { get; set; }
        public virtual double UModelNo { get; set; }
        public virtual double PUItemMstNo { get; set; }
        public virtual double VendorNo { get; set; }
        public virtual string NONSYSPORefNo { get; set; }
        public virtual DateTime? NONSYSPODate { get; set; }
        public virtual double? PurchasePrice { get; set; }
        public virtual string AssetSNo { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? StopDate { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual string Company_code { get; set; }
        public virtual DateTime? DML_Date { get; set; }
        public virtual string PlateId { get; set; }
        public virtual double? FuelCapacity { get; set; }
        public virtual string EngineUMake { get; set; }
        public virtual double? Dgcap { get; set; }
        public virtual string AlternatorMake { get; set; }
        public virtual DateTime? InstalYear { get; set; }
        public virtual double? ColCapacity { get; set; }
        public virtual int ResourceNo { get; set; }
        public virtual CompleteUnitVw CompleteUnitVw { get; set; }
        public virtual DashboardStationaryV2Vw DashboardStationaryV2 { get; set; }
        public virtual AssetGroup AssetGroup { get; set; }
        public virtual AssetStatus AssetStatus { get; set; }
        public virtual Condition Condition { get; set; }
        public virtual IList<SiteResource> SiteResources { get; set; }
        //public IList<MRAT_AssetInsur_TR> MRAT_AssetInsur_TRs { get; set; }
        //public IList<MRAT_AssetLeased_TR> MRAT_AssetLeased_TRs { get; set; }
        //public IList<MRAT_AssetMeters_TR> MRAT_AssetMeters_TRs { get; set; }
        //public IList<MRAT_AssetMeters_TR> MRAT_AssetMeters_TRs { get; set; }
        //public IList<MRAT_AssetProp_TR> MRAT_AssetProp_TRs { get; set; }
        //public IList<MRAT_AssetProp_TR> MRAT_AssetProp_TRs { get; set; }
        //public IList<MRAT_FilePhoto_TR> MRAT_FilePhoto_TRs { get; set; }
        //public IList<OMFT_FleetUnits_TR> OMFT_FleetUnits_TRs { get; set; }
        //public IList<RSPT_SITEResource_TR> RSPT_SITEResource_TRs { get; set; }
        
    }
}
