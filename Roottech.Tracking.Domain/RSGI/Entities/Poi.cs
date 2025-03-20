using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class Poi : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string Company_Code { get; set; }
        public virtual string PoiName { get; set; }
        public virtual int PoiTypeNo { get; set; }
        public virtual PoiType PoiType { get; set; }
        public virtual string PostalAddr { get; set; }
        public virtual string Block_Code { get; set; }
        public virtual string AreaId { get; set; }
        public virtual string CityId { get; set; }
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
        public virtual decimal Lati { get; set; }
        public virtual decimal Longi { get; set; }
        public virtual string GF_YN { get; set; }
        public virtual int GF_Val { get; set; }
        public virtual string CustomerRefNo { get; set; }
        public virtual string ContactPerson { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Cell { get; set; }
        public virtual string Email { get; set; }
        public virtual string User_Code { get; set; }
    }
}