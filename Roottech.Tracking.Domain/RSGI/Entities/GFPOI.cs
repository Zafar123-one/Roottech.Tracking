using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class GFPOIIdentifier
    {
        public virtual int GeoFenceNo { get; set; }
        public virtual int PoiNo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as GFPOIIdentifier;
            if (t == null) return false;
            return GeoFenceNo == t.GeoFenceNo && PoiNo == t.PoiNo;
        }

        public override int GetHashCode()
        {
            return (GeoFenceNo + "|" + PoiNo).GetHashCode();
        }
    }

    public class GFPOI : IKeyed<GFPOIIdentifier>
    {
        private GFPOIIdentifier _id = new GFPOIIdentifier();

        public virtual GFPOIIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual int ORGCODE { get; set; }
        public virtual string Company_code { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}