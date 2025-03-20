using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptGridPowerAssetOperation : IKeyed<RptGridPowerAssetOperationIdentifier>
    {
        public virtual RptGridPowerAssetOperationIdentifier Id { get; set; }
        public virtual string RegionName { get; set; }
        public virtual string CityName { get; set; }
        //public virtual string Territory { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string AssetName { get; set; }
        public virtual DateTime AssetOpenDt { get; set; }
        public virtual DateTime AssetCloseDt { get; set; }
        public virtual string AssetTotalDuration { get; set; }
        public virtual DateTime GridOpenDt { get; set; }
        public virtual DateTime GridCloseDt { get; set; }
        public virtual int GridResourceNo { get; set; }
        public virtual int AssetHour { get; set; }
        public virtual int AssetMinute { get; set; }
        public virtual int AssetSecond { get; set; }
        public virtual int GridHour { get; set; }
        public virtual int GridMinute { get; set; }
        public virtual int GridSecond { get; set; }
    }
    public class RptGridPowerAssetOperationIdentifier
    {
        public virtual Int64 AssetBatchNo { get; set; }
        public virtual Int64 GridBatchNo { get; set; }
        public virtual string TotalDuration { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as RptGridPowerAssetOperationIdentifier;
            if (t == null) return false;
            return AssetBatchNo == t.AssetBatchNo && GridBatchNo == t.GridBatchNo && TotalDuration == t.TotalDuration;
        }

        public override int GetHashCode()
        {
            return (AssetBatchNo + "|" + GridBatchNo + "|" + TotalDuration).GetHashCode();
        }
    }
}