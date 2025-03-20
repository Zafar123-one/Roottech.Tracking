using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.SMIM.Mappings {
    public class RefreshTokenMap : ClassMapping<RefreshToken>
    {
        public RefreshTokenMap() {
			Table("SMIM_RefreshToken_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => map.Generator(Generators.Assigned));
			Property(x => x.UserName);
			Property(x => x.ClientId);
			Property(x => x.IssuedTime);
			Property(x => x.ExpiredTime);
			Property(x => x.ProtectedTicket);
        }
    }
}
