using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class UnitGroupDtlMap : ClassMapping<UnitGroupDtl> {
        
        public UnitGroupDtlMap() {
			Table("RSPT_UGrpDtl_ST");
			Lazy(false);
            Id(x => x.Id, map => map.Column("UGrpMst#"));
            Property(x => x.ResourceNo, map => map.Column("Resource#"));// map.Insert(false); map.Update(false);});
            Property(x => x.Site_Code);
            Property(x => x.User_Code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
            //Bag(x => x.SiteResources, map =>
            //{
            //    map.Table("RSPT_SiteResource_TR");
            //    map.Cascade(Cascade.None);
            //    map.Key(k => { k.Column("Site_Code"); k.NotNullable(true); });
            //    map.Lazy(CollectionLazy.NoLazy);
            //    map.Fetch(CollectionFetchMode.Join);
            //    map.Inverse(true);
            //}, colMap => colMap.OneToMany(otm => otm.Class(typeof(SiteResource))));

            //Bag(x => x.UnitGroupMsts, map =>
            //{
            //    map.Table("RSPT_UGrpMst_ST");
            //    map.Cascade(Cascade.None);
            //    map.Key(k => { k.Column("UGrpMst#"); k.NotNullable(true); });
            //    map.Lazy(CollectionLazy.NoLazy);
            //    map.Fetch(CollectionFetchMode.Join);
            //    map.Inverse(true);
            //}, colMap => colMap.OneToMany(otm => otm.Class(typeof(UnitGroupMst))));
            //ManyToOne<RSPT_UGrpMst_ST>(x => x.RSPT_UGrpMst_ST, map => { map.Column("UnitGroupMst#"); });
            //ManyToOne<RSPT_SITEResource_TR>(x => x.RSPT_SITEResource_TR, map => { map.Column("Resource#"); });
            //ManyToOne<SMIM_UserProfiile_ST>(x => x.SMIM_UserProfiile_ST, map => { map.Column("User_Code"); });
            //Bag<RSPT_UGrpAccessDtl_ST>(x => x.RSPT_UGrpAccessDtl_STs, colmap =>  { colmap.Key(x => x.Column("Resource#"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_UGrpAccessDtl_ST))); });
        }
    }
}
