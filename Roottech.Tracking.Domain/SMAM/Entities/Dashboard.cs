using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAM.Entities {
    
    public class Dashboard : IKeyed<string> {
        public virtual string Id { get; set; }
        public virtual string Comdtycd { get; set; }
        public virtual double? Orgcode { get; set; }
        public virtual string Name { get; set; }
        public virtual string ObjectName { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }
}
