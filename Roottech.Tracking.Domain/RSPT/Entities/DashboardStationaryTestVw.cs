using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.MRAT.Entities;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {

    public class DashboardStationaryTestVw : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string Orgcode { get; set; }
        public virtual double SiteCode { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string Title { get; set; }
        public virtual string SiteType { get; set; }
        public virtual string Address { get; set; }
        public virtual string BlockCode { get; set; }
        public virtual string Areaid { get; set; }
        public virtual string Cityid { get; set; }
        public virtual string Stateid { get; set; }
        public virtual string Countryid { get; set; }
        public virtual string Regionid { get; set; }
        public virtual string Contractperson { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Cell { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Assetname { get; set; }
        public virtual double AssetNo { get; set; }
        public virtual string Fleetunit { get; set; }
        public virtual string Plateid { get; set; }
        public virtual string Client { get; set; }
        public virtual string Itemtype { get; set; }
        public virtual string Priority { get; set; }
        public virtual double? Capacity { get; set; }
        public virtual string Resourcetype { get; set; }
        public virtual int? Basevolume { get; set; }
        public virtual double? MinLevel { get; set; }
        public virtual double? Maxlevel { get; set; }
        public virtual string Leveltype { get; set; }
        public virtual string Currentstatus { get; set; }
        public virtual int? Lastconsumption { get; set; }
        public virtual double? Currents { get; set; }
        public virtual double Resource { get; set; }
        public virtual int? Totlls { get; set; }
        public virtual string Gridact { get; set; }
        public virtual string Gridduration { get; set; }
        public virtual string Totduration { get; set; }
        public virtual DateTime? OpenDt { get; set; }
        public virtual DateTime? Activationdt { get; set; }
        public virtual EDRFuelRun EdrFuelRun { get; set; }
        public virtual Region Region { get; set; }
        public virtual City City { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
