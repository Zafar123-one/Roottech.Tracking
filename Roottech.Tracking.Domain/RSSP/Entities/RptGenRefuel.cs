using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptGenRefuel : IKeyed<Int32>
    {
        public virtual int Id { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string AssetName { get; set; }
        public virtual string RefuelType { get; set; }
        public virtual DateTime OpenDt { get; set; }
        public virtual DateTime EndDt { get; set; }
        public virtual string TotDuration { get; set; }
        public virtual decimal Increase { get; set; }
    }
}
