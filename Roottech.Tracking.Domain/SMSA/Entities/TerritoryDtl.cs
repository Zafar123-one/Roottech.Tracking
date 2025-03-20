using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {

    public class TerritoryDtlIdentifier
    {
        public virtual double TerritoryCode { get; set; }
        public virtual double TerritorySno { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as TerritoryDtlIdentifier;
            if (t == null) return false;
            return TerritoryCode == t.TerritoryCode && TerritorySno == t.TerritorySno;
        }

        public override int GetHashCode()
        {
            return (TerritoryCode + "|" + TerritorySno).GetHashCode();
        }
    }

    public class TerritoryDtl : IKeyed<TerritoryDtlIdentifier>
    {
        private TerritoryDtlIdentifier _id = new TerritoryDtlIdentifier();

        public virtual TerritoryDtlIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public virtual TerritoryMst TerritoryMst { get; set; }
        public virtual AreaDtl AreaDtl { get; set; }
        public virtual string BlockCode { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
    }
}
