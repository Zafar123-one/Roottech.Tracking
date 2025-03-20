using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSSP.Entities;

namespace Roottech.Tracking.Domain.RSSP.Mappings {
    public class DevActivityMap : ClassMapping<DevActivity> {
        
        public DevActivityMap() {
			Table("RSSP_DevActivities_HI");
			Lazy(false);
			Id(x => x.Id, map => { map.Column("DevActivity#"); map.Generator(Generators.Identity); });
            Property(x => x.BatchNo, map => map.Column("Batch#"));
            Property(x => x.IoNo, map => map.Column("IO#"));
            Property(x => x.Frtcdttm);
            Property(x => x.Trtcdttm);
            Property(x => x.Totduration);
            Property(x => x.Trdclosedt);
            Property(x => x.DiState, map => map.Column("DI_State"));
            ManyToOne(x => x.EdrFuelRun, map =>
            {
                map.Column("Batch#");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
            /*
                        Property(x => x.Leveltype);
                        Property(x => x.Resource, map => map.Column("Resource#"));
                        Property(x => x.Eventttype);
                        Property(x => x.Device, map => map.Column("Device#"));
                        Property(x => x.Sensorsource, map => map.Column("SensorSource#"));
                        Property(x => x.Devsensor, map => map.Column("DevSensor#"));
                        Property(x => x.Cdrcount);
                        Property(x => x.Sensorcode);
                        Property(x => x.UsTitle, map => map.Column("US_Title"));
                        Property(x => x.Trckactivity);

                        Property(x => x.Durationss);

                        Property(x => x.Durationhh);
                        Property(x => x.Durationmi);
                        Property(x => x.Fmiles);
                        Property(x => x.Tmiles);
                        Property(x => x.Totmiles);
                        Property(x => x.Fkmtrs);
                        Property(x => x.Tkmtrs);
                        Property(x => x.Totkmtrs);
                        Property(x => x.Fmtrs);
                        Property(x => x.Tmtrs);
                        Property(x => x.Totmtrs);
            */
        }
    }
}
