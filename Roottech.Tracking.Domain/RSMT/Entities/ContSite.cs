using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSMT.Entities {

    public class ContSite : IKeyed<ContSiteIdentifier>
    {
        private ContSiteIdentifier _id = new ContSiteIdentifier();

        public virtual ContSiteIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public virtual DateTime? StartDt { get; set; }
        public virtual DateTime? StopDt { get; set; }
        public virtual string StopRemarks { get; set; }
        public virtual string UserCode { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }

    public class ContSiteIdentifier
    {
        public virtual double ContractNo { get; set; }
        public virtual double SiteCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as ContSiteIdentifier;
            if (t == null) return false;
            return ContractNo == t.ContractNo && SiteCode == t.SiteCode;
        }

        public override int GetHashCode()
        {
            return (ContractNo + "|" + SiteCode).GetHashCode();
        }
    }
}
