using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.SMIM.Mappings
{
    public class UGroupDtlStMap : ClassMapping<UGroupDtlSt>
    {
        public UGroupDtlStMap()
        {
            Table("SMIM_UGroupDTL_ST");
            Schema("dbo");
            Lazy(true);
            ComponentAsId(x => x.Id,
                map =>
                {
                    map.Class<UGroupDtlStIdentifier>();
                    map.Property(x => x.UgrpCode, m => m.Column("UGRP_Code"));
                    map.Property(x => x.UserCode, m => m.Column("User_Code"));
                });
            Property(x => x.UserCode2, map => map.Column("User_Code2"));
            Property(x => x.DmlType, map => map.Column("DML_Type"));
            Property(x => x.DmlDate, map => map.Column("DML_Date"));
        }
    }
}
