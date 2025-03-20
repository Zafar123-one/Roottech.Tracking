using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    
    public partial class FuelPriceMap : ClassMapping<FuelPrice> {
        
        public FuelPriceMap() {
			Schema("dbo");
            Table("RSPT_FuelPrice_TR");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("PriceList#"); map.Generator(Generators.Identity); });
			Property(x => x.FuelType);
			Property(x => x.PriceDate);
			Property(x => x.FuelRate);
			Property(x => x.UnitID);
			Property(x => x.User_code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
        }
    }
}
