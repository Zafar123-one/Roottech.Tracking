using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class GFMst : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string Company_Code { get; set; }
        public virtual string GFType { get; set; }
        public virtual string GFTitle { get; set; }
        public virtual string GFName { get; set; }
        public virtual string GFMargin { get; set; }
        public virtual string Comment { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}