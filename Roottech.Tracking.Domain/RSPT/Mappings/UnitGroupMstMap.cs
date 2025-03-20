using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {

    public class UnitGroupMstMap : ClassMapping<UnitGroupMst>
    {
        
        public UnitGroupMstMap() {
			Table("RSPT_UGrpMst_ST");
			Lazy(false);
			Id(x => x.Id, map => map.Column("UGrpMst#"));
            Property(x => x.OrgCode);
			Property(x => x.Title);
			Property(x => x.Description);
			Property(x => x.Address);
			Property(x => x.Phone1);
			Property(x => x.Phone2);
			Property(x => x.Manager);
			Property(x => x.User_Code);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
            //ManyToOne(x => x.Organization, map =>
            //                                   {
            //                                       map.Column("OrgCode");
            //                                       map.Lazy(LazyRelation.NoLazy);
            //                                       map.Insert(false);
            //                                       map.Update(false);
            //                                   });

            //Bag(x => x.UnitGroupDtls, map =>
            //{
            //    map.Table("RSPT_UGrpDtl_ST");
            //    map.Cascade(Cascade.None);
            //    map.Key(k => { k.Column("UGrpMst#"); k.NotNullable(true); });
            //    map.Lazy(CollectionLazy.NoLazy);
            //    map.Fetch(CollectionFetchMode.Join);
            //    map.Inverse(true);
            //}, colMap => colMap.OneToMany(otm => otm.Class(typeof(UnitGroupDtl))));
			//ManyToOne<SMSE_Organization_Mst_ST>(x => x.SMSE_Organization_Mst_ST, map => { map.Column("OrgCode"); });
			//ManyToOne<SMIM_UserProfiile_ST>(x => x.SMIM_UserProfiile_ST, map => { map.Column("User_Code"); });
			//Bag<RSPT_UGrpDtl_ST>(x => x.RSPT_UGrpDtl_STs, colmap =>  { colmap.Key(x => x.Column("UnitGroupMst#"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_UGrpDtl_ST))); });
			//Bag<RSPT_UGrpEscDtl_ST>(x => x.RSPT_UGrpEscDtl_STs, colmap =>  { colmap.Key(x => x.Column("UnitGroupMst#"));  }, map => { map.OneToMany(x => x.Class(typeof(RSPT_UGrpEscDtl_ST))); });
        }
    }
}
