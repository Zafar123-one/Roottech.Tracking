using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings {
    
    public class AreaMap : ClassMapping<Area> {
        
        public AreaMap() {
			Table("SMSA_Areas_ST");
			Schema("dbo");
			Lazy(true);

            ComponentAsId(x => x.Id,
                map =>
                {
                    map.Class<AreaIdentifier>();
                    map.Property(x => x.CityId, m => m.Column("CityID"));
                    map.Property(x => x.StateId, m => m.Column("StateID"));
                    map.Property(x => x.CountryId, m => m.Column("CountryID"));
                    map.Property(x => x.AreaId, m => m.Column("AreaID"));
                });

			Property(x => x.AreaName);
			Property(x => x.RegionId);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_Date"));
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
