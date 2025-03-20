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

namespace Roottech.Tracking.WebApi.Controllers
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
        public IQueryable<dynamic> GetTodaysMap(string unitId, string ignition, string mapDate, string fromTime, string toTime)
        {
            EDRFuel edrFuel = null;

            var sql = Expression.Sql("RTCDTTM Between convert(datetime, '" + mapDate + " " + fromTime + "') " +
                "and dateadd(s,-1,dateadd(mi,1, convert(datetime, '" + mapDate + " " + toTime + "')))");
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
    }
}
