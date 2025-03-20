using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {
    
    public class Country  : IKeyed<string> {

        public virtual string Id { get; set; }
        public virtual Region Region { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual string CountryName { get; set; }
        public virtual string CountryTitle { get; set; }
        public virtual string CountryDef { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }
}
