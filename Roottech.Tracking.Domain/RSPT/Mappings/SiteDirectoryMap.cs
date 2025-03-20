using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings
{
    public class SiteDirectoryMap : ClassMapping<SiteDirectory>
    {
        public SiteDirectoryMap()
        {
            Table("RSPT_SiteDirectory_ST");
            Schema("dbo");
            Lazy(true);
            Id(x => x.Id, map => { map.Column("SiteCont#"); map.Generator(Generators.Identity); });
            Property(x => x.SiteCode, map => { map.Column("SITE_Code"); map.Precision(18); });
            Property(x => x.Contname, map => map.Length(40));
            Property(x => x.Desig, map => map.Length(20));
            Property(x => x.Cell, map => { map.Column("Cell#"); map.Length(20); });
            Property(x => x.Directphone, map => map.Length(20));
            Property(x => x.Extention, map => map.Length(10));
            Property(x => x.Email, map => map.Length(40));
            Property(x => x.UserCode, map => { map.Column("User_Code"); map.Length(10); });
            Property(x => x.DmlType, map => { map.Column("DML_Type"); map.Length(1); });
            Property(x => x.DmlDate, map => map.Column("DML_Date"));
            Property(x => x.Salutation, map => map.Length(10));
            Property(x => x.Status, map => map.Length(1));
        }
    }
}