using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSPT.Entities;

namespace Roottech.Tracking.Domain.RSPT.Mappings
{
    public class DashboardStationaryV2VwMap : ClassMapping<DashboardStationaryV2Vw> {
        public DashboardStationaryV2VwMap()
        {
            Table("rspt_dashboardStationary_V2_vw");
            Schema("dbo");
            Lazy(true);
            Property(x => x.Batch, map => map.Column("Batch#"));
            Property(x => x.Id, map =>
            {
                map.Column("Unitid");
                map.NotNullable(true);
            });
            Property(x => x.Orgcode);
            Property(x => x.SiteCode, map => { map.Column("SITE_Code"); map.NotNullable(true); });
            Property(x => x.Resource, map => map.NotNullable(true));
            Property(x => x.SiteName);
            Property(x => x.Title);
            Property(x => x.SiteType);
            Property(x => x.AssetName);
            Property(x => x.AssetNo, map => { map.Column("Asset#"); map.NotNullable(true); });
            Property(x => x.Address);
            Property(x => x.BlockCode, map => { map.Column("Block_Code"); map.NotNullable(true); });
            Property(x => x.Areaid);
            Property(x => x.CityId);
            Property(x => x.StateId);
            Property(x => x.Countryid);
            Property(x => x.RegionId);
            Property(x => x.RegionName);
            Property(x => x.ContractPerson);
            Property(x => x.PhoneNo, map => map.Column("Phone#"));
            Property(x => x.CellNo, map => map.Column("Cell#"));
            Property(x => x.FaxNo, map => map.Column("Fax#"));
            Property(x => x.Fleetunit, map => map.Column("FleetUnit#"));
            Property(x => x.Plateid);
            Property(x => x.Client);
            Property(x => x.ItemType);
            Property(x => x.Priority);
            Property(x => x.Capacity);
            Property(x => x.Resourcetype);
            Property(x => x.BaseVolume);
            Property(x => x.MinLevel);
            Property(x => x.MaxLevel);
            Property(x => x.LevelType);
            Property(x => x.LastConsumption);
            Property(x => x.Currents);
            Property(x => x.Totallls);
            Property(x => x.GridDuration);
            Property(x => x.GridStatus);
            Property(x => x.Totduration);
            Property(x => x.ActivationDt);
            Property(x => x.Ustatus, map => map.Column("UStatus#"));
            Property(x => x.DgStatus);
            Property(x => x.OpenDt);
            Property(x => x.Frtcdttm);
            ManyToOne(x => x.EdrFuelRun, map =>
            {
                map.Column("Batch#");
                //map.PropertyRef("UnitID");
                //map.Lazy(LazyRelation.Proxy);
                //map.Insert(false);
                //map.Update(false);
                map.NotNullable(true);
                map.Cascade(Cascade.None);
            });
            ManyToOne(x => x.City, map =>
            {
                map.Columns(
                    colMapper => colMapper.Name("Stateid"),
                    colMapper => colMapper.Name("Countryid"),
                    colMapper => colMapper.Name("Cityid"));
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