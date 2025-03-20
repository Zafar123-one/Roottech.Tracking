using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class FuelPrice :IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual string FuelType { get; set; }
        public virtual DateTime? PriceDate { get; set; }
        public virtual double? FuelRate { get; set; }
        public virtual string UnitID { get; set; }
        public virtual string User_code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}
