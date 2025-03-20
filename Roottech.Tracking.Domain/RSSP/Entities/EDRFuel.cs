using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities {

    public class EDRFuel : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual string CDRKEY { get; private set; } 
        public virtual EDRFuelRun EdrFuelRun { get; set; }
        public virtual DateTime? EDRDate { get; set; }
        public virtual string EDRTime { get; set; }
        public virtual string Event { get; set; }
        public virtual string Fueltype { get; set; }
        public virtual string UTCTime { get; set; }
        public virtual DateTime? UTCDTTM { get; set; }
        public virtual string AI1 { get; set; }
        public virtual string AI2 { get; set; }
        public virtual string DI { get; set; }
        public virtual string DO { get; set; }
        public virtual string HexVolume { get; set; }
        public virtual double n_Value { get; set; }
        public virtual double Qty { get; set; }
        public virtual double ContractNo { get; set; }
        public virtual double SITE_Code { get; set; }
        public virtual string UnitID { get; set; }
        public virtual string AV { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string EW { get; set; }
        public virtual string Latitude { get; set; }
        public virtual string NS { get; set; }
        public virtual string SatelliteNo { get; set; }
        public virtual string GSMSignals { get; set; }
        public virtual string Angle { get; set; }
        public virtual double Speed { get; set; }
        public virtual string Mileage { get; set; }
        public virtual string RTCTM { get; set; }
        public virtual DateTime? RTCDTTM { get; set; }
        public virtual double QtyLtrs { get; set; }
        public virtual string CDRStatus { get; set; }
        public virtual DateTime? Statusdt { get; set; }
        public virtual double n_val1 { get; set; }
        public virtual double Qty1 { get; set; }
        public virtual string MainPower { get; set; }
        public virtual string DI1 { get; set; }
        public virtual string DI2 { get; set; }
        public virtual string DI3 { get; set; }
        public virtual string DI4 { get; set; }
        public virtual string DI5 { get; set; }
        public virtual string DI6 { get; set; }
        public virtual string Longi { get; set; }
        public virtual string Lati { get; set; }
        public virtual string DiDesc { get; set; }
        public virtual CompleteUnitVw CompleteUnitVw { get; set; }
    }
}
