using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class ResourceTypeMap : ClassMapping<ResourceType> {
        
        public ResourceTypeMap() {
			Table("RSPT_ResourceType_ST");
			Lazy(false);
			Id(x => x.Id, map => map.Column("ResourceType#"));
			Property(x => x.OrgCode);
			Property(x => x.ResourceTyp, map => map.Column("ResourceType"));
			Property(x => x.Description);
			Property(x => x.Title);
			Property(x => x.ResourceImage1);
			Property(x => x.User_Code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
           /* ManyToOne(x => x.Organization, map =>
            {
                map.Column("OrgCode");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
*/
            //Bag(x => x.SiteResources, map =>
            //{
            //    map.Table("RSPT_SiteResource_TR");
            //    map.Cascade(Cascade.None);
            //    map.Key(k => { k.Column("ResourceType#"); k.NotNullable(true); });
            //    map.Lazy(CollectionLazy.NoLazy);
            //    map.Fetch(CollectionFetchMode.Join);
            //    map.Inverse(true);
            //}, colMap => colMap.OneToMany(otm => otm.Class(typeof(SiteResource))));

			//Bag<RSPT_SITEResource_TR>(x => x.RSPT_SITEResource_TRs, colmap =>  { colmap.Key(x => x.Column("ResourceType#"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_SITEResource_TR))); });
        }
    }
}
