using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings {
    
    public class StateMap : ClassMapping<State> {
        
        public StateMap() {
			Table("SMSA_States_ST");
			Schema("dbo");
			Lazy(true);
            ComponentAsId(x => x.Id,
             map =>
             {
                 map.Class<StateIdentifier>(); 
                 map.Property(x => x.CountryId, m => m.Column("CountryID"));
                 map.Property(x => x.StateId, m => m.Column("StateID"));
             });
			Property(x => x.StateName);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
			ManyToOne(x => x.Country, map => 
			{
				map.Column("CountryID");
				map.Cascade(Cascade.None);
			});

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
