using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSMT.Entities {
    public class ContrType : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string OrgCode { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string UserCode { get; set; }
        public virtual string DmlType { get; set; }
    }
}
