using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities {

    [Serializable]
    public class SiteResourceIdentifier
    {
        public virtual int Site_Code { get; set; }
        public virtual int ResourceNo { get; set; }
        //private DateTime _LastModifiedOn;
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as SiteResourceIdentifier;
            if (t == null) return false;
            if (Site_Code == t.Site_Code && ResourceNo == t.ResourceNo) return true;
            return false;
        }
        public override int GetHashCode()
        {
            return (Site_Code + "|" + ResourceNo).GetHashCode();
        }
    }

    [Serializable]
    public class SiteResource : IKeyed<SiteResourceIdentifier>
    {
        private SiteResourceIdentifier _id = new SiteResourceIdentifier();
        public virtual SiteResourceIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /*private ContraBatch _contraBatch;
        public virtual ContraBatch ContraBatch
        {
            get { return _contraBatch; }
            set
            {
                _contraBatch = value;
                if (_contraBatch != null) _id.ID_NL_Contra_Batches = _contraBatch.Id;
            }
        }*/

        //public virtual SiteResourceIdentifier Id { get; set; }
        public virtual int Item_Code { get; set; }
        public virtual int PKG_Code { get; set; }
        public virtual double MinQty { get; set; }
        public virtual double MaxQty { get; set; }
        public virtual double ReorderQty { get; set; }
        public virtual double PriorityCode { get; set; }
        public virtual double llscapacity { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string ResourceType { get; set; }
        public virtual double NoofChambers { get; set; }
        public virtual string LeadTimetype { get; set; }
        public virtual double LeadTime { get; set; }
        public virtual int SensorCodeD1 { get; set; }
        public virtual int SensorCodeD2 { get; set; }
        public virtual int SensorCodeD3 { get; set; }
        public virtual int SensorCodeD4 { get; set; }
        public virtual int SensorCodeD5 { get; set; }
        public virtual int SensorCodeD6 { get; set; }
        public virtual string ResourcePosition { get; set; }
        public virtual int AssetNo { get; set; }
        public virtual int ResourceTypeNo { get; set; }
        public virtual DateTime? ActivationDt { get; set; }
        /*
        public virtual ResourceType ResourceTypeObject { get; set; }
        public IList<UnitGroupDtl> UnitGroupDtls { get; set; }
        public virtual Asset Asset { get; set; }
        */
    }
}
