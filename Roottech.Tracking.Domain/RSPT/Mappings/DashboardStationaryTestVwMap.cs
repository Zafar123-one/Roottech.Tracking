using System;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings {
    
    public class DashboardStationaryTestVwMap : ClassMapping<DashboardStationaryTestVw> {

        public DashboardStationaryTestVwMap()
        {
            Table("rspt_dashboardStationary_Test_vw");
            Schema("dbo");
            Lazy(true);
            Property(x => x.Id, map =>
            {
                map.Column("Unitid");
                map.NotNullable(true);
            });
            Property(x => x.Orgcode);
            Property(x => x.SiteCode, map =>
            {
                map.Column("SITE_Code");
                map.NotNullable(true);
            });
            Property(x => x.SiteName, map => map.Column("SITE_Name"));
            Property(x => x.Title);
            Property(x => x.SiteType, map => map.Column("Site_type"));
            Property(x => x.Address);
            Property(x => x.BlockCode, map =>
            {
                map.Column("Block_Code");
                map.NotNullable(true);
            });
            Property(x => x.Areaid);
            Property(x => x.Cityid);
            Property(x => x.Stateid);
            Property(x => x.Countryid);
            Property(x => x.Regionid);
            Property(x => x.Contractperson);
            Property(x => x.Phone, map => map.Column("Phone#"));
            Property(x => x.Cell, map => map.Column("Cell#"));
            Property(x => x.Fax, map => map.Column("Fax#"));

            Property(x => x.Assetname);
            Property(x => x.AssetNo, map =>
            {
                map.Column("Asset#");
                map.NotNullable(true);
            });
            Property(x => x.Fleetunit, map => map.NotNullable(true));
            Property(x => x.Plateid, map => map.NotNullable(true));
            Property(x => x.Client);
            Property(x => x.Itemtype);
            Property(x => x.Priority);
            Property(x => x.Capacity);
            Property(x => x.Resourcetype);
            Property(x => x.Basevolume);
            Property(x => x.MinLevel, map => map.Column("Min_Level"));
            Property(x => x.Maxlevel);
            Property(x => x.Leveltype);
            Property(x => x.Currentstatus, map => map.NotNullable(true));
            Property(x => x.Lastconsumption);
            Property(x => x.Currents);
            Property(x => x.Resource, map => map.NotNullable(true));
            Property(x => x.Totlls);
            Property(x => x.Gridact);
            Property(x => x.Gridduration);
            Property(x => x.Totduration);
            Property(x => x.Activationdt);
            Property(x => x.OpenDt);

            ManyToOne(x => x.EdrFuelRun, map =>
            {
                map.Column("UnitID");
                map.PropertyRef("UnitID");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });


            ManyToOne(x => x.Region, map =>
            {
                map.Column("Regionid");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });


            ManyToOne(x => x.City, map =>
            {
                map.Columns(
                    colMapper =>colMapper.Name("Stateid"),
                    colMapper => colMapper.Name("Countryid"),
                    colMapper =>colMapper.Name("Cityid"));
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });

            ManyToOne(x => x.Asset, map =>
            {
                map.Column("Asset#");
                map.Lazy(LazyRelation.Proxy);
                map.Insert(false);
                map.Update(false);
            });
        }
    }
}
