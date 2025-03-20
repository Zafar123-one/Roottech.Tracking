using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class EdrGeo : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual double CdrNo { get; set; }
        public virtual DateTime? RTCDTTM { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string Latitude { get; set; }
        public virtual double? GeoFenceNo { get; set; }
        public virtual char? IoType { get; set; }
        public virtual DateTime? PullDt { get; set; }
        public virtual DateTime? UpdateDt { get; set; }
    }
}