using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAA.Entities
{
    public class GFAsset : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual int AssetNo { get; set; }
        public virtual int GeoFenceNo { get; set; }
        public virtual bool GeoFence { get; set; }
        public virtual AreaToGF AreaToGF { get; set; }
        public virtual int GFMargin { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }

    public enum AreaToGF { I, O, B }; //Inside, Outside, Both
}