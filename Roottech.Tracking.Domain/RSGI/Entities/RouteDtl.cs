using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class RouteDtlIdentifier
    {
        public virtual int RouteNo { get; set; }
        public virtual int PoiNo { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var t = obj as RouteDtlIdentifier;
            if (t == null) return false;
            if (RouteNo == t.RouteNo && PoiNo == t.PoiNo) return true;
            return false;
        }
        public override int GetHashCode()
        {
            return (RouteNo + "|" + PoiNo).GetHashCode();
        }
    }

    public class RouteDtl : IKeyed<RouteDtlIdentifier>
    {
        private RouteDtlIdentifier _id = new RouteDtlIdentifier();
        
        public virtual RouteDtlIdentifier Id
        {
            get { return _id; }
            set { _id = value; }
        }

        //public virtual int myPoi { get; set; }
        public virtual int PoiSeq { get; set; }
        public virtual Poi Poi { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime DML_Date { get; set; }
    }
}