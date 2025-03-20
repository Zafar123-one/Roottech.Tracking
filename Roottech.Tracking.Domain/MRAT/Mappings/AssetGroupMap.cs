using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.MRAT.Entities;

namespace Roottech.Tracking.Domain.MRAT.Mappings {
    
    
    public class AssetGroupMap : ClassMapping<AssetGroup> {
        
        public AssetGroupMap() {
			Schema("dbo");
            Table("MRAT_AssetGroup_ST");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("AssetGroup#"); map.Generator(Generators.Identity); });
			Property(x => x.AssetGroupName);
			Property(x => x.DmlType);
			Property(x => x.DmlDate);
			/*ManyToOne(x => x.MRAT_AssetTypes_ST, map => 
			{
				map.Column("AssetType#");
				map.PropertyRef("AssetType");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});*/

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
