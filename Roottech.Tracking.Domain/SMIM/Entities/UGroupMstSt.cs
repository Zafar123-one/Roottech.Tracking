using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities {
    
    public class UGroupMstSt : IKeyed<double>
    {
        public virtual double Id { get; set; }
/*        public virtual SmsaCompaniesSt SmsaCompaniesSt { get; set; }
        public virtual SmsaRegionsSt SmsaRegionsSt { get; set; }
        public virtual SmimUserprofiileSt SmimUserprofiileSt { get; set; }*/
        public virtual string UgrpName { get; set; }
        public virtual string UgrpStatus { get; set; }
        public virtual string Superadmin { get; set; }
        public virtual string Admin { get; set; }
        public virtual string Orgcode { get; set; }
        public virtual string Regionwise { get; set; }
        public virtual string Clientwise { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string RegionId { get; set; }
        public virtual string CompanyCode { get; set; }
        public virtual string UserCode { get; set; }
    }
}
