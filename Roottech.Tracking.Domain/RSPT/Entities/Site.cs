using System;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.RSGI.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {
    
    public class Site : IKeyed<double>{

        public virtual double Id { get; set; }
        public virtual AreaDtl AreaDtl { get; set; }
        public virtual Area Area { get; set; }
        public virtual City City { get; set; }
        public virtual State State { get; set; }
        public virtual Country Country { get; set; }
        public virtual Region Region { get; set; }
        //public virtual RsmtVendorsSt RsmtVendorsSt { get; set; }
        public virtual Company Company { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual Poi Poi { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string Title { get; set; }
        public virtual string SiteType { get; set; }
        public virtual DateTime? StartDt { get; set; }
        public virtual string Address { get; set; }
        public virtual string BlockCode { get; set; }
        public virtual string AreaId { get; set; }
        public virtual string CityId { get; set; }
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
        public virtual string TerritoryName { get; set; }
        public virtual string RegionId { get; set; }
        public virtual string Cperson { get; set; }
        public virtual string Cell { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string Priority { get; set; }
        public virtual string Consite { get; set; }
        public virtual string OrgCode { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string Latitude { get; set; }
        public virtual string SiteCat { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string Glong { get; set; }
        public virtual string Glat { get; set; }
        public virtual string SiteOiduType { get; set; }
        public virtual double? ExcCapacity { get; set; }
        public virtual double? P1AcLoad { get; set; }
        public virtual double? P1CpLoad { get; set; }
        public virtual double? P2AcLoad { get; set; }
        public virtual double? P2CpLoad { get; set; }
        public virtual double? P3AcLoad { get; set; }
        public virtual double? P3CpLoad { get; set; }
    }
}
