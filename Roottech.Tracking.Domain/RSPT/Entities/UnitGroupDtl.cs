using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class UnitGroupDtl :IKeyed<int> {
        //public RSPT_UGrpDtl_ST() {
        //    RSPT_UGrpAccessDtl_STs = new List<RSPT_UGrpAccessDtl_ST>();
        //}
        public virtual int Id { get; private set; }
        public virtual int ResourceNo { get; set; }
        public virtual string User_Code { get; set; }
        public virtual int Site_Code { get; set; }
        //public RSPT_UGrpMst_ST RSPT_UGrpMst_ST { get; set; }
        //public RSPT_SITEResource_TR RSPT_SITEResource_TR { get; set; }
        //public SMIM_UserProfiile_ST SMIM_UserProfiile_ST { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
       // public virtual SiteResource SiteResources { get; set; }
        //public virtual IList<UnitGroupMst> UnitGroupMsts { get; set; }
        //public IList<RSPT_UGrpAccessDtl_ST> RSPT_UGrpAccessDtl_STs { get; set; }
        
    }
}

