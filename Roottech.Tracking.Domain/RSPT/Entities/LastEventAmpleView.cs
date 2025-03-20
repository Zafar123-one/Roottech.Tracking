using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class LastEventAmpleView : IKeyed<long>
    {
        public virtual long Id { get; set; }
        public virtual string Activity { get; set; }
        public virtual DateTime Initiate { get; set; }
        public virtual DateTime Stopdt { get; set; }
        public virtual string totDuration { get; set; }
        public virtual string IDLHr { get; set; }
        public virtual double? TotalMileage { get; set; }
        public virtual double? AtcComKM { get; set; }
        public virtual double? NetQty { get; set; }
    }
}