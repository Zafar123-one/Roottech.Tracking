using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings {
    
    public class CountryMap : ClassMapping<Country> {
        
        public CountryMap() {
			Table("SMSA_Country_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map =>
			{
                map.Column("CountryId");
			    map.Generator(Generators.Assigned);
			});
			Property(x => x.CountryName);
			Property(x => x.CountryTitle);
			Property(x => x.CountryDef);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
			ManyToOne(x => x.Region, map => 
			{
				map.Column("RegionID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Organization, map => 
			{
				map.Column("ORGCODE");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.UserProfile, map => 
			{
				map.Column("User_Code");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});
        }
    }
}
