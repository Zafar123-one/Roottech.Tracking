using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptGridPowerOperation : IKeyed<RptGridPowerOperationIdentifier>
    {
        public virtual RptGridPowerOperationIdentifier Id { get; set; }
        public virtual string RegionName { get; set; }
        public virtual string CityName { get; set; }
        //public virtual string Territory { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string AssetName { get; set; }
        public virtual DateTime OpenDt { get; set; }
        public virtual DateTime CloseDt { get; set; }
        public virtual string TotalDuration { get; set; }
        public virtual int Hour { get; set; }
        public virtual int Minute { get; set; }
        public virtual int Second { get; set; }
    }

    public class RptGridPowerOperationIdentifier
    {
        public virtual Int64 GridBatchNo { get; set; }
        public virtual string TotalDuration { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as RptGridPowerOperationIdentifier;
            if (t == null) return false;
            return GridBatchNo == t.GridBatchNo && TotalDuration == t.TotalDuration;
        }

        public override int GetHashCode()
        {
            return (GridBatchNo + "|" + TotalDuration).GetHashCode();
        }
    }
}