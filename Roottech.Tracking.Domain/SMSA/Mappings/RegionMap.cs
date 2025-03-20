using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings {
    
    public class RegionMap : ClassMapping<Region> {
        
        public RegionMap() {
			Table("SMSA_Regions_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map =>
			{
                map.Column("Regionid");
			    map.Generator(Generators.Assigned);
			});
			Property(x => x.Regiontype);
			Property(x => x.Regionname);
			Property(x => x.Regiontitle);
			Property(x => x.Orgcode);
        }
    }
}
