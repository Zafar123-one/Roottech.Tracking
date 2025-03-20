using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class CurrentStatusAmpleView : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string Event { get; set; }
        public virtual string Site { get; set; }
        public virtual DateTime? Fromdt { get; set; }
        public virtual DateTime? Todate { get; set; }
        public virtual string TotalDuration { get; set; }
        public virtual double? FromMileage { get; set; }
        public virtual double? CurrentMileage { get; set; }
        public virtual double? NetMileage { get; set; }
        public virtual double? NetMileage_KM { get; set; }
        public virtual double? FromFuelBal { get; set; }
        public virtual double? CurrentFuelBal { get; set; }
        public virtual double? NetFuelBal { get; set; }
        public virtual string MaxSpeed { get; set; }
        public virtual string IdlHr { get; set; }
        public virtual string Ignition { get; set; }
        public virtual string DI { get; set; }
        public virtual double Batteryval { get; set; }
    }
}