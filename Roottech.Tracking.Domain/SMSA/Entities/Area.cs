using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {

    public class AreaIdentifier
    {
        public virtual string CityId { get; set; }
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
        public virtual string AreaId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as AreaIdentifier;
            if (t == null) return false;
            return CityId == t.CityId && StateId == t.StateId
                && CountryId == t.CountryId && AreaId == t.AreaId;
        }

        public override int GetHashCode()
        {
            return (CityId + "|" + StateId + "|" + CountryId + "|" + AreaId).GetHashCode();
        }
    }

    public class Area : IKeyed<AreaIdentifier>
    {
        private AreaIdentifier _id = new AreaIdentifier();

        public virtual AreaIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual Organization Organization { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual string AreaName { get; set; }
        public virtual string RegionId { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }
}
