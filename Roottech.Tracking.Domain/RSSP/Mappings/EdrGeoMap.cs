using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSSP.Entities;

namespace Roottech.Tracking.Domain.RSSP.Mappings
{
    public class EdrGeoMap : ClassMapping<EdrGeo>
    {
        public EdrGeoMap()
        {
            Table("RSSP_EDRGEO_HI");
            Lazy(false);
            Id(x => x.Id, map => map.Column("GEOCDR#"));
            Property(x => x.CdrNo, map => map.Column("CDR#"));
            Property(x => x.RTCDTTM, map => map.Column("RTCDTTM"));
            Property(x => x.Longitude, map => map.Column("longitude"));
            Property(x => x.Latitude, map => map.Column("latitude"));
            Property(x => x.GeoFenceNo, map => map.Column("GeoFence#"));
            Property(x => x.IoType, map => map.Column("IOType"));
            Property(x => x.PullDt, map => map.Column("PullDt"));
            Property(x => x.UpdateDt, map => map.Column("UpdateDt"));
        }
    }
}