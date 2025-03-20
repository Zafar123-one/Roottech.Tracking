using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class MonthWiseSummary : IKeyed<double>
    {
        public virtual double Id { get; set; }//OpeningFuel
        public virtual double? OpenningMile { get; set; }
        public virtual double? TotalMileage { get; set; }
        public virtual double? Refuel { get; set; }
        public virtual double? Consume { get; set; }
        public virtual double? ConsumeWithUnCon { get; set; }
        public virtual double? Theft { get; set; }
        public virtual string GridRunHr { get; set; }
        public virtual string TotalRuningHour { get; set; }
        public virtual string CompRunHr { get; set; }
        public virtual double? AverageConsumption { get; set; }
        public virtual double? Closing { get; set; }
        public virtual double? Unconsume { get; set; }
        public virtual double? StabMargin { get; set; }
        public virtual double? CheckBalanceQty { get; set; }
        public virtual double? FuelReturnDrain { get; set; }
        public virtual string AverageConsumptionMile { get; set; }
        public virtual string AverageConsumptionKilometer { get; set; }
        public virtual DateTime OpenDate { get; set; }
    }
}