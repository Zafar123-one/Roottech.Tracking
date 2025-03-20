using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class TodayStatistics : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual double? TripConsume { get; set; }
        public virtual double? FuelConsKm { get; set; }
        public virtual double? TripRefuel { get; set; }
        public virtual double? TripTheft { get; set; }
        public virtual double? TotKm { get; set; }
        public virtual string TotRunHrs { get; set; }
        public virtual double? TotParkHrs { get; set; }
        public virtual double? FuelRateHr { get; set; }
        public virtual double? FuelRateKm { get; set; }
        public virtual double? FuelLvlEnd { get; set; }
        public virtual double? AvgConsume { get; set; }
        public virtual string GridOffHrs { get; set; }
    }
}