using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities {
    
    public class PoiImage : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int PoiNo { get; set; }
        public virtual string Title { get; set; }
        public virtual string Name { get; set; }
        public virtual string ImagePath { get; set; }
        public virtual bool ForAll { get; set; }
        public virtual string AddedByUserCode { get; set; }
        public virtual UserProfile AddedByUser { get; set; }
        public virtual string UserCode { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string CompanyCode { get; set; }
    }
}
