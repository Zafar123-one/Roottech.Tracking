using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class UnitGroupMst : IKeyed<int>
    {
        //public RSPT_UGrpMst_ST() {
        //    RSPT_UGrpDtl_STs = new List<RSPT_UGrpDtl_ST>();
        //    RSPT_UGrpEscDtl_STs = new List<RSPT_UGrpEscDtl_ST>();
        //}
        public virtual int Id { get; private set; }//UGrpMstNo
        public virtual int OrgCode { get; set; }
        public virtual string User_Code { get; set; }
        //public SMSE_Organization_Mst_ST SMSE_Organization_Mst_ST { get; set; }
        //public SMIM_UserProfiile_ST SMIM_UserProfiile_ST { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Address { get; set; }
        public virtual string Phone1 { get; set; }
        public virtual string Phone2 { get; set; }
        public virtual string Manager { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
        //public virtual Organization Organization { get; set; }

        //public virtual IList<UnitGroupDtl> UnitGroupDtls { get; set; }
        //public IList<RSPT_UGrpDtl_ST> RSPT_UGrpDtl_STs { get; set; }
        //public IList<RSPT_UGrpEscDtl_ST> RSPT_UGrpEscDtl_STs { get; set; }
        
    }
}
