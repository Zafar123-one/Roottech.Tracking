using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAA.Entities {

    public class OrgModuleIdentifier
    {
        public virtual double AaModNo { get; set; }
        public virtual double AaOrgModNo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as OrgModuleIdentifier;
            if (t == null) return false;
            return AaModNo == t.AaModNo && AaOrgModNo == t.AaOrgModNo;
        }

        public override int GetHashCode()
        {
            return (AaModNo + "|" + AaOrgModNo).GetHashCode();
        }
    }

    public class OrgModule : IKeyed<OrgModuleIdentifier>
    {
        private OrgModuleIdentifier _id = new OrgModuleIdentifier();

        public virtual OrgModuleIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public virtual string OrgCode { get; set; }
        public virtual string Allow { get; set; }
        public virtual DateTime? ValidFrom { get; set; }
        public virtual DateTime? ValidTo { get; set; }
        public virtual string PagePath { get; set; }
    }
}
