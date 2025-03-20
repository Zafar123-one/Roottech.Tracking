using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAA.Entities;

namespace Roottech.Tracking.Domain.SMAA.Mappings
{
    public class ModMstMap : ClassMapping<ModMst>
    {
        public ModMstMap()
        {
            Table("SMAA_ModObjMst_ST");
            Lazy(false);
            Id(x => x.Id, map =>
            {
                map.Column("Mom#");
                map.Generator(Generators.Identity);
            });
            Property(x => x.AaModNo, map => map.Column("AAMod#"));
            Property(x => x.Title);
            Property(x => x.Description);
            Property(x => x.ParentMomNo, map => map.Column("ParentMom#"));
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}