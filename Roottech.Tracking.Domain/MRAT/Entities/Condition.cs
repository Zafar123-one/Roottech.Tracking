using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.MRAT.Entities {
    
    public class Condition : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual string Title { get; set; }
        public virtual string Name { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string UserCode { get; set; }
    }
}
