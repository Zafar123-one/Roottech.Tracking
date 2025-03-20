using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHibernate;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSGI.Entities;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.Models.Helpers;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Mobile.Controllers
{
    [Authorize]
    public class GeoFenceAdminApiController : ApiController
    {
        private readonly IKeyedRepository<int, GFMst> _repositoryGfMst;
        private readonly IKeyedRepository<int, GFDtl> _repositoryGfDtl;
        private readonly IKeyedRepository<GFPOIIdentifier, GFPOI> _repositoryGfpoi;
        private readonly ISession _session;
        private readonly IKeyedRepository<int, RouteMst> _repositoryRouteMst;
        private readonly IKeyedRepository<RouteDtlIdentifier, RouteDtl> _repositoryRouteDtl;

        public GeoFenceAdminApiController(ISessionFactory sessionFactory, IKeyedRepository<int, GFMst> repositoryGfMst,
            IKeyedRepository<int, GFDtl> repositoryGfDtl, IKeyedRepository<GFPOIIdentifier, GFPOI> repositoryGfpoi, IKeyedRepository<int, RouteMst> repositoryRouteMst,
            IKeyedRepository<RouteDtlIdentifier, RouteDtl> repositoryRouteDtl)
        {
            _session = sessionFactory.GetCurrentSession();
            _repositoryGfMst = repositoryGfMst;
            _repositoryGfDtl = repositoryGfDtl;
            _repositoryGfpoi = repositoryGfpoi;
            _repositoryRouteMst = repositoryRouteMst;
            _repositoryRouteDtl = repositoryRouteDtl;
        }

        public IList<GeoFence> GetGeofense(int orgCode, string companyCode, string type)
        {
            return _session.GetNamedQuery("GetGeoFence")
                .SetString("orgCode", orgCode.ToString())
                .SetString("companyCode", companyCode)
                .SetString("Type", type)
                .List<GeoFence>();
        }

        public IList<GeoFence> GetGeofense(int orgCode, string companyCode, int geofenceNo)
        {
            GeoFence geoFence = null;
            GFMst gfMst = null;
            return _repositoryGfDtl.QueryOver()
                .JoinAlias(c => c.GFMst, () => gfMst)
                .Where(() => gfMst.OrgCode == orgCode && gfMst.Company_Code == companyCode && gfMst.Id == geofenceNo)
                .SelectList(list => list
                .Select(x => x.Id).WithAlias(() => geoFence.Id)
                .Select(x => x.GFMstId).WithAlias(() => geoFence.GeofenceNo)
                .Select(x => gfMst.OrgCode).WithAlias(() => geoFence.OrgCode)
                .Select(x => gfMst.Company_Code).WithAlias(() => geoFence.Company_code)
                .Select(x => gfMst.GFName).WithAlias(() => geoFence.GFName)
                .Select(x => gfMst.GFTitle).WithAlias(() => geoFence.GFTitle)
                .Select(x => gfMst.GFType).WithAlias(() => geoFence.GFType)
                .Select(x => gfMst.Comment).WithAlias(() => geoFence.Comment)
                .Select(x => gfMst.GFMargin).WithAlias(() => geoFence.GFMargin)
                .Select(x => x.CLati).WithAlias(() => geoFence.CLati)
                .Select(x => x.CLongi).WithAlias(() => geoFence.CLongi)
                .Select(x => x.CRadius).WithAlias(() => geoFence.CRadius)
                .Select(x => x.PLati).WithAlias(() => geoFence.PLati)
                .Select(x => x.PLongi).WithAlias(() => geoFence.PLongi)
                .Select(x => x.PSeq).WithAlias(() => geoFence.PSeq)
                .Select(x => x.SLeftTopLati).WithAlias(() => geoFence.SLeftTopLati)
                .Select(x => x.SLeftTopLongi).WithAlias(() => geoFence.SLeftTopLongi)
                .Select(x => x.SRightBotLati).WithAlias(() => geoFence.SRightBotLati)
                .Select(x => x.SRightBotLongi).WithAlias(() => geoFence.SRightBotLongi))
                .TransformUsing(Transformers.AliasToBean<GeoFence>())
                .List<GeoFence>();
        }
        
        public dynamic GetGeofenseMst(GridSettings grid, string sidx, string sord, int page, int rows, int orgCode, string companyCode)
        {
            GFMst gfMst = null;
            int totalRecords;

            var queryOverGfMst = _repositoryGfMst.QueryOver().Where(x => x.OrgCode == orgCode && x.Company_Code == companyCode);

            var gfMsts = queryOverGfMst.SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => gfMst.Id)
                    .Select(x => x.GFName).WithAlias(() => gfMst.GFName)
                    .Select(x => x.GFTitle).WithAlias(() => gfMst.GFTitle)
                    .Select(x => x.GFType).WithAlias(() => gfMst.GFType)
                    .Select(x => x.GFMargin).WithAlias(() => gfMst.GFMargin)
                    .Select(x => x.Comment).WithAlias(() => gfMst.Comment)
                ).TransformUsing(Transformers.AliasToBean<GFMst>()).SearchGrid(grid, out totalRecords);


            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from props in gfMsts
                        select new
                        {
                            props.Id,
                            props.GFName,
                            props.GFTitle,
                            props.GFType,
                            props.GFMargin,
                            props.Comment
                        })
            };
                
        }

        [HttpPost]
        public int AddGeofenceMst(string companyCode, string gfType, string gfTitle, string gfName, string gfMargin, string comment, int orgCode)
        {
            var sUserCode = SystemLibrary.GetCookies("User_Code");
            var gfMst = new GFMst
            {
                Company_Code = companyCode == "" ? null : companyCode,
                GFType = gfType,
                GFTitle = gfTitle,
                GFName = gfName,
                GFMargin = gfMargin,
                Comment = comment,
                OrgCode = orgCode,
                User_Code = (sUserCode != "%") ? sUserCode : "",
                DML_Type = "I",
                DML_Date = DateTime.Now
            };
            _repositoryGfMst.Add(gfMst);
            return gfMst.Id;
        }

        public void AddGeofencedtl(int gfMst, double cRadius, float cLati, float cLongi, float pLati, float pLongi, double pSeq,
            float sLeftTopLati, float sLeftTopLongi, float sRightTopLati, float sRightTopLongi)
        {
            var sUserCode = SystemLibrary.GetCookies("User_Code");
            var gfDtl = new GFDtl
            {
                GFMstId = gfMst,
                CRadius = cRadius,
                CLongi = cLongi,
                CLati = cLati,
                PLati = pLati,
                PLongi = pLongi,
                PSeq = pSeq,
                SLeftTopLati = sLeftTopLati,
                SLeftTopLongi = sLeftTopLongi,
                SRightBotLati = sRightTopLati,
                SRightBotLongi = sRightTopLongi,
                DML_Type = "I",
                DML_Date = DateTime.Now,
                User_Code = (sUserCode != "%") ? sUserCode : ""

            };
            _repositoryGfDtl.Add(gfDtl);
        }

        [HttpPost]
        public bool AddPolyGeofencedtl(IList<PolyGeoFence> polyGeoFences, string companyCode, string gfType,
            string gfTitle, string gfName, string gfMargin, string comment, int orgCode, double cRadius)
        {
            if (polyGeoFences == null) return false;

            var sUserCode = SystemLibrary.GetCookies("User_Code");
            var gfMst = new GFMst
            {
                Company_Code = companyCode == "" ? null : companyCode,
                GFType = gfType,
                GFTitle = gfTitle,
                GFName = gfName,
                GFMargin = gfMargin,
                Comment = comment,
                OrgCode = orgCode,
                User_Code = (sUserCode != "%") ? sUserCode : "",
                DML_Type = "I",
                DML_Date = DateTime.Now
            };
            _repositoryGfMst.Add(gfMst);

            if (gfType == "C")
            {
                var gfDtl = new GFDtl
                {
                    GFMstId = gfMst.Id,
                    CLati = polyGeoFences[0].Lati,
                    CLongi = polyGeoFences[0].Longi,
                    CRadius = cRadius,
                    DML_Type = "I",
                    DML_Date = DateTime.Now,
                    User_Code = (sUserCode != "%") ? sUserCode : ""
                };
                _repositoryGfDtl.Add(gfDtl);
            }
            else if (gfType == "R")
            {
                var gfDtl = new GFDtl
                {
                    GFMstId = gfMst.Id,
                    SLeftTopLati = polyGeoFences[0].Lati,
                    SLeftTopLongi = polyGeoFences[0].Longi,
                    SRightBotLati = polyGeoFences[1].Lati,
                    SRightBotLongi = polyGeoFences[1].Longi,
                    DML_Type = "I",
                    DML_Date = DateTime.Now,
                    User_Code = (sUserCode != "%") ? sUserCode : ""
                };
                _repositoryGfDtl.Add(gfDtl);
            }
            else if (gfType == "P" || gfType == "L")
                foreach (var polyGeoFence in polyGeoFences)
                {
                    var gfDtl = new GFDtl
                    {
                        GFMstId = gfMst.Id,
                        PLati = polyGeoFence.Lati,
                        PLongi = polyGeoFence.Longi,
                        PSeq = polyGeoFence.Sequence,
                        DML_Type = "I",
                        DML_Date = DateTime.Now,
                        User_Code = (sUserCode != "%") ? sUserCode : ""
                    };
                    _repositoryGfDtl.Add(gfDtl);
                }
            return true;
        }

        //http://techbrij.com/add-edit-delete-jqgrid-asp-net-web-api
        [HttpPut]
        public HttpResponseMessage EditGeofence(int id, GFMst gfMst)
        {
            gfMst.Id = id;
            var foundGeofence = _repositoryGfMst.FindBy(gfMst.Id);
            if (foundGeofence == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            foundGeofence.GFName = gfMst.GFName;
            foundGeofence.GFTitle = gfMst.GFTitle;
            foundGeofence.Comment = gfMst.Comment;
            //foundGeofence.GFType = gfMst.GFType;
            foundGeofence.GFMargin = gfMst.GFMargin;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        
        [HttpDelete]
        public HttpResponseMessage DeleteGeofence(int id)
        {
            var gfDtls = _repositoryGfDtl.FilterBy(x => x.GFMstId == id);
            foreach (var gfDtl in gfDtls) _repositoryGfDtl.Delete(gfDtl);

            _repositoryGfMst.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        public void AddGeofencePoi(int poi, string companyCode, int orgCode)
        {
            var sUserCode = SystemLibrary.GetCookies("User_Code");
            var gfPoi = new GFPOI();
            var gfPoiIdentifier = new GFPOIIdentifier();
            const string sql = "SELECT IDENT_CURRENT('RSGI_GFMST_TR')";
            var query = _session.CreateSQLQuery(sql);
            var result = query.UniqueResult();
            gfPoiIdentifier.GeoFenceNo = Convert.ToInt32(result);
            gfPoiIdentifier.PoiNo = poi;
            gfPoi.Id = gfPoiIdentifier;
            gfPoi.Company_code = companyCode;
            gfPoi.ORGCODE = orgCode;
            gfPoi.DML_Type = "I";
            gfPoi.DML_Date = DateTime.Now;
            gfPoi.User_Code = (sUserCode != "%") ? sUserCode : "";
            _repositoryGfpoi.Add(gfPoi);

        }

        public IQueryable<RouteMst> GetRoute(int orgCode, string Company_code)
        {
            RouteMst routeMst = null;
            return _repositoryRouteMst.QueryOver()
                .SelectList(list => list
                .Select(x => x.Id).WithAlias(() => routeMst.Id)
                .Select(x => x.RouteName).WithAlias(() => routeMst.RouteName))
                .Where(x => x.OrgCode == orgCode)
                .And(x => x.Company_Code == Company_code)
                .TransformUsing(Transformers.AliasToBean<RouteMst>()).List().AsQueryable();
        }

        public IQueryable<RouteDtl> GetPoisByRoute(int route)
        {
            RouteDtl routeDtl = null;

            return _repositoryRouteDtl.QueryOver()
                    .Where(x => x.Id.RouteNo == route)
                    .SelectList(list => list
                            .Select(x => x.Poi).WithAlias(() => routeDtl.Poi))
                    .TransformUsing(Transformers.AliasToBean<RouteDtl>()).List()
                    .OrderBy(x => x.PoiSeq)
                    .AsQueryable();
        }

        public dynamic GetRouteMst(GridSettings grid, string sidx, string sord, int page, int rows, int orgCode,
            string companyCode)
        {
            RouteMst routeMst = null;
            int totalRecords;

            var routeMsts = _repositoryRouteMst.QueryOver()
                .Where(x => x.OrgCode == orgCode && x.Company_Code == companyCode)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => routeMst.Id)
                    .Select(x => x.RouteName).WithAlias(() => routeMst.RouteName)
                    .Select(x => x.Title).WithAlias(() => routeMst.Title)
                    .Select(x => x.LineColor).WithAlias(() => routeMst.LineColor)
                    .Select(x => x.Comments).WithAlias(() => routeMst.Comments)
                ).TransformUsing(Transformers.AliasToBean<RouteMst>()).SearchGrid(grid, out totalRecords);


            var pageSize = rows;
            var totalPages = (int) Math.Ceiling((float) totalRecords/pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from props in routeMsts
                    select new
                    {
                        props.Id,
                        props.RouteName,
                        props.Title,
                        props.LineColor,
                        props.Comments
                    })
            };
        }

        [HttpPost]
        public bool AddRouteMst(int[] poiNos, int orgCode, string companyCode, string title, string routeName, string lineColor, string comment)
        {
            if (poiNos.Length == 0) return false;

            var sUserCode = SystemLibrary.GetCookies("User_Code");
            var routeMst = new RouteMst
            {
                OrgCode = orgCode,
                Company_Code = string.IsNullOrEmpty(companyCode) ? null : companyCode,
                LineColor = string.IsNullOrEmpty(lineColor) ? null : lineColor,
                Title = title,
                RouteName = routeName,
                Comments = comment,
                DML_Type = "I",
                DML_Date = DateTime.Now,
                User_Code = (sUserCode != "%") ? sUserCode : ""
            };
            _repositoryRouteMst.Add(routeMst);
            for (int i = 0; i < poiNos.Length; i++)
            {
                var routeDtl = new RouteDtl
                {
                    Id = new RouteDtlIdentifier() {PoiNo = poiNos[i], RouteNo = routeMst.Id},
                    PoiSeq = i+1,
                    DML_Type = "I",
                    DML_Date = DateTime.Now,
                    User_Code = (sUserCode != "%") ? sUserCode : ""
                };
                _repositoryRouteDtl.Add(routeDtl);
            }
            return true;
        }

        [HttpPut]
        public HttpResponseMessage EditRoute(int id, RouteMst routeMst)
        {
            routeMst.Id = id;
            var foundRoute = _repositoryRouteMst.FindBy(routeMst.Id);
            if (foundRoute == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            foundRoute.RouteName = routeMst.RouteName;
            foundRoute.Title = routeMst.Title;
            foundRoute.Comments = routeMst.Comments;
            foundRoute.LineColor = routeMst.LineColor;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpDelete]
        public HttpResponseMessage DeleteRoute(int id)
        {
            var routeDtls = _repositoryRouteDtl.FilterBy(x => x.Id.RouteNo == id);
            foreach (var routeDtl in routeDtls) _repositoryRouteDtl.Delete(routeDtl);

            _repositoryRouteMst.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
    
    public class PolyGeoFence
    {
        public double Lati { get; set; }
        public double Longi { get; set; }
        public int Sequence { get; set; }
    }
}
