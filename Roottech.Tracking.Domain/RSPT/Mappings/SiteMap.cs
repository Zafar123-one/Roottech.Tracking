using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class SiteMap : ClassMapping<Site> {
        
        public SiteMap() {
			Table("RSPT_Site_TR");
			Schema("dbo");
			Lazy(true);
			Id(x => x.Id, map => { map.Column("SITE_Code"); map.Generator(Generators.Identity); });
			Property(x => x.SiteName, map => map.Column("SITE_Name"));
			Property(x => x.Title);
			Property(x => x.SiteType, map => map.Column("SITE_Type"));
			Property(x => x.StartDt);
			Property(x => x.Address);
			Property(x => x.Cperson);
			Property(x => x.Cell, map => map.Column("Cell#"));
			Property(x => x.Phone, map => map.Column("Phone#"));
			Property(x => x.Fax, map => map.Column("Fax#"));
			Property(x => x.Priority);
			Property(x => x.Consite, map => map.Column("#consite"));
			Property(x => x.OrgCode);
			Property(x => x.Longitude);
			Property(x => x.Latitude);
			Property(x => x.SiteCat);
			Property(x => x.DmlType, map => map.Column("DML_Type"));
			Property(x => x.DmlDate, map => map.Column("DML_DAte"));
			Property(x => x.Glong);
			Property(x => x.Glat);
			Property(x => x.SiteOiduType, map => map.Column("site_OIDUType"));
			Property(x => x.ExcCapacity, map => map.Column("Exc_Capacity"));
			Property(x => x.P1AcLoad, map => map.Column("P1_AC_load"));
			Property(x => x.P1CpLoad, map => map.Column("P1_CP_Load"));
			Property(x => x.P2AcLoad, map => map.Column("P2_AC_Load"));
			Property(x => x.P2CpLoad, map => map.Column("P2_CP_Load"));
			Property(x => x.P3AcLoad, map => map.Column("P3_AC_Load"));
			Property(x => x.P3CpLoad, map => map.Column("P3_CP_Load"));

			ManyToOne(x => x.AreaDtl, map => 
			{
				map.Column("Block_Code");
				map.Cascade(Cascade.None);
			});

            ManyToOne(x => x.Area, map =>
            {
                map.Columns(x => x.Name("CityID"), x => x.Name("StateID"), x => x.Name("CountryID"), x => x.Name("AreaID"));
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.City, map =>
            {
                map.Columns(x => x.Name("StateID"), x => x.Name("CountryID"), x => x.Name("CityID"));
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.State, map =>
            {
                map.Columns(x => x.Name("CountryID"), x => x.Name("StateID"));
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.Country, map =>
            {
                map.Column("CountryId");
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.Region, map => 
			{
				map.Column("RegionID");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			/*ManyToOne(x => x.RsmtVendorsSt, map => 
			{
				map.Column("vendorcode");
				map.PropertyRef("Vendorcode");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});*/

			ManyToOne(x => x.Company, map => 
			{
				map.Column("Company_code");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.UserProfile, map => 
			{
				map.Column("User_Code");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});

			ManyToOne(x => x.Poi, map => 
			{
				map.Column("POI#");
				map.NotNullable(true);
				map.Cascade(Cascade.None);
			});
        }
    }
}
