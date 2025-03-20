using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.MRAT.Entities;

namespace Roottech.Tracking.Domain.MRAT.Mappings {

    public class AssetStatusMap : ClassMapping<AssetStatus>
    {
        public AssetStatusMap() {
			Schema("dbo");
            Table("MRAT_AssetStatus_ST");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("AssetStatus#"); map.Generator(Generators.Identity); });
			Property(x => x.Title);
			Property(x => x.Name);
			Property(x => x.DefaultType);
			Property(x => x.DmlType);
			Property(x => x.DmlDate);
			Property(x => x.UserCode);
			ManyToOne(x => x.Organization, map => 
			{
				map.Column("ORGCODE");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

        }
    }
}
