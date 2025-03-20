using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSMT.Entities;

namespace Roottech.Tracking.Domain.RSMT.Mappings {
    
    public class VendorMap : ClassMapping<Vendor> {
        
        public VendorMap() {
			Table("RSMT_Vendors_ST");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map =>
			{
			    map.Generator(Generators.Identity);
                map.Column("VendorCode");
			});
			Property(x => x.BusinessCatCode, map => map.Column("BusinessCat_Code"));
			Property(x => x.Title);
			Property(x => x.VendorName);
			Property(x => x.ContactPerson);
			Property(x => x.Phone, map => map.Column("Phone#"));
			Property(x => x.Cell, map => map.Column("Cell#"));
			Property(x => x.EmailAddress);
			Property(x => x.WebAddress);
			Property(x => x.MailAddress);
			Property(x => x.AreaId);
			Property(x => x.CityId);
			Property(x => x.PostalZip);
			Property(x => x.StateId);
			Property(x => x.CountryId);
			Property(x => x.HoAddress);
			Property(x => x.HoAreaId);
			Property(x => x.HoCityId);
			Property(x => x.HoPostalZip);
			Property(x => x.HoStateId);
			Property(x => x.HoCountryId);
			Property(x => x.VendorType);
			Property(x => x.OrgCode);
        }
    }
}
