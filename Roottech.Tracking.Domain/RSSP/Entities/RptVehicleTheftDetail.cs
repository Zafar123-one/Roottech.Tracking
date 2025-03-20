using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptVehicleTheftDetail : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string Organization { get; set; }
        public virtual string Description { get; set; }
        public virtual string LevelType { get; set; }
        public virtual DateTime OpenDate { get; set; }
        public virtual DateTime CloseDate { get; set; }
        public virtual string TotDuration { get; set; }
        public virtual decimal FuelTheftQty { get; set; }
        public virtual double Longitude { get; set; }
        public virtual double Latitude { get; set; }
        
    }
}