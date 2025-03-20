using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAA.Entities;

namespace Roottech.Tracking.Domain.SMAA.Mappings {
    
    public class ModuleMap : ClassMapping<Module>
    {    
        public ModuleMap() {
			Table("SMAA_Modules_ST");
			Lazy(false);
			Id(x => x.Id, map => { map.Column("AAMod#"); map.Generator(Generators.Identity); });
			Property(x => x.ModTitleCode, map => map.Column("ModTitle_code"));
			Property(x => x.Description);
        }
    }
}
