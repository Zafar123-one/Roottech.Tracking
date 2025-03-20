using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {

    public class CompleteUnitVw : IKeyed<string>
    {
        public virtual string Id { get; set; } //UnitID
        public virtual string ORGCODE { get; set; }
        // public virtual string UnitID { get; set; }
        public virtual double FleetUnitNo { get; set; }
        public virtual string PlateID { get; set; }
        public virtual double SITE_Code { get; set; }
        public virtual string site_name { get; set; }
        public virtual double ResourceNo { get; set; }
        public virtual int AssetNo { get; set; }
        public virtual string AssetName { get; set; }
        public virtual string specifications { get; set; }
        public virtual double PKG_Code { get; set; }
        public virtual double MinQty { get; set; }
        public virtual double MaxQty { get; set; }
        public virtual double ReorderQty { get; set; }
        public virtual double llscapacity { get; set; }
        public virtual string ResourceType { get; set; }
        public virtual double NoofChambers { get; set; }
        public virtual double di1 { get; set; }
        public virtual double di22 { get; set; }
        public virtual double di3 { get; set; }
        public virtual double di4 { get; set; }
        public virtual double di5 { get; set; }
        public virtual double di6 { get; set; }
    }
}
