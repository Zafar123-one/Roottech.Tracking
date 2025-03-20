using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMAA.Entities;

namespace Roottech.Tracking.Domain.SMAA.Mappings
{
    public class GfAssetMap : ClassMapping<GFAsset>
    {
        public GfAssetMap()
        {
            Schema("SMAA");
            Table("GFAsset");
            Lazy(false);
            Id(x => x.Id, map =>
            {
                map.Column("GFAsset#");
                map.Generator(Generators.Identity);
            });
            Property(x => x.OrgCode);
            Property(x => x.AssetNo, map => map.Column("Asset#"));
            Property(x => x.GeoFenceNo, map => map.Column("GeoFence#"));
            Property(x => x.GeoFence);
            Property(x => x.AreaToGF, map => map.Type<NHibernate.Type.EnumStringType<AreaToGF>>());//<NHibernate.Type.EnumStringType`1[[AreaToGF, MooDB]], NHibernate>());
            Property(x => x.GFMargin);
            Property(x => x.User_Code);
            Property(x => x.DML_Type);
            Property(x => x.DML_Date);
        }
    }
}