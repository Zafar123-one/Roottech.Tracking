using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings 
{
    public class GFDtlMap : ClassMapping<GFDtl> {
        
        public GFDtlMap() 
        {
			Table("RSGI_GFDtl_TR");
			Lazy(false);
			Id(x => x.Id, map => map.Column("GFDtl#"));
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            ManyToOne(x => x.GFMst, map =>
            {
                map.Column("GeoFence#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            Property(x => x.GFMstId, map => map.Column("GeoFence#"));
			Property(x => x.CRadius);
			Property(x => x.CLongi);
			Property(x => x.CLati);
			Property(x => x.PLongi);
			Property(x => x.PLati);
			Property(x => x.PSeq);
			Property(x => x.SLeftTopLongi);
			Property(x => x.SLeftTopLati);
			Property(x => x.SRightBotLongi);
			Property(x => x.SRightBotLati);
			Property(x => x.DML_Type);
			Property(x => x.DML_Date);
			Property(x => x.User_Code);
        }
    }
}