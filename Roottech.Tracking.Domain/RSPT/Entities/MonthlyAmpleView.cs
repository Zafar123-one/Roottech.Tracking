using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class MonthlyAmpleView : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual double Opening_Fuel { get; set; }
        public virtual double OpenMile { get; set; }
        public virtual double Refuel { get; set; }
        public virtual double Theft { get; set; }
        public virtual double Consume { get; set; }
        public virtual string totRuninghr { get; set; }
        public virtual double TotalMileage { get; set; }
        public virtual double TotalKM { get; set; }
        public virtual double Avgkmltr { get; set; }
        public virtual string IdleHours { get; set; }
        public virtual string CompRunHr { get; set; }
        public virtual double Avg_Consum { get; set; }
        public virtual double closing { get; set; }
        public virtual double netfuel { get; set; }
        public virtual double CloseNetFuel { get; set; }
        public virtual double BlQty { get; set; }
        public virtual double FuelReturn { get; set; }
        public virtual double Avg_Consumption_Mile { get; set; }
        public virtual double Avg_Consumption_KM { get; set; }
        public virtual double Consume1 { get; set; }
    }
}