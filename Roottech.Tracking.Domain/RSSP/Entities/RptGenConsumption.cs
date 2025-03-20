using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptGenConsumption : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string SiteCode { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string AssetName { get; set; }
        public virtual DateTime OpenDtForGroup { get; set; }
        public virtual DateTime OpenDt { get; set; }
        public virtual DateTime CloseDt { get; set; }
        public virtual string Duration { get; set; }
        public virtual decimal? ConsumeQty { get; set; }
        public virtual int Hour { get; set; }
        public virtual int Minute { get; set; }
        public virtual int Second { get; set; }
        public virtual decimal? Average { get; set; }
    }
}