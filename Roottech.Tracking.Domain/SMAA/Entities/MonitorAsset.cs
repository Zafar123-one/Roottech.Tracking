using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAA.Entities
{
    public class MonitorAsset : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual double AssetNo { get; set; }
        public virtual bool Monitor { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}