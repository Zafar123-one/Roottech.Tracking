using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class VehicleAmpleView : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string Site_Reg { get; set; }
        public virtual string TripNo { get; set; }
        public virtual string Trip_Status { get; set; }
        public virtual string CurrentEvent { get; set; }
        public virtual DateTime? EventStartDt { get; set; }
        public virtual string TotDuration { get; set; }
        public virtual int Batch { get; set; }
        public virtual double OpenMileage { get; set; }
        public virtual double TotalMileage { get; set; }
        public virtual string LevelType { get; set; }
        public virtual string Currents { get; set; }
        public virtual string UnitID { get; set; }
        public virtual int RegionID { get; set; }
        public virtual string CurBatch { get; set; }
        public virtual int Site_code { get; set; }
        public virtual string Client { get; set; }
        public virtual string Site_Name { get; set; }
        public virtual string ItemType { get; set; }
        public virtual string Priority { get; set; }
        public virtual string Capacity { get; set; }
        public virtual string Min_Level { get; set; }
        public virtual string LastConsumption { get; set; }
        public virtual string Title { get; set; }
        public virtual string Site_Type { get; set; }
        public virtual string Address { get; set; }
        public virtual string ContractPerson { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Cell { get; set; }
        public virtual string Fax { get; set; }
        public virtual string MaxLevel { get; set; }
        public virtual string Resource { get; set; }
        public virtual string BaseVolume { get; set; }
        public virtual string TotLLS { get; set; }
        public virtual string ProjNo { get; set; }
        public virtual string ProjDesc { get; set; }
    }
}
