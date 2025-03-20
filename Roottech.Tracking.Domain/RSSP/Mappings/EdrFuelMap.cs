using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSSP.Entities;

namespace Roottech.Tracking.Domain.RSSP.Mappings {

    public class EdrFuelMap : ClassMapping<EDRFuel> {
    
        public EdrFuelMap()
        {
			Table("RSSP_EDRFuel_HI");
			Lazy(false);
			Id(x => x.Id , map => map.Column("CDR#"));
			Property(x => x.CDRKEY, map => map.Column("CDRKEY"));
            ManyToOne(x => x.EdrFuelRun, map =>
            {
                map.Column("Batch#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
			Property(x => x.EDRDate, map => map.Column("EDRDate"));
			Property(x => x.EDRTime, map => map.Column("EDRTime"));
			Property(x => x.Event, map => map.Column("Event"));
			Property(x => x.Fueltype, map => map.Column("Fueltype"));
			Property(x => x.UTCTime, map => map.Column("UTCTime"));
			Property(x => x.UTCDTTM, map => map.Column("UTCDTTM"));
			Property(x => x.AI1, map => map.Column("AI1"));
			Property(x => x.AI2, map => map.Column("AI2"));
			Property(x => x.DI, map => map.Column("DI"));
			Property(x => x.DO, map => map.Column("DO"));
			Property(x => x.HexVolume, map => map.Column("HexVolume"));
			Property(x => x.n_Value, map => map.Column("n_Value"));
			Property(x => x.Qty, map => map.Column("Qty"));
			Property(x => x.ContractNo, map => map.Column("Contract#"));
			Property(x => x.SITE_Code, map => map.Column("SITE_Code"));
			Property(x => x.UnitID, map =>
			{
			    map.Column("UnitID");
                map.Type(NHibernateUtil.AnsiString);
                map.Length(20);
			});
			Property(x => x.AV, map => map.Column("AV"));
			Property(x => x.Longitude, map => map.Column("Longitude"));
			Property(x => x.EW, map => map.Column("EW"));
			Property(x => x.Latitude, map => map.Column("Latitude"));
			Property(x => x.NS, map => map.Column("NS"));
			Property(x => x.SatelliteNo, map => map.Column("Satellite#"));
			Property(x => x.GSMSignals, map => map.Column("GSMSignals"));
			Property(x => x.Angle, map => map.Column("Angle"));
			Property(x => x.Speed, map => map.Column("Speed"));
			Property(x => x.Mileage, map => map.Column("Mileage"));
			Property(x => x.RTCTM, map => map.Column("RTCTM"));
			Property(x => x.RTCDTTM, map => map.Column("RTCDTTM"));
			Property(x => x.QtyLtrs, map => map.Column("QtyLtrs"));
			Property(x => x.CDRStatus, map => map.Column("CDRStatus"));
			Property(x => x.Statusdt, map => map.Column("Statusdt"));
			Property(x => x.n_val1, map => map.Column("n_val1"));
			Property(x => x.Qty1);
			Property(x => x.MainPower);
			Property(x => x.DI1, map =>
			{
                map.Type(NHibernateUtil.AnsiString);
			});
			Property(x => x.DI2);
			Property(x => x.DI3);
			Property(x => x.DI4);
			Property(x => x.DI5);
			Property(x => x.DI6);
			Property(x => x.Longi);
			Property(x => x.Lati);
            
            Property(x => x.DiDesc, map =>
            {
                map.Formula("(dbo.fn_getdiarray_descr(dbo.fn_getdiarray(6,DI)))");
                map.Lazy(true);
                map.Insert(false);
                map.Update(false);
                map.Type(NHibernateUtil.String);
            });


            ManyToOne(x => x.CompleteUnitVw, map =>
            {
                map.Column("UnitID");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
        }
    }
}
