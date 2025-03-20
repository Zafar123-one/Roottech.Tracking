using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings {
    
    public class CityMap : ClassMapping<City> {
        
        public CityMap() {
			Table("SMSA_Cities_ST");
			Schema("dbo");
			Lazy(true);
            ComponentAsId(x => x.Id,
                map =>
                {
                    map.Class<CityIdentifier>();
                    map.Property(x => x.StateId, m => m.Column("StateID"));
                    map.Property(x => x.CountryId, m => m.Column("CountryID"));
                    map.Property(x => x.CityId, m => m.Column("CityID"));
                });
			Property(x => x.CityName);
			Property(x => x.DistrictCode, map => map.Column("District_Code"));
			Property(x => x.DivisionCode, map => map.Column("Division_Code"));
			Property(x => x.CityTitle);
			Property(x => x.OrgCode);
        }
    }
}
