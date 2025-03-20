using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAA.Entities
{
    public class ModMst : IKeyed<int>
    {
        public virtual int Id { get; private set; }
        public virtual int AaModNo { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual int ParentMomNo { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}
