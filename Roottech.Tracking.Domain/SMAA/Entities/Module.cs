using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAA.Entities {
    
    public class Module : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual string ModTitleCode { get; set; }
        public virtual string Description { get; set; }
    }
}
