using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {

    public class StateIdentifier
    {
        public virtual string CountryId { get; set; }
        public virtual string StateId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as StateIdentifier;
            if (t == null) return false;
            return CountryId == t.CountryId && StateId == t.StateId;
        }

        public override int GetHashCode()
        {
            return (CountryId + "|" + StateId).GetHashCode();
        }
    }

    public class State : IKeyed<StateIdentifier>
    {
        private StateIdentifier _id = new StateIdentifier();

        public virtual StateIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public virtual Country Country { get; set; }
        public virtual Region Region { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual string StateName { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }
}
