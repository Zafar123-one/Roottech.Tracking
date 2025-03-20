using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.MRAT.Entities;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class DashboardStationaryV2Vw : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual double? Batch { get; set; }
        public virtual string Orgcode { get; set; }
        public virtual double SiteCode { get; set; }
        public virtual double Resource { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string Title { get; set; }
        public virtual string SiteType { get; set; }
        public virtual string AssetName { get; set; }
        public virtual double AssetNo { get; set; }
        public virtual string Address { get; set; }
        public virtual string BlockCode { get; set; }
        public virtual string Areaid { get; set; }
        public virtual string CityId { get; set; }
        public virtual string StateId { get; set; }
        public virtual string Countryid { get; set; }
        public virtual string RegionId { get; set; }
        public virtual string RegionName { get; set; }
        public virtual string ContractPerson { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual string CellNo { get; set; }
        public virtual string FaxNo { get; set; }
        public virtual double? Fleetunit { get; set; }
        public virtual string Plateid { get; set; }
        public virtual string Client { get; set; }
        public virtual string ItemType { get; set; }
        public virtual string Priority { get; set; }
        public virtual double? Capacity { get; set; }
        public virtual string Resourcetype { get; set; }
        public virtual int? BaseVolume { get; set; }
        public virtual double? MinLevel { get; set; }
        public virtual double? MaxLevel { get; set; }
        public virtual string LevelType { get; set; }
        public virtual int? LastConsumption { get; set; }
        public virtual double? Currents { get; set; }
        public virtual int? Totallls { get; set; }
        public virtual string GridDuration { get; set; }
        public virtual string GridStatus { get; set; }
        public virtual string Totduration { get; set; }
        public virtual DateTime? ActivationDt { get; set; }
        public virtual double? Ustatus { get; set; }
        public virtual string DgStatus { get; set; }
        public virtual DateTime? OpenDt { get; set; }
        public virtual DateTime? Frtcdttm { get; set; }
        public virtual string Lati { get; set; }
        public virtual string Longi { get; set; }
        public virtual EDRFuelRun EdrFuelRun { get; set; }
        public virtual decimal PerLevel { get; set; }
        public virtual City City { get; set; }
        public virtual Region Region { get; set; }
        public virtual Asset Asset { get; set; }
    }
}