using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings {
    
    public class TerritoryMstMap : ClassMapping<TerritoryMst> {
        
        public TerritoryMstMap() {
			Table("SMSA_TerritoryMST_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("Territory_Code"); map.Generator(Generators.Identity); });
			Property(x => x.Title);
			Property(x => x.Description);
			Property(x => x.CityId);
			Property(x => x.StateId);
			Property(x => x.CountryId);
			ManyToOne(x => x.Organization, map => 
			{
				map.Column("ORGCODE");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Company, map => 
			{
				map.Column("Company_code");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});
        }
    }
}
