using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class CurrentStatus : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string AssetName { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string Depot { get; set; }
        public virtual string UnitUsageDesc { get; set; }
        public virtual string UnitTypeDesc { get; set; }
        public virtual string RegDt { get; set; }
        public virtual string UnitId { get; set; }
        public virtual string Rtcdttm { get; set; }
        public virtual double? Ignition { get; set; }
        public virtual double? Speed { get; set; }
        public virtual double? FuelQuantity { get; set; }
        public virtual double? Temperature { get; set; }
        public virtual double? Longi { get; set; }
        public virtual double? Lati { get; set; }
        public virtual double? Battery { get; set; }
        public virtual int  EventNo { get; set; }
        public virtual string Activity { get; set; }
        public virtual DateTime? BatchDt { get; set; }
        public virtual DateTime? EventStart { get; set; }
        public virtual DateTime? EventEnd { get; set; }
        public virtual string TotalDuration { get; set; }
        public virtual double? FromMileage { get; set; }
        public virtual double? CurrentMileage { get; set; }
        public virtual double? NetMileage { get; set; }
        public virtual double? NetMileageInKm { get; set; }
        public virtual double? FuelOpenBal { get; set; }
        public virtual double? CurrentFuelBal { get; set; }
        public virtual double? NetFuelBal { get; set; }
        public virtual double? TotalCapacity { get; set; }
        public virtual double? Compressor { get; set; }
    }
}
