using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings
{
    public class TerritoryDtlMap : ClassMapping<TerritoryDtl> 
    {
        public TerritoryDtlMap() {
			Table("SMSA_TerritoryDTL_ST");
			Schema("dbo");
			Lazy(true);
            ComponentAsId(x => x.Id,
                map =>
                {
                    map.Class<TerritoryDtlIdentifier>();
                    map.Property(x => x.TerritoryCode, m => m.Column("Territory_Code"));
                    map.Property(x => x.TerritorySno, m => m.Column("Territory_SNO"));
                });
            Property(x => x.BlockCode, map => map.Column("Block_Code"));
			Property(x => x.Title);
			Property(x => x.Description);
			ManyToOne(x => x.TerritoryMst, map => { map.Column("Territory_Code"); map.Cascade(Cascade.None); });

			ManyToOne(x => x.AreaDtl, map => 
			{
				map.Column("Block_Code");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}
