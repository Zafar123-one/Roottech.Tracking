using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.SMSA.Mappings
{
    public class AreaDtlMap : ClassMapping<AreaDtl>
    {
        public AreaDtlMap()
        {
            Table("SMSA_AreasDTL_ST");
            Lazy(false);
            Id(x => x.Id, map => map.Column("Block_Code"));
            Property(x => x.AreaId);
            Property(x => x.CityId);
            Property(x => x.StateId);
            Property(x => x.CountryId);
            Property(x => x.Block_Title);
            Property(x => x.Block_Description);
            Property(x => x.ORGCode);
            Property(x => x.User_Code);
        }
    }
}