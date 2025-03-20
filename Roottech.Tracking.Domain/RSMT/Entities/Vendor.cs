using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSMT.Entities {
    
    public class Vendor : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual double? BusinessCatCode { get; set; }
        public virtual string Title { get; set; }
        public virtual string VendorName { get; set; }
        public virtual string ContactPerson { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Cell { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string WebAddress { get; set; }
        public virtual string MailAddress { get; set; }
        public virtual string AreaId { get; set; }
        public virtual string CityId { get; set; }
        public virtual string PostalZip { get; set; }
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
        public virtual string HoAddress { get; set; }
        public virtual string HoAreaId { get; set; }
        public virtual string HoCityId { get; set; }
        public virtual string HoPostalZip { get; set; }
        public virtual string HoStateId { get; set; }
        public virtual string HoCountryId { get; set; }
        public virtual string VendorType { get; set; }
        public virtual string OrgCode { get; set; }
    }
}
