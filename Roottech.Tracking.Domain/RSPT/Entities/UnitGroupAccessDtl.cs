using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class UnitGroupAccessDtl : IKeyed<UnitGroupAccessDtlIdentifier>
    {
        private UnitGroupAccessDtlIdentifier _id = new UnitGroupAccessDtlIdentifier();

        public virtual UnitGroupAccessDtlIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    public class UnitGroupAccessDtlIdentifier
    {
        public virtual double Ugrpmst { get; set; }
        public virtual string UgrpCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as UnitGroupAccessDtlIdentifier;
            if (t == null) return false;
            return Ugrpmst == t.Ugrpmst && UgrpCode == t.UgrpCode;
        }

        public override int GetHashCode()
        {
            return (Ugrpmst + "|" + UgrpCode).GetHashCode();
        }
    }
}