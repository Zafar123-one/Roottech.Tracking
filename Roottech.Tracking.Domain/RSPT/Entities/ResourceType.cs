using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class ResourceType: IKeyed<int> {
        //public RSPT_ResourceType_ST() {
        //    RSPT_SITEResource_TRs = new List<RSPT_SITEResource_TR>();
        //}
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string ResourceTyp { get; set; }
        public virtual string Description { get; set; }
        public virtual string Title { get; set; }
        public virtual byte[] ResourceImage1 { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }

        //public virtual Organization Organization { get; set; }
        //public virtual IList<SiteResource> SiteResources { get; set; }
        //public IList<RSPT_SITEResource_TR> RSPT_SITEResource_TRs { get; set; }
        
    }
}
