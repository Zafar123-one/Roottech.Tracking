using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class TrckUnit : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual Company Company { get; set; }
        public virtual double? Contract { get; set; }
        public virtual double SiteCode { get; set; }
        public virtual double? ItemCode { get; set; }
        public virtual double? Resource { get; set; }
        public virtual double? TotalUnit { get; set; }
        public virtual double? MinQty { get; set; }
        public virtual double? TotalCapacity { get; set; }
        public virtual double? MaxQty { get; set; }
        public virtual double? ReorderQty { get; set; }
        public virtual double? OpenBal { get; set; }
        public virtual DateTime? OpenDate { get; set; }
        public virtual double? BalanceQty { get; set; }
        public virtual DateTime? RefuelDate { get; set; }
        public virtual double? RefuelQty { get; set; }
        public virtual DateTime? ConsumeDate { get; set; }
        public virtual double? ConsumeQty { get; set; }
        public virtual DateTime? RunFrom { get; set; }
        public virtual DateTime? RunTo { get; set; }
        public virtual string RunHrs { get; set; }
        public virtual double? RunConsume { get; set; }
        public virtual double? RunRefuel { get; set; }
        public virtual double? Lls { get; set; }
        public virtual string Orgcode { get; set; }
        public virtual double? Ustatus { get; set; }
        public virtual double? Reason { get; set; }
        public virtual double? Mileage { get; set; }
        public virtual double? Lls1qty { get; set; }
        public virtual double? Lls2qty { get; set; }
        public virtual string OldUnitId { get; set; }
    }
}
