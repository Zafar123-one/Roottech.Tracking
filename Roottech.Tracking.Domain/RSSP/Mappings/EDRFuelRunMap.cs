using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSSP.Entities;

namespace Roottech.Tracking.Domain.RSSP.Mappings
{
    public class EDRFuelRunMap : ClassMapping<EDRFuelRun>
    {
        public EDRFuelRunMap()
        {
            Table("RSSP_EDRFuelRun_HI");
            Lazy(false);
            Id(x => x.Id, map => map.Column("Batch#"));
            Property(x => x.UnitID, map =>
            {
                map.Column("UnitID");
                map.Type(NHibernateUtil.AnsiString);
                map.Length(20);
            });
            Property(x => x.CdrNo, map => map.Column("CDR#"));
            Property(x => x.EventType, map => map.Type(NHibernateUtil.AnsiChar));
            
            Property(x => x.OpenDt);
            Property(x => x.FRTCDTTM);
            Property(x => x.DurationHh);
            Property(x => x.DurationMi);
            Property(x => x.DurationSs);
            Property(x => x.TotDuration);
            Property(x => x.BalanceQty);
            Property(x => x.NQty, map => map.Column("n_qty"));
            Property(x => x.PoiNo, map => map.Column("POI#"));
            Property(x => x.CloseDt);
            Property(x => x.LevelType);
            ManyToOne(x => x.EdrFuel, map =>
            {
                map.Column("CDR#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });

            Bag(x => x.DevActivities, colmap =>
            {
                colmap.Key(x => x.Column("Batch#"));
                colmap.Inverse(true);
            }, map => { map.OneToMany(); });

        //    map.Table("RSPT_SiteResource_TR");
        //    map.Cascade(Cascade.None);
        //    map.Key(k => { k.Column("Asset#"); k.NotNullable(true); });
        //    map.Lazy(CollectionLazy.NoLazy);
        //    map.Fetch(CollectionFetchMode.Join);
        //    map.Inverse(true);
        //}, colMap => colMap.OneToMany(otm => otm.Class(typeof(SiteResource))));
        }
    }
}