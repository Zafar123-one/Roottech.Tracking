using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities {

    public class GFDtl : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual GFMst GFMst { get; set; }
        public virtual int GFMstId { get; set; }
        public virtual string User_Code{ get; set; }
        public virtual double? CRadius { get; set; }
        public virtual double? CLongi { get; set; }
        public virtual double? CLati { get; set; }
        public virtual double? PLongi { get; set; }
        public virtual double? PLati { get; set; }
        public virtual double? PSeq { get; set; }
        public virtual double? SLeftTopLongi { get; set; }
        public virtual double? SLeftTopLati { get; set; }
        public virtual double? SRightBotLongi { get; set; }
        public virtual double? SRightBotLati { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}
