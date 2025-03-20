using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class TrckUnitMap : ClassMapping<TrckUnit> {

        public TrckUnitMap()
        {
			Table("RSPT_TrckUnits_TR");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map =>
			{
                map.Column("Unitid");
			    map.Generator(Generators.Assigned);
			});
			Property(x => x.Contract, map => map.Column("Contract#"));
			Property(x => x.SiteCode, map => { map.Column("SITE_Code"); map.NotNullable(true); });
			Property(x => x.ItemCode, map => map.Column("Item_code"));
			Property(x => x.Resource, map => map.Column("Resource#"));
			Property(x => x.TotalUnit);
			Property(x => x.MinQty);
			Property(x => x.TotalCapacity);
			Property(x => x.MaxQty);
			Property(x => x.ReorderQty);
			Property(x => x.OpenBal);
			Property(x => x.OpenDate);
			Property(x => x.BalanceQty);
			Property(x => x.RefuelDate);
			Property(x => x.RefuelQty);
			Property(x => x.ConsumeDate);
			Property(x => x.ConsumeQty);
			Property(x => x.RunFrom);
			Property(x => x.RunTo);
			Property(x => x.RunHrs);
			Property(x => x.RunConsume);
			Property(x => x.RunRefuel);
			Property(x => x.Lls, map => map.Column("LLS#"));
			Property(x => x.Orgcode);
			Property(x => x.Ustatus, map => map.Column("UStatus#"));
			Property(x => x.Reason, map => map.Column("Reason#"));
			Property(x => x.Mileage);
			Property(x => x.Lls1qty);
			Property(x => x.Lls2qty);
			Property(x => x.OldUnitId);
			ManyToOne(x => x.Company, map => 
			{
				map.Column("Company_code");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});
        }
    }
}
