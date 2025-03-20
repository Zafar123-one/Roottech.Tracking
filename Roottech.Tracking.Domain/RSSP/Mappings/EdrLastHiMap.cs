using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSSP.Entities;

namespace Roottech.Tracking.Domain.RSSP.Mappings {
    
    public class EdrLastHiMap : ClassMapping<EdrLastHi> {

        public EdrLastHiMap()
        {
            Table("RSSP_EDRLast_HI");
            Schema("dbo");
            Lazy(true);
            Id(x => x.Id, map =>
            {
                map.Column("Unitid");
                map.Generator(Generators.Assigned);
            });
            Property(x => x.CdrNo, map => map.Column("CDR#"));
            Property(x => x.Rtcdttm);
            Property(x => x.DmlDate, map => map.Column("DML_Date"));
            Property(x => x.Battery);
            Property(x => x.Ai1);
            Property(x => x.Ai2);
            Property(x => x.Tempr1);
            Property(x => x.Devstate, map => map.Column("DevState#"));
            Property(x => x.Ustatus, map => map.Column("UStatus#"));
            Property(x => x.Msgoord);
            Property(x => x.Moortcdttm);
            Property(x => x.NrCount, map => map.Column("NR_Count"));
            Property(x => x.NrDate, map => map.Column("NR_Date"));
            ManyToOne(x => x.EdrFuel, map =>
            {
                map.Column("CDR#");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
        }
    }
}
