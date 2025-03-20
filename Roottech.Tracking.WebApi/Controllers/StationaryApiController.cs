using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Castle.Core.Internal;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.MRAT.Entities;
using Roottech.Tracking.Domain.OMOP.Entities;
using Roottech.Tracking.Domain.RSMT.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Domain.SMAA.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;
using Roottech.Tracking.Library.Models;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.Models.Helpers;
using Roottech.Tracking.Library.Utils;
using Roottech.Tracking.RTM.UI.Web.Infrastructure;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class StationaryApiController : ApiController
    {
        private readonly IKeyedRepository<double, Asset> _repositoryAsset;
        private readonly IKeyedRepository<double, ContractMst> _repositoryContractMst;
        private readonly IKeyedRepository<double, EDRFuelRun> _repositoryEdrFuelRun;
        private readonly IKeyedRepository<double, FuelPrice> _repositoryFuelPrice;
        private readonly IKeyedRepository<double, Site> _repositorySite;
        private readonly ISession _session;
        private readonly IKeyedRepository<CityIdentifier, City> _repositoryCity;
        private readonly IKeyedRepository<string, Region> _repositoryRegion;
        private readonly IKeyedRepository<OrgModuleIdentifier, OrgModule> _repositoryOrgModule;
        private readonly IKeyedRepository<string, DashboardStationaryV2Vw> _repositoryDashboardStationaryV2Vw;
        private readonly IKeyedRepository<UGroupDtlStIdentifier, UGroupDtlSt> _repositoryUGroupDtlSt;
        private readonly IKeyedRepository<double, UGroupMstSt> _repositoryUGroupMstSt;
        private readonly IKeyedRepository<string, FleetUnitsView> _repositoryFleetUnitsView;

        public StationaryApiController(
            IKeyedRepository<double, Site> repositorySite, IKeyedRepository<double, ContractMst> repositoryContractMst,
            IKeyedRepository<double, Asset> repositoryAsset, IKeyedRepository<double, EDRFuelRun> repositoryEdrFuelRun,
            ISessionFactory sessionFactory, IKeyedRepository<double, FuelPrice> repositoryFuelPrice,
            IKeyedRepository<CityIdentifier, City> repositoryCity, IKeyedRepository<string, Region> repositoryRegion,
            IKeyedRepository<OrgModuleIdentifier, OrgModule> repositoryOrgModule,
            IKeyedRepository<string, DashboardStationaryV2Vw> repositoryDashboardStationaryV2Vw,
            IKeyedRepository<UGroupDtlStIdentifier, UGroupDtlSt> repositoryUGroupDtlSt,
            IKeyedRepository<double, UGroupMstSt> repositoryUGroupMstSt,
            IKeyedRepository<string, FleetUnitsView> repositoryFleetUnitsView)
        {
            _repositorySite = repositorySite;
            _repositoryContractMst = repositoryContractMst;
            _repositoryAsset = repositoryAsset;
            _repositoryEdrFuelRun = repositoryEdrFuelRun;
            _repositoryFuelPrice = repositoryFuelPrice;
            _repositoryCity = repositoryCity;
            _repositoryRegion = repositoryRegion;
            _repositoryOrgModule = repositoryOrgModule;
            _repositoryDashboardStationaryV2Vw = repositoryDashboardStationaryV2Vw;
            _repositoryUGroupDtlSt = repositoryUGroupDtlSt;
            _repositoryUGroupMstSt = repositoryUGroupMstSt;
            _repositoryFleetUnitsView = repositoryFleetUnitsView;
            _session = sessionFactory.GetCurrentSession();
        }

        [HttpGet]
        public bool SelectedOrganizationIsBank()
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            return _repositoryOrgModule.QueryOver()
                .Select(x => x.PagePath)
                .Where(x => x.OrgCode == principal.FindFirst("UserOrgCode").Value)
                .WithSubquery.WhereProperty(x => x.Id.AaModNo).In(QueryOver.Of<Module>()
                    .Select(Projections.Distinct(Projections.Property<Module>(x => x.Id)))
                    .Where(x => x.ModTitleCode == "FMS"))
                .SingleOrDefault<string>().ToLower().IndexOf("bank") > -1;
        }

        /*private IList<DashboardStationaryTestVw> GetDashboardStationaryTestVws(GridSettings grid)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            var userCode = principal.FindFirst("User_Code").Value;
            int totalRecords;
            Region region = null;
            EDRFuel edrFuel = null;
            EDRFuelRun edrFuelRun = null;
            City city = null;
            DashboardStationaryTestVw dashboardStationaryTestVw = null;

            var dashboardStationaryQueryover = _repositoryDashboardStationaryTestVw.QueryOver()
                .Where(x => x.Orgcode == orgCode)
                .JoinAlias(c => c.City, () => city, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.Region, () => region, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.EdrFuelRun, () => edrFuelRun)
                .JoinAlias(() => edrFuelRun.EdrFuel, () => edrFuel)

                .Where(() => edrFuelRun.CloseDt == null)
                .WithSubquery.WhereProperty(x => x.Regionid).In(QueryOver.Of<UGroupMstSt>()
                    .Select(Projections.Distinct(Projections.Property<UGroupMstSt>(x => x.RegionId)))
                    .Where(x => x.Orgcode == orgCode)
                    .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<UGroupDtlSt>()
                        .Select(Projections.Distinct(Projections.Property<UGroupDtlSt>(x => x.Id.UgrpCode)))
                        .Where(x => x.Id.UserCode == userCode)))
                .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<TrckUnit>()
                    .Select(Projections.Distinct(Projections.Property<TrckUnit>(x => x.Id)))
                    .Where(x => x.Ustatus == 1))
                //.OrderBy(x => x.Regionid).Asc.ThenBy(x => x.SiteName).Asc.ThenBy(x => x.Assetname).Asc
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => dashboardStationaryTestVw.Id)
                    .Select(() => edrFuel.Lati).WithAlias(() => dashboardStationaryTestVw.Title)
                    .Select(() => edrFuel.Longi).WithAlias(() => dashboardStationaryTestVw.Gridact)

                    /*old .Select(Projections.SqlFunction("concat", NHibernateUtil.String,
                Projections.Constant("("), Projections.Property(() => region.Regionname), Projections.Constant(")-("),
                Projections.Property<DashboardStationaryTestVw>(e => e.SiteName), Projections.Constant(")-("),
                Projections.Property<DashboardStationaryTestVw>(e => e.Assetname), Projections.Constant(")"))).WithAlias(() => dashboardStationary.Description)#1#

                    .Select(Projections.SqlFunction("FN_GetEventType", NHibernateUtil.String,
                        Projections.Property(() => edrFuelRun.EventType),
                        Projections.Property<DashboardStationaryTestVw>(p => p.Resourcetype),
                        Projections.Property<DashboardStationaryTestVw>(p => p.Orgcode)))
                    .WithAlias(() => dashboardStationaryTestVw.Gridduration)
                    .Select(x => x.SiteName).WithAlias(() => dashboardStationaryTestVw.SiteName)
                    .Select(x => x.Assetname).WithAlias(() => dashboardStationaryTestVw.Assetname)
                    .Select(() => region.Regionname).WithAlias(() => dashboardStationaryTestVw.Regionid)
                    .Select(x => x.Itemtype).WithAlias(() => dashboardStationaryTestVw.Itemtype)
                    .Select(x => x.Capacity).WithAlias(() => dashboardStationaryTestVw.Capacity)
                    .Select(x => x.Basevolume).WithAlias(() => dashboardStationaryTestVw.Basevolume)
                    .Select(x => x.MinLevel).WithAlias(() => dashboardStationaryTestVw.MinLevel)
                    .Select(x => x.Currents).WithAlias(() => dashboardStationaryTestVw.Currents)
                    .Select(x => x.Leveltype).WithAlias(() => dashboardStationaryTestVw.Leveltype)
                    .Select(x => x.Totduration).WithAlias(() => dashboardStationaryTestVw.Totduration)
                    .Select(x => x.OpenDt).WithAlias(() => dashboardStationaryTestVw.OpenDt)
                    .Select(() => city.CityName).WithAlias(() => dashboardStationaryTestVw.Cityid)
                )
                .TransformUsing(Transformers.AliasToBean<DashboardStationaryTestVw>());

            return dashboardStationaryQueryover.SearchGrid(grid, out totalRecords);
        }
*/
        private IList<DashboardStationaryV2Vw> GetDashboardStationaryV2Vws(GridSettings grid)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            var userCode = principal.FindFirst("User_Code").Value;

            var cookie = Request.Headers.GetCookies("session-id").FirstOrDefault();
            if (cookie != null)
            {
                //sessionId = cookie["session-id"].Values["User_Code"]// .Value;
                orgCode = cookie["session-id"].Values["UserOrgCode"];
                userCode = cookie["session-id"].Values["User_Code"];
            }

            int totalRecords;

            EDRFuel edrFuel = null;
            EDRFuelRun edrFuelRun = null;
            DashboardStationaryV2Vw dashboardStationaryV2Vw = null;
            City city = null;
            var projectionGridStatus =
                Projections.Conditional(
                    Restrictions.Eq(Projections.Property<DashboardStationaryV2Vw>(x => x.GridStatus), "1"),
                    Projections.Constant("ON"), Projections.Constant("OFF"));

            var UgrpCodes = _repositoryUGroupDtlSt.QueryOver()
                .Where(x => x.Id.UserCode == userCode)
                .Select(x => x.Id.UgrpCode)//Projections.Distinct(Projections.Property<UGroupDtlSt>(x => x.Id.UgrpCode)))
            .List<string>().ToArray();

            var regionIds = _repositoryUGroupMstSt.QueryOver()
                .Where(x => x.Orgcode == orgCode).AndRestrictionOn(x => x.Id).IsIn(UgrpCodes)
                .Select(Projections.Distinct(Projections.Property<UGroupMstSt>(x => x.RegionId)))
                .List<string>().ToArray();

            var decimalType = TypeFactory.Basic("decimal(20,2)");
            var DashboardStationaryV2Queryover = _repositoryDashboardStationaryV2Vw.QueryOver()
                .JoinAlias(c => c.EdrFuelRun, () => edrFuelRun)
                .JoinAlias(() => edrFuelRun.EdrFuel, () => edrFuel)
                .JoinAlias(c => c.City, () => city)
                .Where(() => edrFuelRun.CloseDt == null)
                .And(x => x.Orgcode == orgCode && x.DgStatus != "Undefine")
                .AndRestrictionOn(x => x.RegionId).IsIn(regionIds);

            if (grid.Where != null)
            {
                var i = 0;
                foreach (var rule in grid.Where.rules)
                {
                    if (rule.field == "PerLevel")
                    {
                        DashboardStationaryV2Queryover.And(
                            Restrictions.Le(
                                Projections.SqlFunction(new VarArgsSQLFunction("(", "/", ")*100"),
                                    NHibernateUtil.Double,
                                    Projections.Property<DashboardStationaryV2Vw>(x => x.Currents),
                                    Projections.Property<DashboardStationaryV2Vw>(x => x.Capacity))
                                , rule.data));
                        grid.Where.rules[i] = new Rule();
                    }
                    else if (rule.field == "MinimumFuel")
                    {
                        if (rule.data == "true")
                            DashboardStationaryV2Queryover.And(
                                Restrictions.LtProperty(
                                    Projections.Property<DashboardStationaryV2Vw>(x => x.Currents),
                                    Projections.Property<DashboardStationaryV2Vw>(x => x.MinLevel)));
                        grid.Where.rules[i] = new Rule();
                    }
                    i++;
                }
            }

            DashboardStationaryV2Queryover.WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<TrckUnit>()
                    .Select(Projections.Distinct(Projections.Property<TrckUnit>(x => x.Id)))
                    .Where(x => x.Ustatus == 1))

                .WithSubquery.WhereProperty(x => x.Resource).In(QueryOver.Of<UnitGroupDtl>()
                    .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(x => x.ResourceNo)))
                    //.Where(x => x.Orgcode == orgCode)
                    .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<UnitGroupAccessDtl>()
                        .Select(Projections.Distinct(Projections.Property<UnitGroupAccessDtl>(x => x.Id.Ugrpmst)))
                        .AndRestrictionOn(x => x.Id.UgrpCode).IsIn(UgrpCodes)))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => dashboardStationaryV2Vw.Id)
                    .Select(x => x.Title).WithAlias(() => dashboardStationaryV2Vw.Title)
                    .Select(x => x.SiteCode).WithAlias(() => dashboardStationaryV2Vw.SiteCode)
                    .Select(x => x.RegionName).WithAlias(() => dashboardStationaryV2Vw.RegionName)
                    .Select(x => x.AssetName).WithAlias(() => dashboardStationaryV2Vw.AssetName)
                    .Select(x => x.SiteName).WithAlias(() => dashboardStationaryV2Vw.SiteName)
                    .Select(x => x.Capacity).WithAlias(() => dashboardStationaryV2Vw.Capacity)
                    .Select(x => x.Currents).WithAlias(() => dashboardStationaryV2Vw.Currents)
                    .Select(x => x.MinLevel).WithAlias(() => dashboardStationaryV2Vw.MinLevel)
                    .Select(
                    Projections.SqlFunction(
                        new SQLFunctionTemplate(decimalType, "cast(?1 as decimal(20,2))"),
                            decimalType,

                            Projections.SqlFunction(new VarArgsSQLFunction("(", "/", ")*100"), NHibernateUtil.Double, Projections.Property<DashboardStationaryV2Vw>(x => x.Currents), Projections.Property<DashboardStationaryV2Vw>(x => x.Capacity))
                        )).WithAlias(() => dashboardStationaryV2Vw.PerLevel)
                    .Select(x => x.LevelType).WithAlias(() => dashboardStationaryV2Vw.LevelType)
                    .Select(x => x.Totduration).WithAlias(() => dashboardStationaryV2Vw.Totduration)
                    .Select(x => x.DgStatus).WithAlias(() => dashboardStationaryV2Vw.DgStatus)
                    .Select(projectionGridStatus).WithAlias(() => dashboardStationaryV2Vw.GridStatus)
                    .Select(x => x.GridDuration).WithAlias(() => dashboardStationaryV2Vw.GridDuration)
                    .Select(x => x.AssetNo).WithAlias(() => dashboardStationaryV2Vw.AssetNo)
                    .Select(x => x.Frtcdttm).WithAlias(() => dashboardStationaryV2Vw.Frtcdttm)
                .Select(() => edrFuel.Lati).WithAlias(() => dashboardStationaryV2Vw.Lati)
                .Select(() => edrFuel.Longi).WithAlias(() => dashboardStationaryV2Vw.Longi)
                .Select(() => city.CityName).WithAlias(() => dashboardStationaryV2Vw.CityId)
                )
                .TransformUsing(Transformers.AliasToBean<DashboardStationaryV2Vw>());
            return DashboardStationaryV2Queryover.SearchGrid(grid, out totalRecords);
        }

        public dynamic GetDashboardStationariesForMap(GridSettings grid, string sidx, string sord, int page, int rows, bool isBank)
        {
            int totalRecords; 
            //if (isBank)
            {
                var dashboardStationaries = GetDashboardStationaryV2Vws(grid);
                totalRecords = dashboardStationaries.Count();

                var pageSize = rows;
                var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);

                return new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    userdata = new {isBank, DateTime.Now},
                    rows = (from props in dashboardStationaries
                        select new
                        {
                            props.Id,
                            props.Lati,
                            props.Longi,
                            props.Title,
                            props.SiteCode,
                            props.RegionName,
                            props.AssetName,
                            props.SiteName,
                            props.CityId,
                            props.Capacity,
                            props.Currents,
                            props.MinLevel,
                            props.PerLevel,
                            props.LevelType,
                            props.DgStatus,
                            props.Totduration,
                            props.GridStatus,
                            props.GridDuration,
                            props.AssetNo,
                            props.Frtcdttm
                        })
                };
            }/*
            else
            {

                var dashboardStationaries = GetDashboardStationaryTestVws(grid);
                totalRecords = dashboardStationaries.Count();
                var pageSize = rows;
                var totalPages = (int) Math.Ceiling((float) totalRecords/pageSize);
                return new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    userdata = new {isBank},
                    rows = (from props in dashboardStationaries
                        select new
                        {
                            props.Id,
                            props.Title,
                            props.Gridact,
                            props.Gridduration,
                            props.SiteName,
                            props.Assetname,
                            props.Regionid,
                            props.Itemtype,
                            props.Capacity,
                            props.Basevolume,
                            props.MinLevel,
                            props.Currents,
                            props.Leveltype,
                            props.Totduration,
                            props.Cityid,
                            props.OpenDt
                        })
                };
            }*/
        }

        public void GetDashboardStationariesForMapEe(GridSettings grid, bool isBank, bool all)
        {
            if (all) grid.PageSize = 0;
            if (isBank)
            {
                string[,] colNamesToReplace = new string[15, 2];

                colNamesToReplace[0, 0] = "RegionName";
                colNamesToReplace[0, 1] = "Region";

                colNamesToReplace[1, 0] = "CityId";
                colNamesToReplace[1, 1] = "City";

                colNamesToReplace[2, 0] = "Title";
                colNamesToReplace[2, 1] = "Branch Code";

                colNamesToReplace[3, 0] = "SiteName";
                colNamesToReplace[3, 1] = "Branch";

                colNamesToReplace[4, 0] = "AssetName";
                colNamesToReplace[4, 1] = "Asset";

                colNamesToReplace[5, 0] = "Capacity";
                colNamesToReplace[5, 1] = "Capicity(Ltrs)";

                colNamesToReplace[6, 0] = "Basevolume";
                colNamesToReplace[6, 1] = "Base Volume(Ltrs)";

                colNamesToReplace[7, 0] = "MinLevel";
                colNamesToReplace[7, 1] = "Min Fuel Level(Ltrs)";

                colNamesToReplace[8, 0] = "Currents";
                colNamesToReplace[8, 1] = "Current Fuel(Ltrs)";

                colNamesToReplace[9, 0] = "LevelType";
                colNamesToReplace[9, 1] = "Fuel Level(%)";

                colNamesToReplace[10, 0] = "DgStatus";
                colNamesToReplace[10, 1] = "DG Status";

                colNamesToReplace[11, 0] = "Totduration";
                colNamesToReplace[11, 1] = "DG Duration";

                colNamesToReplace[12, 0] = "GridStatus";
                colNamesToReplace[12, 1] = "Grid Status";

                colNamesToReplace[13, 0] = "GridDuration";
                colNamesToReplace[13, 1] = "Grid Duration";

                colNamesToReplace[14, 0] = "OpenDt";
                colNamesToReplace[14, 1] = "Live Message Date";

                var dashboardStationaries = GetDashboardStationaryV2Vws(grid);
                ExportFilesByHttpResponse.ExportToExcel(dashboardStationaries.AsQueryable(),
                    props => new
                    {
                        //props.SiteCode,
                        props.RegionName,
                        props.CityId,
                        props.Title,
                        props.SiteName,
                        props.AssetName,
                        props.Capacity,
                        props.Currents,
                        props.MinLevel,
                        //props.PerLevel,
                        props.LevelType,
                        props.DgStatus,
                        props.Totduration,
                        props.GridStatus,
                        props.GridDuration,
                        //props.AssetNo,
                        props.OpenDt
                    }, null, "Dashboard Stationary", colNamesToReplace);
            }
            else
            {
                string[,] colNamesToReplace = new string[12, 2];

                colNamesToReplace[0, 0] = "RegionId";
                colNamesToReplace[0, 1] = "Region";

                colNamesToReplace[1, 0] = "Cityid";
                colNamesToReplace[1, 1] = "City";

                colNamesToReplace[2, 0] = "SiteName";
                colNamesToReplace[2, 1] = "Site";

                colNamesToReplace[3, 0] = "Assetname";
                colNamesToReplace[3, 1] = "Asset";

                colNamesToReplace[4, 0] = "Itemtype";
                colNamesToReplace[4, 1] = "Asset Type";

                colNamesToReplace[5, 0] = "Capacity";
                colNamesToReplace[5, 1] = "Capicity(Ltrs)";

                colNamesToReplace[6, 0] = "Basevolume";
                colNamesToReplace[6, 1] = "Base Volume(Ltrs)";

                colNamesToReplace[7, 0] = "MinLevel";
                colNamesToReplace[7, 1] = "Min Fuel Level(Ltrs)";

                colNamesToReplace[8, 0] = "Currents";
                colNamesToReplace[8, 1] = "Current Fuel(Ltrs)";

                colNamesToReplace[9, 0] = "Leveltype";
                colNamesToReplace[9, 1] = "Fuel Level";

                colNamesToReplace[10, 0] = "Totduration";
                colNamesToReplace[10, 1] = "Current Status";

                colNamesToReplace[11, 0] = "OpenDt";
                colNamesToReplace[11, 1] = "Duration";
                var dashboardStationaries = GetDashboardStationaryV2Vws(grid);
                ExportFilesByHttpResponse.ExportToExcel(dashboardStationaries.AsQueryable(),
                    props => new
                    {
                        props.SiteName,
                        props.AssetName,
                        props.RegionId,
                        props.ItemType,
                        props.Capacity,
                        props.BaseVolume,
                        props.MinLevel,
                        props.Currents,
                        props.LevelType,
                        props.Totduration,
                        props.CityId,
                        props.OpenDt
                    }, null, "Dashboard Stationary", colNamesToReplace);
            }
        }

        public DashboardStationaryDetail GetDashboardStationaryDetailsByUnitId(string unitId)
        {
            Region region = null;
            DashboardStationaryDetail dashboardStationaryDetail = null;
            EDRFuelRun edrFuelRun = null;
            DashboardStationaryV2Vw dashboardStationaryV2Vw = null;

            return _repositoryDashboardStationaryV2Vw.QueryOver(() => dashboardStationaryV2Vw)
                .Where(x => x.Id == unitId)
                .JoinAlias(c => c.Region, () => region, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.EdrFuelRun, () => edrFuelRun)
                .SelectList(list => list
                    //.Select(x => x.SiteCode).WithAlias(() => dashboardStationary.SiteCode)
                    .Select(x => x.SiteName).WithAlias(() => dashboardStationaryDetail.SiteName)
                    //.Select(x => x.SiteType).WithAlias(() => dashboardStationary.SiteType)
                    .Select(x => x.AssetName).WithAlias(() => dashboardStationaryDetail.AssetName)
                    //.Select(x => x.Regionid).WithAlias(() => dashboardStationary.Regionid)
                    .Select(() => region.Regionname).WithAlias(() => dashboardStationaryDetail.RegionName)
                    .Select(x => x.ItemType).WithAlias(() => dashboardStationaryDetail.AssetType)
                    .Select(x => x.Capacity).WithAlias(() => dashboardStationaryDetail.Capacity)
                    .Select(x => x.BaseVolume).WithAlias(() => dashboardStationaryDetail.BaseVolume)
                    .Select(x => x.MinLevel).WithAlias(() => dashboardStationaryDetail.MinLevel)
                    .Select(x => x.Currents).WithAlias(() => dashboardStationaryDetail.Currents)
                    .Select(x => x.LevelType).WithAlias(() => dashboardStationaryDetail.LevelType)
                    .Select(x => x.Totduration).WithAlias(() => dashboardStationaryDetail.TotDuration)
                    .Select(x => x.Frtcdttm).WithAlias(() => dashboardStationaryDetail.ActivationDt)
                    .SelectSubQuery(QueryOver.Of<Asset>().Where(ast => ast.Id == dashboardStationaryV2Vw.AssetNo)
                        .Select(ast => ast.Dgcap)).WithAlias(() => dashboardStationaryDetail.Dgcap)
                    //Select DGCap,* from MRAT_Assets_TR where Asset# = 80
/*                     .Select(x => x.Client).WithAlias(() => dashboardStationary.Client)
                     .Select(x => x.Priority).WithAlias(() => dashboardStationary.Priority)
                     .Select(x => x.Lastconsumption).WithAlias(() => dashboardStationary.Lastconsumption)
                     .Select(x => x.AssetNo).WithAlias(() => dashboardStationary.AssetNo)
                     .Select(x => x.Title).WithAlias(() => dashboardStationary.Title)
                     .Select(x => x.Address).WithAlias(() => dashboardStationary.Address)
                     .Select(x => x.Contractperson).WithAlias(() => dashboardStationary.Contractperson)*/
                    .Select(Projections.SqlFunction("FN_GetEventType", NHibernateUtil.String,
                        Projections.Property(() => edrFuelRun.EventType),
                        Projections.Property<DashboardStationaryV2Vw>(p => p.Resourcetype),
                        Projections.Property<DashboardStationaryV2Vw>(p => p.Orgcode)))
                    .WithAlias(() => dashboardStationaryDetail.EventName)
                )
                .Where(() => edrFuelRun.CloseDt == null)
                .TransformUsing(Transformers.AliasToBean<DashboardStationaryDetail>())
                .SingleOrDefault<DashboardStationaryDetail>();
        }

        public dynamic GetMultipleDatasetsForAmpleViewByUnitId(string unitId)
        {
            DashboardStationaryV2Vw dashboardStationaryV2Vw = null;
            dashboardStationaryV2Vw = _repositoryDashboardStationaryV2Vw.QueryOver().Where(x => x.Id == unitId)
                .SelectList(list => list
                    .Select(x => x.SiteCode).WithAlias(() => dashboardStationaryV2Vw.SiteCode)
                    .Select(x => x.Address).WithAlias(() => dashboardStationaryV2Vw.Address)
                    .Select(x => x.AssetNo).WithAlias(() => dashboardStationaryV2Vw.AssetNo))
                .TransformUsing(Transformers.AliasToBean<DashboardStationaryV2Vw>())
                .SingleOrDefault<DashboardStationaryV2Vw>();

            var siteId = dashboardStationaryV2Vw.SiteCode;
            var assetNo = dashboardStationaryV2Vw.AssetNo;

            Site site = null;
            AreaDtl areaDtl = null;
            Area area = null;
            City city = null;
            State state = null;
            Country country = null;
            Region region = null;

            var projectionSiteOiduType =
                Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.SiteOiduType), "O"),
                    Projections.Constant("ODU"),
                    Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.SiteOiduType), "I"),
                        Projections.Constant("IDU"), Projections.Constant("N/A")));

            var projectionPriority =
                Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.Priority), "A"),
                    Projections.Constant("P1"),
                    Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.Priority), "B"),
                        Projections.Constant("P2"),
                        Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.Priority), "C"),
                            Projections.Constant("P3"), Projections.Constant("N/A"))));

            var projectionSiteCat =
                Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.SiteCat), "P"),
                    Projections.Constant("Prime"),
                    Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.SiteCat), "T"),
                        Projections.Constant("Temporary Prime"),
                        Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.SiteCat), "S"),
                            Projections.Constant("Stand-by"),
                            Projections.Conditional(Restrictions.Eq(Projections.Property<Site>(x => x.SiteCat), "O"),
                                Projections.Constant("Others"), Projections.Constant("N/A")))));

            var futureSite = _repositorySite.QueryOver(() => site)
                .JoinAlias(c => c.AreaDtl, () => areaDtl, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.Area, () => area, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.City, () => city, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.State, () => state, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.Country, () => country, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.Region, () => region, JoinType.LeftOuterJoin)
                .SelectList(list => list
                    .Select(() => areaDtl.Block_Description).WithAlias(() => site.BlockCode)
                    .Select(() => area.AreaName).WithAlias(() => site.AreaId)
                    .Select(() => city.CityName).WithAlias(() => site.CityId)
                    .Select(() => state.StateName).WithAlias(() => site.StateId)
                    .Select(() => country.CountryName).WithAlias(() => site.CountryId)
                    .Select(() => region.Regionname).WithAlias(() => site.RegionId)
                    .Select(x => x.Phone).WithAlias(() => site.Phone)
                    .Select(x => x.Cell).WithAlias(() => site.Cell)
                    .Select(x => x.Fax).WithAlias(() => site.Fax)
                    .Select(x => x.SiteName).WithAlias(() => site.SiteName)
                    .Select(x => x.Title).WithAlias(() => site.Title)
                    .Select(x => x.Address).WithAlias(() => site.Address)
                    .SelectSubQuery(QueryOver.Of<TerritoryMst>()
                    .WithSubquery.WhereProperty(ast => ast.Id).In(QueryOver.Of<TerritoryDtl>().Where(x => x.BlockCode == areaDtl.Id).Select(x =>x.TerritoryMst.Id))
                     .Select(ast => ast.Title)).WithAlias(() => site.TerritoryName)
                    .Select(projectionSiteOiduType).WithAlias(() => site.SiteOiduType)
                    .Select(projectionPriority).WithAlias(() => site.Priority)
                    .Select(projectionSiteCat).WithAlias(() => site.SiteCat)
                    .Select(Projections.SqlFunction("FN_GetSiteType", NHibernateUtil.String,
                        Projections.Property<Site>(x => x.SiteType), Projections.Property<Site>(x => x.Consite)))
                    .WithAlias(() => site.SiteType)
                )
                .Where(x => x.Id == siteId).TransformUsing(Transformers.AliasToBean<Site>()).FutureValue<Site>();

            ContractMst contractMst = null;
            ContSite contSite = null;
            ProjectMst projectMst = null;
            Vendor vendor = null;
            var contractMsts = _repositoryContractMst.QueryOver(() => contractMst)
                .JoinAlias(c => c.ContSites, () => contSite, JoinType.InnerJoin)
                .JoinAlias(c => c.ProjectMsts, () => projectMst, JoinType.InnerJoin)
                .JoinAlias(c => c.Vendor, () => vendor, JoinType.LeftOuterJoin)
                .Where(() => contSite.Id.SiteCode == siteId)
                .SelectList(list => list
                    .Select(() => projectMst.Title).WithAlias(() => contractMst.MasterContractNo)
                    .Select(x => x.Title).WithAlias(() => contractMst.Title)
                    .Select(x => x.DateFrom).WithAlias(() => contractMst.DateFrom)
                    .Select(x => x.DateTo).WithAlias(() => contractMst.DateTo)
                    .Select(x => x.ContrType).WithAlias(() => contractMst.ContrType)
                    .Select(() => vendor.VendorName).WithAlias(() => contractMst.ToVendorCode))
                .TransformUsing(Transformers.AliasToBean<ContractMst>()).Future<ContractMst>();


            Asset asset = null;
            AssetGroup assetGroup = null;
            AssetStatus assetStatus = null;
            Condition condition = null;
            dashboardStationaryV2Vw = null;
            SiteResource siteResources = null;
            var futureAsset = _repositoryAsset.QueryOver(() => asset)
                .JoinAlias(c => c.AssetGroup, () => assetGroup, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.AssetStatus, () => assetStatus, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.Condition, () => condition, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.DashboardStationaryV2, () => dashboardStationaryV2Vw, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.SiteResources, () => siteResources, JoinType.LeftOuterJoin)
                //.Where(() => dashboardStationaryTestVw.SiteCode == siteId)
                .Where(() => asset.Id == assetNo)
                .SelectList(list => list
                    .Select(x => x.AssetName).WithAlias(() => asset.AssetName)
                    .Select(x => x.inventoryNo).WithAlias(() => asset.inventoryNo)
                    .Select(() => assetGroup.AssetGroupName).WithAlias(() => asset.AssetGroupName)
                    .Select(x => x.AssetStatusNo).WithAlias(() => asset.AssetStatusNo)
                    .Select(() => assetStatus.Name).WithAlias(() => asset.AssetStatusName)
                    .Select(() => condition.Name).WithAlias(() => asset.ATConditionName)
                    .Select(x => x.AssetSNo).WithAlias(() => asset.AssetSNo)
                    .Select(x => x.Dgcap).WithAlias(() => asset.Dgcap)
                    .Select(() => siteResources.llscapacity).WithAlias(() => asset.NoOfTanks)
                    .Select(x => x.EngineUMake).WithAlias(() => asset.EngineUMake)
                    .Select(x => x.AlternatorMake).WithAlias(() => asset.AlternatorMake)
                    .Select(x => x.InstalYear).WithAlias(() => asset.InstalYear)
                    .Select(x => x.FuelCapacity).WithAlias(() => asset.FuelCapacity)
                    .Select(() => dashboardStationaryV2Vw.Capacity).WithAlias(() => asset.ColCapacity)
                    .Select(() => siteResources.ActivationDt).WithAlias(() => asset.StartDate)
                    .Select(() => siteResources.Id.ResourceNo).WithAlias(() => asset.ResourceNo))
                .TransformUsing(Transformers.AliasToBean<Asset>()).FutureValue<Asset>();

            EDRFuelRun edrFuelRun = null;
            EDRFuel edrFuel = null;

            var projectionLevelType =
                Projections.Conditional(Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.LevelType), "D"),
                    Projections.Constant("Currently DG-ON"), Projections.Constant("Currently DG-OFF"));
            var projectionEventType =
                Projections.Conditional(Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "C"),
                    Projections.Constant("Currently DG-ON"),
                    Projections.Conditional(Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "R"),
                        Projections.Constant("Currently Refuel"),
                        Projections.Conditional(
                            Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "N"),
                            Projections.Constant("Currently DG-OFF"),
                            Projections.Conditional(
                                Restrictions.Or(
                                    Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "I"),
                                    Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "D")),
                                Projections.Constant("Currently Stabilization"), Projections.Constant("N/A")))));

            var futureEdrFuelRun = _repositoryEdrFuelRun.QueryOver(() => edrFuelRun)
                .JoinAlias(c => c.EdrFuel, () => edrFuel)
                .Where(x => x.UnitID == unitId).AndRestrictionOn(x => x.CloseDt).IsNull
                .WithSubquery.WhereProperty(x => x.OpenDt).Eq(
                    QueryOver.Of<EDRFuelRun>().Where(x => x.UnitID == unitId).AndRestrictionOn(x => x.CloseDt).IsNull
                        .Select(Projections.Max<EDRFuelRun>(x => x.OpenDt)))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => edrFuelRun.Id)
                    .Select(x => x.OpenDt).WithAlias(() => edrFuelRun.OpenDt)
                    .Select(x => x.FRTCDTTM).WithAlias(() => edrFuelRun.FRTCDTTM)
                    .Select(x => x.TotDuration).WithAlias(() => edrFuelRun.TotDuration)
                    .Select(x => x.NQty).WithAlias(() => edrFuelRun.NQty)
                    .Select(Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                            NHibernateUtil.Double,
                            Projections.Property<EDRFuelRun>(x => x.BalanceQty),
                            Projections.Property<EDRFuelRun>(x => x.NQty))).WithAlias(() => edrFuelRun.BalanceQty)
                    .Select(projectionLevelType).WithAlias(() => edrFuelRun.LevelType)
                    .Select(projectionEventType).WithAlias(() => edrFuelRun.EventTypeName)
                    .Select(() => edrFuel.AI1).WithAlias(() => edrFuelRun.AI1)
                    .Select(() => edrFuel.AI2).WithAlias(() => edrFuelRun.AI2)
                    .Select(() => edrFuel.DI).WithAlias(() => edrFuelRun.DI))
                .TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).FutureValue<EDRFuelRun>();
            edrFuelRun = null;
            projectionEventType =
                Projections.Conditional(Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "C"),
                    Projections.Constant("Last DG-ON"),
                    Projections.Conditional(Restrictions.Eq(Projections.Property<EDRFuelRun>(x => x.EventType), "D"),
                        Projections.Constant("Last Decrease Stabilization"), Projections.Constant("N/A")));
            var futureEdrFuelRunLastConsume = _repositoryEdrFuelRun.QueryOver()
                .Where(x => x.UnitID == unitId)
                .AndRestrictionOn(x => x.TotDuration)
                .IsNotNull //.AndRestrictionOn(x => x.CloseDt).IsNull
                .WithSubquery.WhereProperty(x => x.OpenDt).Eq(
                    QueryOver.Of<EDRFuelRun>()
                        .Where(x => x.UnitID == unitId && x.EventType == 'C')
                        .And(Restrictions.Gt(Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                            NHibernateUtil.Double,
                            Projections.Property<EDRFuelRun>(x => x.BalanceQty),
                            Projections.Property<EDRFuelRun>(x => x.NQty)), 0))
                        .AndRestrictionOn(x => x.CloseDt)
                        .IsNotNull
                        .AndRestrictionOn(x => x.TotDuration).IsNotNull
                        .Select(Projections.Max<EDRFuelRun>(x => x.OpenDt)))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => edrFuelRun.Id)
                    .Select(x => x.OpenDt).WithAlias(() => edrFuelRun.OpenDt)
                    .Select(x => x.FRTCDTTM).WithAlias(() => edrFuelRun.FRTCDTTM)
                    .Select(x => x.TotDuration).WithAlias(() => edrFuelRun.TotDuration)
                    .Select(Projections.Constant(0)).WithAlias(() => edrFuelRun.NQty)
                    .Select(Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                            NHibernateUtil.Double,
                            Projections.Property<EDRFuelRun>(x => x.BalanceQty),
                            Projections.Property<EDRFuelRun>(x => x.NQty))).WithAlias(() => edrFuelRun.BalanceQty)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.LevelType)
                    .Select(projectionEventType).WithAlias(() => edrFuelRun.EventTypeName)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI1)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI2)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.DI))
                .TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).FutureValue<EDRFuelRun>();
                //.FutureValue<EDRFuelRun>();


            projectionEventType =
                Projections.Conditional(Restrictions.Eq(Projections.Group<EDRFuelRun>(x => x.EventType), "R"),
                    Projections.Constant("Last Refuel"), Projections.Constant("N/A"));
            var futureEdrFuelRunLastRefuel = _repositoryEdrFuelRun.QueryOver(() => edrFuelRun)
                .Where(x => x.UnitID == unitId && x.EventType == 'R').AndRestrictionOn(x => x.CloseDt).IsNotNull
                .And(Restrictions.EqProperty(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String, Projections.Property<EDRFuelRun>(x => x.OpenDt)),
                Projections.SubQuery(QueryOver.Of<EDRFuelRun>()
                        .Where(x => x.UnitID == unitId && x.EventType == 'R')
                        .AndRestrictionOn(x => x.CloseDt).IsNotNull
                        .Select(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String, Projections.Max<EDRFuelRun>(x => x.OpenDt))))))
                /*.WithSubquery.WhereProperty(x => x.OpenDt).Eq(QueryOver.Of<EDRFuelRun>()
                        .Where(x => x.UnitID == unitId && x.EventType == 'R')
                        .AndRestrictionOn(x => x.CloseDt).IsNotNull
                        .Select(Projections.Max<EDRFuelRun>(x => x.OpenDt)))*/
                .SelectList(list => list
                    .Select(Projections.Min<EDRFuelRun>(x => x.Id)).WithAlias(() => edrFuelRun.Id)
                    .Select(Projections.Min<EDRFuelRun>(x => x.OpenDt)).WithAlias(() => edrFuelRun.OpenDt)
                    .Select(Projections.Max<EDRFuelRun>(x => x.FRTCDTTM)).WithAlias(() => edrFuelRun.FRTCDTTM)
                    .Select(Projections.SqlFunction("FN_GetDuration", NHibernateUtil.String,
                        Projections.Min<EDRFuelRun>(x => x.OpenDt), Projections.Max<EDRFuelRun>(x => x.CloseDt)))
                    .WithAlias(() => edrFuelRun.TotDuration)
                    .Select(Projections.Count<EDRFuelRun>(x => x.EventType)).WithAlias(() => edrFuelRun.NQty)
                    .Select(Projections.Sum(
                        Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                            NHibernateUtil.Double,
                            Projections.Property(() => edrFuelRun.NQty),
                            Projections.Property(() => edrFuelRun.BalanceQty)))).WithAlias(() => edrFuelRun.BalanceQty)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.LevelType)
                    .Select(projectionEventType).WithAlias(() => edrFuelRun.EventTypeName)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI1)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI2)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.DI))
                .TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).FutureValue<EDRFuelRun>();

            projectionEventType =
                Projections.Conditional(Restrictions.Eq(Projections.Group<EDRFuelRun>(x => x.EventType), "T"),
                    Projections.Constant("Last Theft"), Projections.Constant("N/A"));
            
            var futureEdrFuelRunLastTheft = _repositoryEdrFuelRun.QueryOver(() => edrFuelRun)
                .Where(x => x.UnitID == unitId && x.EventType == 'T').AndRestrictionOn(x => x.CloseDt).IsNotNull
                .And(Restrictions.EqProperty(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String, Projections.Property<EDRFuelRun>(x => x.OpenDt)),
                Projections.SubQuery(QueryOver.Of<EDRFuelRun>()
                        .Where(x => x.UnitID == unitId && x.EventType == 'T')
                        .AndRestrictionOn(x => x.CloseDt).IsNotNull
                        .Select(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String,Projections.Max<EDRFuelRun>(x => x.OpenDt))))))
                /*.WithSubquery.WhereProperty(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String, Projections.Property<EDRFuelRun>(x => x.OpenDt))).Eq(
                    QueryOver.Of<EDRFuelRun>()
                        .Where(x => x.UnitID == unitId && x.EventType == 'T')
                        .AndRestrictionOn(x => x.CloseDt).IsNotNull
                        .Select(Projections.Max(Projections.Property<EDRFuelRun>(x => x.OpenDt))))*/
                .SelectList(list => list
                    .Select(Projections.Min<EDRFuelRun>(x => x.Id)).WithAlias(() => edrFuelRun.Id)
                    .Select(Projections.Min<EDRFuelRun>(x => x.OpenDt)).WithAlias(() => edrFuelRun.OpenDt)
                    .Select(Projections.Max<EDRFuelRun>(x => x.CloseDt)).WithAlias(() => edrFuelRun.FRTCDTTM)
                    .Select(Projections.SqlFunction("FN_GetDuration", NHibernateUtil.String,
                        Projections.Min<EDRFuelRun>(x => x.OpenDt), Projections.Max<EDRFuelRun>(x => x.CloseDt)))
                    .WithAlias(() => edrFuelRun.TotDuration)
                    .Select(Projections.Count<EDRFuelRun>(x => x.EventType)).WithAlias(() => edrFuelRun.NQty)
                    .Select(Projections.Sum(
                        Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                            NHibernateUtil.Double,
                            Projections.Property(() => edrFuelRun.BalanceQty),
                            Projections.Property(() => edrFuelRun.NQty)))).WithAlias(() => edrFuelRun.BalanceQty)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.LevelType)
                    .Select(projectionEventType).WithAlias(() => edrFuelRun.EventTypeName)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI1)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI2)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.DI))
                .TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).FutureValue<EDRFuelRun>();

            DevActivity devActivity = null;
            DevActivity dDevActivity = null;

            projectionEventType =
                Projections.Conditional(Restrictions.Eq(Projections.Property(() => devActivity.DiState), "1"),
                    Projections.Constant("Currently GRID-ON"), Projections.Constant("Currently Grid-OFF"));

            var futureEdrFuelRunCurrentGrid = _repositoryEdrFuelRun.QueryOver(() => edrFuelRun).
                Left.JoinAlias(x => x.DevActivities, () => devActivity)
                .Where(x => x.UnitID == unitId)
                .AndRestrictionOn(() => devActivity.Trdclosedt).IsNull
                .And(() => devActivity.IoNo == 2 && devActivity.Totduration != "00:00:00")
                .WithSubquery.WhereProperty(x => x.OpenDt).Eq(
                    QueryOver.Of<EDRFuelRun>()
                        .Where(x => x.UnitID == unitId)
                        .AndRestrictionOn(x => x.CloseDt).IsNull
                        .Select(Projections.Max<EDRFuelRun>(x => x.OpenDt)))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => edrFuelRun.Id)
                    .Select(() => devActivity.Frtcdttm).WithAlias(() => edrFuelRun.OpenDt)
                    .Select(() => devActivity.Trtcdttm).WithAlias(() => edrFuelRun.FRTCDTTM)
                    .Select(() => devActivity.Totduration).WithAlias(() => edrFuelRun.TotDuration)
                    .Select(Projections.Constant(0)).WithAlias(() => edrFuelRun.NQty)
                    .Select(Projections.Constant(0)).WithAlias(() => edrFuelRun.BalanceQty)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.LevelType)
                    .Select(projectionEventType).WithAlias(() => edrFuelRun.EventTypeName)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI1)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI2)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.DI))
                .TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).Take(1).FutureValue<EDRFuelRun>();

            var futureEdrFuelRunLastGrid = _repositoryEdrFuelRun.QueryOver(() => edrFuelRun).
                Left.JoinAlias(x => x.DevActivities, () => devActivity)
                .Where(x => x.UnitID == unitId)
                .And(() => devActivity.IoNo == 2 && devActivity.DiState == "1" && devActivity.Totduration != "00:00:00")
                .WithSubquery.WhereProperty(x => x.OpenDt).Eq(
                    QueryOver.Of<EDRFuelRun>()
                    .Left.JoinAlias(x => x.DevActivities, () => dDevActivity)
                        .Where(x => x.UnitID == unitId)
                        .And(() => dDevActivity.IoNo == 2 && dDevActivity.DiState == "1" && dDevActivity.Totduration != "00:00:00")
                        .AndRestrictionOn(() => dDevActivity.Trdclosedt).IsNotNull
                        .Select(Projections.Max<EDRFuelRun>(x => x.OpenDt)))
                .OrderBy(() => devActivity.Frtcdttm).Desc
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => edrFuelRun.Id)
                    .Select(() => devActivity.Frtcdttm).WithAlias(() => edrFuelRun.OpenDt)
                    .Select(() => devActivity.Trtcdttm).WithAlias(() => edrFuelRun.FRTCDTTM)
                    .Select(() => devActivity.Totduration).WithAlias(() => edrFuelRun.TotDuration)
                    .Select(Projections.Constant(0)).WithAlias(() => edrFuelRun.NQty)
                    .Select(Projections.Constant(0)).WithAlias(() => edrFuelRun.BalanceQty)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.LevelType)
                    .Select(Projections.Constant("Last GRID-ON")).WithAlias(() => edrFuelRun.EventTypeName)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI1)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.AI2)
                    .Select(Projections.Constant("")).WithAlias(() => edrFuelRun.DI))
                .TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).FutureValue<EDRFuelRun>();

            var fuelPrice = _repositoryFuelPrice.QueryOver()
                .Where(x => x.FuelType == "D")
                .And(Restrictions.EqProperty(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String, Projections.Property<FuelPrice>(x => x.PriceDate)),
                Projections.SubQuery(QueryOver.Of<FuelPrice>()
                        .Where(x => x.FuelType == "D")
                        //.And(Expression.Sql("DATEDIFF(day,PriceDate,GetDate()) < 0 ", "", NHibernateUtil.DateTime))
                        .And(x => x.PriceDate < DateTime.Today)
                        .Select(Projections.SqlFunction("FN_GetDateOnly", NHibernateUtil.String, Projections.Max<FuelPrice>(x => x.PriceDate))))))
                .Select(x => x.FuelRate).FutureValue<double>().Value;


            site = futureSite.Value;
            asset = futureAsset.Value;
            var currentLevel = futureEdrFuelRun.Value;
            var lastConsume = futureEdrFuelRunLastConsume.Value;
            var lastRefuel = futureEdrFuelRunLastRefuel.Value;
            var lastTheft = futureEdrFuelRunLastTheft.Value;
            var currentGrid = futureEdrFuelRunCurrentGrid.Value;
            var lastGrid = futureEdrFuelRunLastGrid.Value;
            var monthWiseSummary = GetMonthWiseSummaryByUnitIdAndMonthAndYear(unitId, string.Empty, string.Empty);
            TodayStatistics todayStatistics = GetStatisticsByUnitIdAndOpenDateAndFuelRate(unitId, DateTime.Today, fuelPrice);
            IList<SensorInfoVoltage> sensorInfoVoltage = _session.GetNamedQuery("GetSensorInfoVoltage").SetString("UnitId", unitId).SetInt32("Resource", asset.ResourceNo).List<SensorInfoVoltage>();
            IList<SensorInfo> sensorInfo = _session.GetNamedQuery("GetSensorInfo").SetInt32("Resource", asset.ResourceNo).SetString("Di", currentLevel.DI).List<SensorInfo>();
            return new
            {
                site,
                asset,
                contractMsts,
                currentLevel,
                lastConsume,
                lastRefuel,
                lastTheft,
                monthWiseSummary,
                todayStatistics,
                sensorInfo,
                fuelPrice,
                currentGrid,
                lastGrid,
                sensorInfoVoltage
            };

            //for union http://stackoverflow.com/questions/18623146/sql-union-to-nhibernate-criteria
        }

        public TodayStatistics GetStatisticsByUnitIdAndOpenDateAndFuelRate(string unitId, DateTime openDate, double fuelRate)
        {
            return _session.GetNamedQuery("GetTodayStatistics").SetString("UnitId", unitId).SetDateTime("OpenDt", openDate).SetDouble("FuelRate", fuelRate).UniqueResult<TodayStatistics>();
        }

        public MonthWiseSummary GetMonthWiseSummaryByUnitIdAndMonthAndYear(string unitId, string month, string year)
        {
            MonthWiseSummary monthWiseSummary = null;
            try
            {
                month = month.IsNullOrEmpty() ? DateTime.Today.Month.ToString() : month;
                year = year.IsNullOrEmpty() ? DateTime.Today.Year.ToString() : year;
                monthWiseSummary = _session.GetNamedQuery("GetMonthWiseSummary").SetString("UnitId", unitId).SetString("Year", year).SetString("Month", month).UniqueResult<MonthWiseSummary>();
                //monthWiseSummary.Consume1 = 1.1;//Convert.ToDouble(month);
                //monthWiseSummary.CheckBalanceQty = Convert.ToDouble(year);

            }
            catch (Exception)
            {
                return monthWiseSummary;
            }
            
            return monthWiseSummary;
        }

        public IList<EDRFuelRun> GetRefuelDetailsForStationaryByUnitIdAndEventTypeAndMonthAndYear(string unitId, char eventType, string month, string year)
        {

            month = month.IsNullOrEmpty() ? DateTime.Today.Month.ToString() : month;
            year = year.IsNullOrEmpty() ? DateTime.Today.Year.ToString() : year;
            var firstDayOfMonthDate = new DateTime(Int32.Parse(year), Int32.Parse(month), 1);
            var lastDayOfMonthDate = firstDayOfMonthDate.AddMonths(1).AddDays(-1);

            EDRFuelRun edrFuelRun = null;
            return _repositoryEdrFuelRun.QueryOver(() => edrFuelRun)
                .Where(x => x.UnitID == unitId && x.EventType == eventType)
                .And(Expression.Sql("DATEDIFF(day,opendt,?) <= 0 ", firstDayOfMonthDate, NHibernateUtil.DateTime))
                .And(Expression.Sql("DATEDIFF(day,opendt,?) >= 0 ", lastDayOfMonthDate, NHibernateUtil.DateTime))
                .OrderBy(x => x.Id).Desc
                .SelectList(list => list
                .Select(x => x.Id).WithAlias(() => edrFuelRun.Id)
                .Select(x => x.LevelType).WithAlias(() => edrFuelRun.LevelType)
                .Select(Projections.Cast(NHibernateUtil.Date, Projections.Property<EDRFuelRun>(x => x.OpenDt))).WithAlias(() => edrFuelRun.OpenDt)
                .Select(Projections.Cast(NHibernateUtil.Time, Projections.Property<EDRFuelRun>(x => x.CloseDt))).WithAlias(() => edrFuelRun.CloseDt)
                .Select(Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                            NHibernateUtil.Double,
                            Projections.Property(() => edrFuelRun.NQty ),
                            Projections.Property(() => edrFuelRun.BalanceQty))).WithAlias(() => edrFuelRun.BalanceQty)
                ).TransformUsing(Transformers.AliasToBean<EDRFuelRun>()).List();
        }

        public IList<KeyValue<string, string>> GetCities()
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            KeyValue<string, string> keyValue = null;
            return _repositoryCity.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .OrderBy(x => x.CityName).Asc
                .SelectList(list => list
                    .Select(x => x.CityName).WithAlias(() => keyValue.Name)
                    .Select(x => x.Id.CityId).WithAlias(() => keyValue.Id)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();
        }

        public IList<KeyValue<string, string>> GetRegionsByCity(string city)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            KeyValue<string, string> keyValue = null;
            if (string.IsNullOrEmpty(city))
                return _repositoryRegion.QueryOver()
                    .Where(x => x.Regiontype == "T")
                    .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<Site>()
                        .Select(Projections.Distinct(Projections.Property<Site>(x => x.Region.Id)))
                        .Where(x => x.OrgCode == orgCode))// &&
                    .OrderBy(x => x.Regionname).Asc
                    .SelectList(list => list
                        .Select(x => x.Id).WithAlias(() => keyValue.Id)
                        .Select(x => x.Regionname).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                    .List<KeyValue<string, string>>();
            else
            return _repositoryRegion.QueryOver()
                .Where(x => x.Regiontype == "T")
                .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<Site>()
                    .Select(Projections.Distinct(Projections.Property<Site>(x => x.Region.Id)))
                    .Where(x => x.OrgCode == orgCode && x.City.Id.CityId == city))// &&
                    .OrderBy(x => x.Regionname).Asc
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => keyValue.Id)
                    .Select(x => x.Regionname).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();
        }

        public IList<KeyValue<string, string>> GetSitesByRegion(string region)//city
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            KeyValue<string, string> keyValue = null;

            var site = _repositorySite.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .OrderBy(x => x.SiteName).Asc;

            if (!string.IsNullOrEmpty(region)) //city))
                site.And(x => x.Region.Id == region);
                //site.And(x => x.City.Id.CityId == city);

                return site.SelectList(list => list
                    .Select(x => x.Title).WithAlias(() => keyValue.Id)
                    .Select(x => x.SiteName).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();


        }

        public IList<KeyValue<int, string>> GetAssetsBySite(string site)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = Convert.ToInt32(principal.FindFirst("UserOrgCode").Value);
            KeyValue<int, string> keyValue = null;

            SiteResource siteResources = null;
            if (!site.IsNullOrEmpty())
                return _repositoryAsset.QueryOver() //() => asset)
                    .JoinAlias(c => c.SiteResources, () => siteResources, JoinType.LeftOuterJoin)
                    .Where(x => x.OrgCode == (int) orgCode) //&& siteResources.Id.Site_Code == site)

                    .WithSubquery.WhereProperty(x => siteResources.Id.Site_Code).In(QueryOver.Of<Site>()
                        .Select(Projections.Distinct(Projections.Property<Site>(x => x.Id)))
                        .Where(x => x.OrgCode == orgCode.ToString() && x.Title == site.ToString())) // &&

                    .SelectList(list => list
                        .Select(x => x.AssetName).WithAlias(() => keyValue.Name)
                        .Select(() => siteResources.Id.ResourceNo).WithAlias(() => keyValue.Id))
                    .TransformUsing(Transformers.AliasToBean<KeyValue<int, string>>()).List<KeyValue<int, string>>();

            return _repositoryAsset.QueryOver()
                .JoinAlias(c => c.SiteResources, () => siteResources, JoinType.LeftOuterJoin)
                .Where(x => x.OrgCode == (int) orgCode)
                .SelectList(list => list
                    .Select(x => x.AssetName).WithAlias(() => keyValue.Name)
                    .Select(() => siteResources.Id.ResourceNo).WithAlias(() => keyValue.Id))
                .TransformUsing(Transformers.AliasToBean<KeyValue<int, string>>()).List<KeyValue<int, string>>();
        }

        public dynamic GetAssetInformationReport(GridSettings grid, string sidx, string sord, int page, int rows, string regionId, string site, string resource, bool? assetStatus)
        {
            int totalRecords;
            if (string.IsNullOrEmpty(regionId)) regionId = "%";

            Region region = null;
            Asset asset = null;
            DashboardStationaryDetail dashboardStationaryDetail = null;
            EDRFuelRun edrFuelRun = null;
            DashboardStationaryV2Vw dashboardStationaryV2Vw = null;
            SiteResource siteResource = null;

            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var orgCode = principal.FindFirst("UserOrgCode").Value;
            var userCode = principal.FindFirst("User_Code").Value;

            var dashboardStationaryDetails =
                _repositoryDashboardStationaryV2Vw.QueryOver(() => dashboardStationaryV2Vw);

            dashboardStationaryDetails
                .WhereRestrictionOn(x => x.RegionId).IsLike(regionId)
                .And(x => x.Orgcode == orgCode)
                .WithSubquery.WhereProperty(x => x.RegionId).In(QueryOver.Of<UGroupMstSt>()
                    .Select(Projections.Distinct(Projections.Property<UGroupMstSt>(x => x.RegionId)))
                    .Where(x => x.Orgcode == orgCode)
                    .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<UGroupDtlSt>()
                        .Select(Projections.Distinct(Projections.Property<UGroupDtlSt>(x => x.Id.UgrpCode)))
                        .Where(x => x.Id.UserCode == userCode)));
            if (!string.IsNullOrEmpty(site)) dashboardStationaryDetails.And(x => x.SiteCode == Convert.ToDouble(site));
            if (!string.IsNullOrEmpty(resource)) dashboardStationaryDetails.And(() => siteResource.Id.ResourceNo == Convert.ToInt32(resource) && siteResource.Id.Site_Code == Convert.ToInt32(site));
            if (assetStatus.HasValue)
            {
                dashboardStationaryDetails.And(() => edrFuelRun.EventType == (assetStatus.Value ? 'C' : 'N'));
                dashboardStationaryDetails.And(() => edrFuelRun.LevelType == (assetStatus.Value ? "D" : "N"));
            }

            var stationaryDetails = dashboardStationaryDetails.JoinAlias(c => c.Region, () => region, JoinType.LeftOuterJoin)
                .JoinAlias(c => c.EdrFuelRun, () => edrFuelRun)
                .JoinAlias(c => c.Asset, () => asset)
                .JoinAlias(c => c.Asset.SiteResources, () => siteResource, JoinType.LeftOuterJoin)
                .SelectList(list => list
                    .Select(x => x.SiteName).WithAlias(() => dashboardStationaryDetail.SiteName)
                    .Select(x => x.AssetName).WithAlias(() => dashboardStationaryDetail.AssetName)
                    .Select(() => region.Regionname).WithAlias(() => dashboardStationaryDetail.RegionName)
                    .Select(x => x.Capacity).WithAlias(() => dashboardStationaryDetail.Capacity)
                    .Select(x => x.BaseVolume).WithAlias(() => dashboardStationaryDetail.BaseVolume)
                    .Select(x => x.MinLevel).WithAlias(() => dashboardStationaryDetail.MinLevel)
                    .Select(x => x.Currents).WithAlias(() => dashboardStationaryDetail.Currents)
                    .Select(x => x.LevelType).WithAlias(() => dashboardStationaryDetail.LevelType)
                    .Select(x => x.Totduration).WithAlias(() => dashboardStationaryDetail.TotDuration)
                    .Select(() => asset.Dgcap).WithAlias(() => dashboardStationaryDetail.Dgcap)
                    .Select(() => siteResource.ActivationDt).WithAlias(() => dashboardStationaryDetail.ActivationDt)
                    .Select(Projections.SqlFunction("FN_GetEventType", NHibernateUtil.String,
                        Projections.Property(() => edrFuelRun.EventType),
                        Projections.Property<DashboardStationaryV2Vw>(p => p.Resourcetype),
                        Projections.Property<DashboardStationaryV2Vw>(p => p.Orgcode)))
                    .WithAlias(() => dashboardStationaryDetail.EventName)
                )
                .Where(() => edrFuelRun.CloseDt == null)
                .TransformUsing(Transformers.AliasToBean<DashboardStationaryDetail>()) 
                .List<DashboardStationaryDetail>().AsQueryable().SearchGrid(grid, out totalRecords);


            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from props in stationaryDetails
                        select new
                        {
                            props.SiteName,
                            props.AssetName,
                            props.RegionName,
                            props.Capacity,
                            props.BaseVolume,
                            props.MinLevel,
                            props.Currents,
                            props.LevelType,
                            props.TotDuration,
                            props.Dgcap,
                            props.ActivationDt,
                            props.EventName
                        })
            };
        }
    }

    /*
     * exec SP_Apmleview_Stationery @UnitID='11051458'
exec SP_Get_TodayStatistics @UnitID='11051458',@OpenDt='2016-03-30 00:00:00',@FuelRate=71.120000000000005
exec SP_GET_MONTHSUM_DASHBOARD @UnitID='11051458',@Year='2016',@month='03'

exec SP_GETAll_Site_N_AssetInfo '1823', 81

sp_helptext SP_GETAll_Site_N_AssetInfo
     * Select *from Rspt_dashboardStationary_vw
     */
}