using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect.Function;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSGI.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.Models.Helpers;
using Roottech.Tracking.Library.PdfReports;
using Roottech.Tracking.Library.Utils;
using Roottech.Tracking.RTM.UI.Web.Infrastructure;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Mobile.Controllers
{
    [Authorize]
    public class ReportApiController : ApiController
    {
        private readonly IKeyedRepository<double, EDRFuel> _repositoryEdrFuel;
        private readonly ISession _session;

        public ReportApiController(IKeyedRepository<double, EDRFuel> repositoryEdrFuel, 
            ISessionFactory sessionFactory)
        {
            _repositoryEdrFuel = repositoryEdrFuel;
            _session = sessionFactory.GetCurrentSession();
        }

        [HttpGet]
        public dynamic GetTodaysReport(GridSettings grid, string sidx, string sord, int page, int rows, int assetNo)
        {
            EDRFuel edrFuel = null;
            CompleteUnitVw completeUnitVw = null;
            int totalRecords;
            //var expressions = new List<Expression<Func<EdrFuel, object>>> { x => x.Id, x => x.RTCDTTM, x => x.DiDesc, x => x.Speed };
            var sql = Expression.Sql("DATEDIFF(day, RTCDTTM, GetDate()) = 0");
            var edrFuels = _repositoryEdrFuel.QueryOver(() => edrFuel)
                .JoinAlias(x => x.CompleteUnitVw, () => completeUnitVw)
                .Where(sql).And(() => completeUnitVw.AssetNo == assetNo)
                .SelectList(list => list
                                        .Select(x => x.RTCDTTM).WithAlias(() => edrFuel.RTCDTTM)
                                        //.Select(x => x.DiDesc).WithAlias(() => edrFuel.DiDesc)
                                        .Select(x => x.DI1).WithAlias(() => edrFuel.DI1)
                                        .Select(x => x.DI2).WithAlias(() => edrFuel.DI2)
                                        .Select(x => x.DI3).WithAlias(() => edrFuel.DI3)
                                        .Select(x => x.DI4).WithAlias(() => edrFuel.DI4)
                                        .Select(x => x.DI5).WithAlias(() => edrFuel.DI5)
                                        .Select(x => x.DI6).WithAlias(() => edrFuel.DI6)
                                        .Select(Projections.SqlFunction("Round", NHibernateUtil.Double, Projections.SqlFunction(new VarArgsSQLFunction("(", "*", ")"),
                                        NHibernateUtil.Double,
                                        Projections.Property(() => edrFuel.Speed),
                                        Projections.Constant(1.609344)), // convert miles to kilometer per hour
                                        Projections.Constant(1))).WithAlias(()=> edrFuel.Speed)
                                        .Select(x => x.Mileage).WithAlias(() => edrFuel.Mileage)
                                        .Select(x => x.Qty).WithAlias(() => edrFuel.Qty)
                                        .Select(x => x.Qty1).WithAlias(() => edrFuel.Qty1)
                ).TransformUsing(Transformers.AliasToBean<EDRFuel>()).SearchGrid(grid, out totalRecords);

            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = edrFuels
            };
        }

        [Queryable]
        [HttpGet]
        public IQueryable<dynamic> GetTodaysMap(string unitId, string ignition, string fromDate, string toDate, string fromTime, string toTime)
        {
            EDRFuel edrFuel = null;

            var sql = Expression.Sql("RTCDTTM Between convert(datetime, '" + fromDate + " " + fromTime + "') " +
                "and dateadd(s,-1,dateadd(mi,1, convert(datetime, '" + toDate + " " + toTime + "')))");
                //Expression.Sql("DATEDIFF(day, RTCDTTM, '" + mapDate + "') = 0");

            IQueryOver<EDRFuel, EDRFuel> edrFuels = _repositoryEdrFuel.QueryOver(() => edrFuel)
                .Where(x => x.UnitID == unitId).And(sql)
                .And(
                    Restrictions.Not(
                        Restrictions.Eq(
                            Projections.Cast(NHibernateUtil.Double, Projections.Property<EDRFuel>(x => x.Lati)), 0)));
            //.And(Restrictions.Not(Restrictions.Eq(Projections.Property<EDRFuel>(x => x.Longi), "0.0000000000")));//x.DI1 == "1");
            if (ignition != "A") edrFuels.And(() => edrFuel.DI1 == ignition);
            return edrFuels.SelectList(list => list
                                                   .Select(x => x.RTCDTTM).WithAlias(() => edrFuel.RTCDTTM)
                                                   .Select(Projections.SqlFunction("Round", NHibernateUtil.Double, Projections.SqlFunction(new VarArgsSQLFunction("(", "*", ")"),
                                                        NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuel.Speed),
                                                        Projections.Constant(1.609344)), // convert miles to kilometer per hour
                                                        Projections.Constant(1))).WithAlias(() => edrFuel.Speed)
                                                   .Select(x => x.Angle).WithAlias(() => edrFuel.Angle)
                                                   .Select(x => x.Longi).WithAlias(() => edrFuel.Longi)
                                                   .Select(x => x.Lati).WithAlias(() => edrFuel.Lati)
                                                   .Select(x => x.DI1).WithAlias(() => edrFuel.DI1)
                                                   .Select(x => x.GSMSignals).WithAlias(() => edrFuel.GSMSignals)
                ).OrderBy(x => x.RTCDTTM).Asc
                //.TransformUsing(Transformers.AliasToBean<EDRFuel>())
                .List<object[]>()
                .Select(props =>
                        new
                            {
                                RTCDTTM = DateTime.Parse(props[0].ToString()).ToString(),
                                Speed = props[1],//Math.Round(Convert.ToDouble(props[1]) * 1.609344, 1),
                                Angle = props[2],
                                Longi = props[3],
                                Lati = props[4],
                                DI1 = props[5],
                                GSMSignals = props[6]
                            })
                .AsQueryable();
        }

        [Queryable]
        [HttpGet]
        public IQueryable<dynamic> GetTodaysTrack(string unitId, string mapDate, string fromTime, string toTime)
        {
            EDRFuel edrFuel = null;
            EDRFuelRun edrFuelRun = null;

            var sql = Expression.Sql("RTCDTTM Between convert(datetime, '" + mapDate + " " + fromTime + "') " +
                "and dateadd(s,-1,dateadd(mi,1, convert(datetime, '" + mapDate + " " + toTime + "')))");

            IQueryOver<EDRFuel, EDRFuelRun> edrFuels = _repositoryEdrFuel.QueryOver(() => edrFuel)
                .Where(x => x.UnitID == unitId).And(sql)
                .And(
                    Restrictions.Not(
                        Restrictions.Eq(
                            Projections.Cast(NHibernateUtil.Double, Projections.Property<EDRFuel>(x => x.Lati)), 0)))
                .JoinQueryOver(r => r.EdrFuelRun, () => edrFuelRun)
                .AndNot(() => edrFuelRun.EventType == 'K');// According to Habib bhai its not needed in today's full map
                //.TransformUsing(Transformers.ToList);
            return edrFuels.SelectList(list => list
                                                   .Select(x => x.RTCDTTM).WithAlias(() => edrFuel.RTCDTTM)
                                                   .Select(Projections.SqlFunction("Round", NHibernateUtil.Double, Projections.SqlFunction(new VarArgsSQLFunction("(", "*", ")"),
                                                        NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuel.Speed),
                                                        Projections.Constant(1.609344)), // convert miles to kilometer per hour
                                                        Projections.Constant(1)))
                                                   .Select(x => x.Angle)
                                                   .Select(x => x.Longi)
                                                   .Select(x => x.Lati)
                                                   .Select(x => x.DI1)
                                                   .Select(x => x.GSMSignals)
                                                   .Select(() => edrFuelRun.EventType)
                                                   .Select(() => edrFuelRun.OpenDt)
                                                   .Select(() => edrFuelRun.FRTCDTTM)
                                                   .Select(() => edrFuelRun.TotDuration)
                                                   .Select(
                                                        Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                                                        NHibernateUtil.Double,
                                                        Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuelRun.BalanceQty), Projections.Constant(0)),
                                                        Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuelRun.NQty), Projections.Constant(0)))
                                                   ) //balanceqty - n_qty as netqty
                ).OrderBy(() => edrFuel.RTCDTTM).Asc
                .List<object[]>()
                .Select(props =>
                        new
                        {
                            RTCDTTM = DateTime.Parse(props[0].ToString()).ToString(),
                            Speed = props[1],//Math.Round(Convert.ToDouble(props[1]) * 1.609344, 1),
                            Angle = props[2],
                            Longi = props[3],
                            Lati = props[4],
                            DI1 = props[5],
                            GSMSignals = props[6],
                            EventType = props[7],
                            OpenDt = DateTime.Parse(props[8].ToString()).ToString(),
                            FRTCDTTM = DateTime.Parse(props[9].ToString()).ToString(),
                            TotDuration = props[10],
                            NetQty = props[11]
                        })
                .AsQueryable();
        }
       
        [Queryable]
        [HttpGet]
        public IQueryable<dynamic> GetEventsExceptDrive(string unitId, string mapDate, string fromTime, string toTime)
        {
            EDRFuel edrFuel = null;
            EDRFuelRun edrFuelRun = null;

            var sql = Expression.Sql("RTCDTTM Between convert(datetime, '" + mapDate + " " + fromTime + "') " +
                "and dateadd(s,-1,dateadd(mi,1, convert(datetime, '" + mapDate + " " + toTime + "')))");

            IQueryOver<EDRFuel, EDRFuelRun> edrFuels = _repositoryEdrFuel.QueryOver(() => edrFuel)
                .WhereRestrictionOn(() => edrFuelRun.EventType).IsIn(new List<string> { "N", "T", "R"})
                .And(x  => x.RTCDTTM == edrFuelRun.OpenDt)
                //.And(Restrictions.Eq(Projections.SqlFunction(new SQLFunctionTemplate(NHibernateUtil.DateTime,"DateDiff(day, ?1, getdate()-1)"),
                //NHibernateUtil.Int16,Projections.Property(() =>edrFuelRun.OpenDt)),0))
                .And(x => x.UnitID == unitId).And(x => x.UnitID == edrFuelRun.UnitID).And(sql)
                .And(Restrictions.Not(
                        Restrictions.Eq(
                            Projections.Cast(NHibernateUtil.Double, Projections.Property<EDRFuel>(x => x.Lati)), 0)))
                .JoinQueryOver(r => r.EdrFuelRun, () => edrFuelRun);
            //case  edrfuelrun1_.eventType when 'T' then edrfuelrun1_.BalanceQty-edrfuelrun1_.n_qty when 'R' then edrfuelrun1_.n_qty - edrfuelrun1_.BalanceQty else edrfuelrun1_.BalanceQty end
            return edrFuels.SelectList(list => list
                                                   .Select(x => x.RTCDTTM).WithAlias(() => edrFuel.RTCDTTM)
                                                   .Select(Projections.SqlFunction("Round", NHibernateUtil.Double, Projections.SqlFunction(new VarArgsSQLFunction("(", "*", ")"),
                                                        NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuel.Speed),
                                                        Projections.Constant(1.609344)), // convert miles to kilometer per hour
                                                        Projections.Constant(1)))
                                                   .Select(x => x.Angle)
                                                   .Select(x => x.Longi)
                                                   .Select(x => x.Lati)
                                                   .Select(x => x.DI1)
                                                   .Select(x => x.GSMSignals)
                                                   .Select(() => edrFuelRun.EventType)
                                                   .Select(() => edrFuelRun.OpenDt)
                                                   .Select(() => edrFuelRun.FRTCDTTM)
                                                   .Select(() => edrFuelRun.TotDuration)
                                                   .Select(Projections.Conditional(
                                                    Restrictions.Where(() => edrFuelRun.EventType == 'T'),
                                                        Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                                                        NHibernateUtil.Double,
                                                        Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuelRun.BalanceQty), Projections.Constant(0)),
                                                        Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                        Projections.Property(() => edrFuelRun.NQty), Projections.Constant(0))),
                                                        
                                                            Projections.Conditional(
                                                                Restrictions.Where(() => edrFuelRun.EventType == 'R'),
                                                                    Projections.SqlFunction(new VarArgsSQLFunction("(", "-", ")"),
                                                                    NHibernateUtil.Double,
                                                                    Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                                    Projections.Property(() => edrFuelRun.NQty), Projections.Constant(0)),
                                                                    Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                                    Projections.Property(() => edrFuelRun.BalanceQty), Projections.Constant(0))),
                                                                    
                                                                        Projections.SqlFunction("Coalesce", NHibernateUtil.Double,
                                                                        Projections.Property(() => edrFuelRun.NQty), Projections.Constant(0)))
                                                    ))
                                                    .SelectSubQuery(QueryOver.Of<Poi>().Select(tx => tx.PoiName).Where(tw => tw.Id == edrFuelRun.PoiNo))
                ).OrderBy(() => edrFuelRun.OpenDt).Asc
                .List<object[]>()
                .Select(props =>
                        new
                        {
                            RTCDTTM = DateTime.Parse(props[0].ToString()).ToString(),
                            Speed = props[1],//Math.Round(Convert.ToDouble(props[1]) * 1.609344, 1),
                            Angle = props[2],
                            Longi = props[3],
                            Lati = props[4],
                            DI1 = props[5],
                            GSMSignals = props[6],
                            EventType = props[7],
                            OpenDt = DateTime.Parse(props[8].ToString()).ToString(),
                            FRTCDTTM = DateTime.Parse(props[9].ToString()).ToString(),
                            TotDuration = props[10],
                            NetQty = props[11],
                            PoiName = props[12]
                        })
                .AsQueryable();
        }

        //[Queryable][HttpGet]
        public EDRFuel GetFirstCDRByDateAndFromTimeAndToTime(int assetNo, string mapDate, string fromTime, string toTime)
        {
            EDRFuel edrFuel = null;

            var sql = Expression.Sql("RTCDTTM Between convert(datetime, '" + mapDate + " " + fromTime + "') " +
                "and dateadd(s,-1,dateadd(mi,1, convert(datetime, '" + mapDate + " " + toTime + "')))");

            IQueryOver<EDRFuel, EDRFuel> edrFuels = _repositoryEdrFuel.QueryOver(() => edrFuel)
                .WithSubquery.WhereProperty(x => x.UnitID)
                .In(QueryOver.Of<PackageMst>().WithSubquery.WhereProperty(y => y.Id)
                        .In(QueryOver.Of<SiteResource>().Where(z => z.AssetNo == assetNo)
                                .Select(Projections.Distinct(Projections.Property<SiteResource>(p => p.PKG_Code))))
                        .Select(Projections.Distinct(Projections.Property<PackageMst>(p => p.UnitID)))).And(sql)
                .And(
                    Restrictions.Not(
                        Restrictions.Eq(
                            Projections.Cast(NHibernateUtil.Double, Projections.Property<EDRFuel>(x => x.Lati)), 0)));

            return edrFuels.SelectList(list => list
                .Select(x => x.RTCDTTM).WithAlias(() => edrFuel.RTCDTTM)
                .Select(Projections.SqlFunction("Round", NHibernateUtil.Double,
                    Projections.SqlFunction(new VarArgsSQLFunction("(", "*", ")"),
                        NHibernateUtil.Double,
                        Projections.Property(() => edrFuel.Speed),
                        Projections.Constant(1.609344)), // convert miles to kilometer per hour
                    Projections.Constant(1))).WithAlias(() => edrFuel.Speed)
                .Select(x => x.Angle).WithAlias(() => edrFuel.Angle)
                .Select(x => x.Longi).WithAlias(() => edrFuel.Longi)
                .Select(x => x.Lati).WithAlias(() => edrFuel.Lati)
                .Select(x => x.DI1).WithAlias(() => edrFuel.DI1)
                .Select(x => x.GSMSignals).WithAlias(() => edrFuel.GSMSignals)
                ).OrderBy(x => x.RTCDTTM).Asc
                .TransformUsing(Transformers.AliasToBean<EDRFuel>()).Take(1)
                .SingleOrDefault<EDRFuel>();
            //
            /*.List<object[]>()
                .Select(props =>
                        new
                        {
                            RTCDTTM = DateTime.Parse(props[0].ToString()).ToString(),
                            Speed = props[1],//Math.Round(Convert.ToDouble(props[1]) * 1.609344, 1),
                            Angle = props[2],
                            Longi = props[3],
                            Lati = props[4],
                            DI1 = props[5],
                            GSMSignals = props[6]
                        })
                .AsQueryable();*/

        }

        [Queryable][HttpGet]
        public IQueryable<VehicleReport> GetVehicleReportByUnitIdAndDate(string unitId, DateTime date)
        {
            return _session.GetNamedQuery("GetVehicleReport")
                .SetString("UnitID", unitId)
                .SetDateTime("Date", date)
                .List<VehicleReport>().AsQueryable();
        }

        [Queryable]
        [HttpGet]
        public IQueryable<SpeedVehicleReport> GetSpeedReportByUnitIdAndDate(string unitId, DateTime date)
        {
            return _session.GetNamedQuery("GetSpeedVehicleReport")
                .SetString("UnitID", unitId)
                .SetDateTime("Date", date)
                .List<SpeedVehicleReport>().AsQueryable();
        }

        [Queryable]
        [HttpGet]
        public IQueryable<StartStopReport> GetStartStopReportByUnitIdAndDate(string unitId, DateTime Fromdate, DateTime Todate)
        {
            return _session.GetNamedQuery("GetStartStopReport")
                .SetString("UnitID", unitId)
                .SetDateTime("FromDate", Fromdate)
                .SetDateTime("ToDate", Todate)
                .List<StartStopReport>().AsQueryable();
        }

        //[Queryable][HttpGet]
        public dynamic GetVehicleRefuelDetailReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            var rptGenRefuels = GetRptVehicleRefuelDetail(fromDate, toDate, resource, ugrpmst);

            var totalRecords = rptGenRefuels.Count();
            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = rptGenRefuels
            };
        }

        public void GetVehicleRefuelDetailReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            var rptGenRefuels = GetRptVehicleRefuelDetail(fromDate, toDate, resource, ugrpmst);
            string[,] colNamesToReplace = new string[9, 2];

            colNamesToReplace[0, 0] = "Description";
            colNamesToReplace[0, 1] = "Plate Id";

            colNamesToReplace[1, 0] = "Location";
            colNamesToReplace[1, 1] = "Site Location";

            colNamesToReplace[2, 0] = "Id";
            colNamesToReplace[2, 1] = "Batch#";

            colNamesToReplace[3, 0] = "EventType";
            colNamesToReplace[3, 1] = "Event Type";

            colNamesToReplace[4, 0] = "OpenDate";
            colNamesToReplace[4, 1] = "Event Start";

            colNamesToReplace[5, 0] = "CloseDate";
            colNamesToReplace[5, 1] = "Event End";

            colNamesToReplace[6, 0] = "LevelType";
            colNamesToReplace[6, 1] = "Ignition";

            colNamesToReplace[7, 0] = "TotDuration";
            colNamesToReplace[7, 1] = "Total Duration";

            colNamesToReplace[8, 0] = "EndQty";
            colNamesToReplace[8, 1] = "Refueled(Ltrs)";

            ExportFilesByHttpResponse.ExportToExcel(rptGenRefuels.AsQueryable(),
                row => new
                {
                    row.Description,
                    row.Location,
                    row.Id,
                    row.EventType,
                    row.OpenDate,
                    row.CloseDate,
                    row.LevelType,
                    row.TotDuration,
                    row.EndQty
                }, null, "Refuel Details Report", colNamesToReplace);
        }

        //public void GetFuelIntakeReportEp(GridSettings grid, DateTime fromDate, DateTime toDate, string region, string city, string site, string resource, string ugrpmst)
        //{
        //    var rptGenRefuels = GetRptVehicleRefuelDetail(fromDate, toDate, resource, ugrpmst);
        //    new ProductsPdfReport().CreatePdfReport(rptGenRefuels);
        //}
        private IList<RptVehicleRefuelDetail> GetRptVehicleRefuelDetail(DateTime fromDate, DateTime toDate, string resource, string ugrpmst)//, string orgCode, string userCode)
        {
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(ugrpmst)) ugrpmst = "%";

            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            var userCode = SystemLibrary.GetCookies("User_Code");

            return _session.GetNamedQuery("GetRptVehicleRefuelDetail")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Orgcode", orgCode)
                .SetString("ResourceNo", resource)
                .SetString("UserCode", userCode)
                .SetString("ugrpmst", ugrpmst)
                .SetTimeout(0)
                .List<RptVehicleRefuelDetail>();
        }

        public dynamic GetVehicleTheftDetailReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            var rptGenRefuels = GetRptVehicleTheftDetail(fromDate, toDate, resource, ugrpmst);

            var totalRecords = rptGenRefuels.Count();
            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = rptGenRefuels
            };
        }
        public void GetVehicleTheftDetailReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            var rptGenRefuels = GetRptVehicleTheftDetail(fromDate, toDate, resource, ugrpmst);
            string[,] colNamesToReplace = new string[8, 2];

            colNamesToReplace[0, 0] = "Description";
            colNamesToReplace[0, 1] = "Plate Id";

            colNamesToReplace[1, 0] = "Id";
            colNamesToReplace[1, 1] = "Batch#";

            colNamesToReplace[2, 0] = "OpenDate";
            colNamesToReplace[2, 1] = "Event Start";

            colNamesToReplace[3, 0] = "CloseDate";
            colNamesToReplace[3, 1] = "Event End";

            colNamesToReplace[4, 0] = "LevelType";
            colNamesToReplace[4, 1] = "Ignition";

            colNamesToReplace[5, 0] = "TotDuration";
            colNamesToReplace[5, 1] = "Total Duration";

            colNamesToReplace[6, 0] = "FuelTheftQty";
            colNamesToReplace[6, 1] = "Fuel Theft(Ltrs)";

            ExportFilesByHttpResponse.ExportToExcel(rptGenRefuels.AsQueryable(),
                row => new
                {
                    row.Description,
                    row.Id,
                    row.OpenDate,
                    row.CloseDate,
                    row.LevelType,
                    row.TotDuration,
                    row.FuelTheftQty
                }, null, "Fuel Theft Details Report", colNamesToReplace);
        }
        private IList<RptVehicleTheftDetail> GetRptVehicleTheftDetail(DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(ugrpmst)) ugrpmst = "%";

            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            var userCode = SystemLibrary.GetCookies("User_Code");

            return _session.GetNamedQuery("GetRptVehicleTheftDetail")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Orgcode", orgCode)
                .SetString("PlateId", resource)
                .SetString("UserCode", userCode)
                .SetString("ugrpmst", ugrpmst)
                .SetTimeout(0)
                .List<RptVehicleTheftDetail>();
        }
        public dynamic GetRptDayWiseSummaryReport(GridSettings grid, string sidx, string sord, int page, int rows, DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            var rptGenRefuels = GetRptDayWiseSummary(fromDate, toDate, resource, ugrpmst);

            var totalRecords = rptGenRefuels.Count();
            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = rptGenRefuels
            };
        }
        public void GetRptDayWiseSummaryReportEe(GridSettings grid, DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            var rptGenRefuels = GetRptDayWiseSummary(fromDate, toDate, resource, ugrpmst);
            string[,] colNamesToReplace = new string[15, 2];

            colNamesToReplace[0, 0] = "PlateID";
            colNamesToReplace[0, 1] = "Plate Id";

            colNamesToReplace[1, 0] = "Date";
            colNamesToReplace[1, 1] = "Date";

            colNamesToReplace[2, 0] = "StartPoint";
            colNamesToReplace[2, 1] = "Start Point";

            colNamesToReplace[3, 0] = "EndPoint";
            colNamesToReplace[3, 1] = "End Point";

            colNamesToReplace[4, 0] = "Opening";
            colNamesToReplace[4, 1] = "Opening";

            colNamesToReplace[5, 0] = "Consume";
            colNamesToReplace[5, 1] = "Consume";

            colNamesToReplace[6, 0] = "Refuel";
            colNamesToReplace[6, 1] = "Refuel";

            colNamesToReplace[7, 0] = "Theft";
            colNamesToReplace[7, 1] = "Theft";

            colNamesToReplace[8, 0] = "Closing";
            colNamesToReplace[8, 1] = "EDR Closing";

            colNamesToReplace[9, 0] = "RunningHour";
            colNamesToReplace[9, 1] = "Running Hours";

            colNamesToReplace[10, 0] = "Idle";
            colNamesToReplace[10, 1] = "Idle Hours";

            colNamesToReplace[11, 0] = "Parking";
            colNamesToReplace[11, 1] = "Parking Hours";

            colNamesToReplace[12, 0] = "Kms";
            colNamesToReplace[12, 1] = "Kmtrs";

            colNamesToReplace[13, 0] = "AvgLtrhr";
            colNamesToReplace[13, 1] = "Avg Ltr/hr";

            colNamesToReplace[14, 0] = "TotMile";
            colNamesToReplace[14, 1] = "Avg Km/Ltr";

            ExportFilesByHttpResponse.ExportToExcel(rptGenRefuels.AsQueryable(),
                row => new
                {
                    row.PlateID,
                    row.Date,
                    row.StartPoint,
                    row.EndPoint,
                    row.Opening,
                    row.Consume,
                    row.Refuel,
                    row.Theft,
                    row.Closing,
                    row.RunningHour,
                    row.Idle,
                    row.Parking,
                    row.Kms,
                    row.AvgLtrhr,
                    row.TotMile
                }, null, "Day Wise Summary Report", colNamesToReplace);
        }
        private IList<RptDayWiseSummary> GetRptDayWiseSummary(DateTime fromDate, DateTime toDate, string resource, string ugrpmst)
        {
            if (string.IsNullOrEmpty(resource)) resource = "%";
            if (string.IsNullOrEmpty(ugrpmst)) ugrpmst = "%";

            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            var userCode = SystemLibrary.GetCookies("User_Code");

            return _session.GetNamedQuery("GetRptDayWiseSummary")
                .SetDateTime("FromDt", fromDate).SetDateTime("ToDt", toDate)
                .SetString("Orgcode", orgCode)
                .SetString("PlateID", resource)
                .SetString("UserCode", userCode)
                .SetString("ugrpmst", ugrpmst)
                .SetTimeout(0)
                .List<RptDayWiseSummary>();
        }
        //
    }
}
