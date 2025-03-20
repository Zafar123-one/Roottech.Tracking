using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAA.Entities;

namespace Roottech.Tracking.Domain.SMAA.Mappings
{
    public class MonitorAssetMap : ClassMapping<MonitorAsset>
    {
        public MonitorAssetMap()
        {
            Schema("SMAA");
            Table("MonitorAsset");
            Lazy(false);
            Id(x => x.Id, map =>
            {
                map.Column("MonitorAsset#");
                map.Generator(Generators.Identity);
            });
            Property(x => x.OrgCode);
            Property(x => x.AssetNo, map => map.Column("Asset#"));
            Property(x => x.Monitor);
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}