using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAA.Entities;

namespace Roottech.Tracking.Domain.SMAA.Mappings
{
    public class ModDtlMap : ClassMapping<ModDtl>
    {
        public ModDtlMap()
        {
            Table("SMAA_ModObjDtl_ST");
            Lazy(false);
            Id(x => x.Id, map =>
            {
                map.Column("MoD#");
                map.Generator(Generators.Identity);
            });
            Property(x => x.MomNo, map => map.Column("Mom#"));
            ManyToOne(x => x.ModMst, map =>
            {
                map.Column("Mom#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            Property(x => x.ObjCode);
            Property(x => x.Title);
            Property(x => x.Description);
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}