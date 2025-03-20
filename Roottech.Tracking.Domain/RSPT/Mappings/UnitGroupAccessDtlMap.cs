using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings
{
    public class UnitGroupAccessDtlMap : ClassMapping<UnitGroupAccessDtl>
    {
        public UnitGroupAccessDtlMap()
        {
            Table("RSPT_UGrpAccessDtl_ST");
            Schema("dbo");
            Lazy(false);
            ComponentAsId(x => x.Id,
                map =>
                {
                    map.Class<UnitGroupAccessDtlIdentifier>();
                    map.Property(x => x.Ugrpmst, colMap => colMap.Column("UGrpMst#"));
                    map.Property(x => x.UgrpCode, colMap => colMap.Column("UGRP_Code"));
                });
        }
    }
}