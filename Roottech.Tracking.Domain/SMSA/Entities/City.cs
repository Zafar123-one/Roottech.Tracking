using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities {
    
    public class CityIdentifier
    {
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
        public virtual string CityId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as CityIdentifier;
            if (t == null) return false;
            return StateId == t.StateId && CountryId == t.CountryId && CityId == t.CityId;
        }

        public override int GetHashCode()
        {
            return (StateId + "|" + CountryId + "|" + CityId).GetHashCode();
        }
    }
    public class City : IKeyed<CityIdentifier>
    {
        public virtual CityIdentifier Id { get; set; }
        public virtual string CityName { get; set; }
        public virtual string DistrictCode { get; set; }
        public virtual string DivisionCode { get; set; }
        public virtual string CityTitle { get; set; }
        public virtual string OrgCode { get; set; }
    }
}
