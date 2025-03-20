using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class SiteResourceMap : ClassMapping<SiteResource> {
        
        public SiteResourceMap() {
			Table("RSPT_SITEResource_TR");
			Lazy(false);
            ComponentAsId(x => x.Id,
                          map =>
                              {
                                  map.Class<SiteResourceIdentifier>();
                                  map.Property(x => x.Site_Code);
                                  map.Property(x => x.ResourceNo, colMap => colMap.Column("Resource#"));
                              });
            /*
            ManyToOne(x => x.ContraBatch, map =>
            {
                map.Column("ID_NL_Contra_Batches");
                map.Lazy(LazyRelation.NoLazy);
                map.Fetch(FetchKind.Join);
                map.Insert(false);
                map.Update(false);
            });*/
            //Id(x => x.Id, map => { map.Column("Site_Code"); map. });
            Property(x => x.Item_Code, map => map.Column("Item_Code"));
            //Property(x => x.ResourceNo, map => map.Column("Resource#"));
			Property(x => x.MinQty);
			Property(x => x.MaxQty);
			Property(x => x.ReorderQty);
			Property(x => x.PriorityCode);
			Property(x => x.PKG_Code);
			Property(x => x.llscapacity);
			Property(x => x.OrgCode);
			Property(x => x.ResourceType);
            Property(x => x.NoofChambers, map => map.Column("#ofChambers"));
			Property(x => x.LeadTimetype);
			Property(x => x.LeadTime);
			Property(x => x.SensorCodeD1, map => map.Column("SensorCodeD1"));
			Property(x => x.SensorCodeD2);
			Property(x => x.SensorCodeD3);
			Property(x => x.SensorCodeD4);
			Property(x => x.SensorCodeD5);
			Property(x => x.SensorCodeD6);
			Property(x => x.ResourcePosition);
			Property(x => x.AssetNo, map => map.Column("Asset#"));
			Property(x => x.ResourceTypeNo,map => map.Column("ResourceType#") );
            Property(x => x.ActivationDt);
            /*ManyToOne(x => x.ResourceTypeObject, map =>
            {
                map.Column("ResourceType#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            
            Bag(x => x.UnitGroupDtls, map =>
            {
                map.Table("RSPT_UGrpDtl_ST");
                map.Cascade(Cascade.None);
                map.Key(k => { k.Column("Site_Code"); k.NotNullable(true); });
                map.Lazy(CollectionLazy.NoLazy);
                map.Fetch(CollectionFetchMode.Join);
                map.Inverse(true);
            }, colMap => colMap.OneToMany(otm => otm.Class(typeof(UnitGroupDtl))));
            
            ManyToOne(x => x.Asset, map =>
            {
                map.Column("Asset#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            */
        }
    }
}
