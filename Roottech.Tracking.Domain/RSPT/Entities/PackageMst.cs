using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class PackageMst : IKeyed<int>
    {
        /*public PackageMst() {
			RSPT_SITEResource_TRs = new List<RSPT_SITEResource_TR>();
			RSPT_TPCKDtl_TRs = new List<RSPT_TPCKDtl_TR>();
			RSPT_TPCKDtl_TRs = new List<RSPT_TPCKDtl_TR>();
        }*/

        public int Id { get; private set; }
        public int Item_code { get; set; }
        public string PKG_DSCR { get; set; }
        public string Consume { get; set; }
        public string UnitID { get; set; }
        public string IMEINo { get; set; }
        public string IMSINo { get; set; }
        public string CDRBaseType { get; set; }
        public double? LLSQty { get; set; }
        public string LLSCDRType { get; set; }
        public double? TotalCapacity { get; set; }
        public string imsiNo2 { get; set; }
        public string FuelTankType { get; set; }
        public int OrgCode { get; set; }
        public int ProductNo { get; set; }
        public int TransNo { get; set; }
        public int InsrtId { get; set; }
        public int ItemSNo { get; set; }
        public string Company_Code { get; set; }
        public string User_Code { get; set; }
        public string DML_Type { get; set; }
        public DateTime? DML_Date { get; set; }

        /*public IList<RSPT_SITEResource_TR> RSPT_SITEResource_TRs { get; set; }
        public IList<RSPT_TPCKDtl_TR> RSPT_TPCKDtl_TRs { get; set; }
        public IList<RSPT_TPCKDtl_TR> RSPT_TPCKDtl_TRs { get; set; }*/
        
    }
}
