using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.SMIM.Mappings {
    public class ClientMasterMap : ClassMapping<ClientMaster> {
        
        public ClientMasterMap() {
			Table("SMIM_ClientMaster_ST");
			Schema("dbo");
			Lazy(true);
            Id(x => x.Id, map =>
            {
                map.Column("ClientKeyId");
                map.Generator(Generators.Identity);
            });
			Property(x => x.ClientId, map => map.NotNullable(true));
			Property(x => x.ClientSecret, map => map.NotNullable(true));
			Property(x => x.ClientName, map => map.NotNullable(true));
			Property(x => x.Active, map => map.NotNullable(true));
			Property(x => x.RefreshTokenLifeTime, map => map.NotNullable(true));
			Property(x => x.AllowedOrigin, map => map.NotNullable(true));
        }
    }
}
