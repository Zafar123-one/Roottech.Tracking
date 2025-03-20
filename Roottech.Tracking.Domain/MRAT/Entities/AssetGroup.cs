using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.MRAT.Entities {
    
    public class AssetGroup : IKeyed<double>
    {
        public AssetGroup() { }
        public virtual double Id { get; set; }
        //public virtual AssetType MRAT_AssetTypes_ST { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual string AssetGroupName { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }
}
