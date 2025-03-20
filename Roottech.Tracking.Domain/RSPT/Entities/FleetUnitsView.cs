using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class FleetUnitsView : IKeyed<string> {
        public virtual string OrgCode { get; set; }
        public virtual string Id { get; set; }
        public virtual double? Fleetunit { get; set; }
        public virtual string Plateid { get; set; }
        public virtual double SiteCode { get; set; }
        public virtual double? Dgcap { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string Title { get; set; }
        public virtual double? Resource { get; set; }
        public virtual double? Asset { get; set; }
        public virtual string Assetname { get; set; }
        public virtual string Specifications { get; set; }
        public virtual double? PkgCode { get; set; }
        public virtual double? Minqty { get; set; }
        public virtual double? Maxqty { get; set; }
        public virtual double? Reorderqty { get; set; }
        public virtual double? Llscapacity { get; set; }
        public virtual string Resourcetype { get; set; }
        public virtual double? Ofchambers { get; set; }
        public virtual double? Di1 { get; set; }
        public virtual double? Di22 { get; set; }
        public virtual double? Di3 { get; set; }
        public virtual double? Di4 { get; set; }
        public virtual double? Di5 { get; set; }
        public virtual double? Di6 { get; set; }
        public virtual string Engineumake { get; set; }
        public virtual double? Ustatus { get; set; }
        public virtual DateTime? Activationdt { get; set; }
    }
}
