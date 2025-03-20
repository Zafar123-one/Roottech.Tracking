using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities {
    
    public class UGroupDtlStIdentifier
    {
        public virtual string UgrpCode { get; set; }
        public virtual string UserCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as UGroupDtlStIdentifier;
            if (t == null) return false;
            return UgrpCode == t.UgrpCode && UserCode == t.UserCode;
        }

        public override int GetHashCode()
        {
            return (UgrpCode + "|" + UserCode).GetHashCode();
        }
    }

    public class UGroupDtlSt : IKeyed<UGroupDtlStIdentifier>
    {
        public virtual UGroupDtlStIdentifier Id { get; set; }
        public virtual string UserCode2 { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
    }
}
