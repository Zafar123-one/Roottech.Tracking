using System;
using System.Collections.Generic;
using System.Web.Http;
using Castle.Core.Logging;
using NHibernate.Criterion;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSGI.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.RSSP.Entities;
using Roottech.Tracking.Domain.SMAA.Entities;
using Roottech.Tracking.WebApi.Infrastructure.Attributes;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class GeoFenceApiController : ApiController
    {
        private readonly ILogger _logger;
        private readonly IKeyedRepository<int, GFDtl> _repositoryGfDtl;
        private readonly IKeyedRepository<int, GFAsset> _repositoryGfAsset;
        private readonly IKeyedRepository<double, EdrGeo> _repositoryEdrGeo;

        public GeoFenceApiController(ILogger logger, IKeyedRepository<int, GFDtl> repositoryGfDtl, IKeyedRepository<int, GFAsset> repositoryGfAsset,
            IKeyedRepository<double, EdrGeo> repositoryEdrGeo)
        {
            _logger = logger;
            _repositoryGfDtl = repositoryGfDtl;
            _repositoryGfAsset = repositoryGfAsset;
            _repositoryEdrGeo = repositoryEdrGeo;
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public IList<GeoFence> GetGeofenseForMonitoring(int oc)//orgCode,geofenceNo
        {
            GeoFence geoFence = null;
            GFMst gfMst = null;
            return _repositoryGfDtl.QueryOver()
                .JoinAlias(c => c.GFMst, () => gfMst)
                .Where(() => gfMst.OrgCode == oc) //&& gfMst.Id == gfn)

                .WithSubquery.WhereProperty(x => x.GFMstId)
                .In(QueryOver.Of<GFAsset>().Where(z => z.GeoFenceNo == gfMst.Id && z.OrgCode == gfMst.OrgCode && z.GeoFence)
                
                .WithSubquery.WhereProperty(x => x.AssetNo)
                .In(QueryOver.Of<MonitorAsset>().Where(z => z.OrgCode == gfMst.OrgCode && z.Monitor)
                .Select(Projections.Distinct(Projections.Property<MonitorAsset>(p => p.AssetNo))))

                .Select(Projections.Distinct(Projections.Property<GFAsset>(p => p.GeoFenceNo))))
                

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

        [HttpGet]
        [AllowCrossSiteJson]
        public IList<GFAsset> GetGfAssetsForMonitoring(int oc)
        {
            GFAsset gfAsset = null;
            return _repositoryGfAsset.QueryOver().Where(x => x.OrgCode == oc && x.GeoFence)
                .SelectList(list => list
                .Select(x => x.Id).WithAlias(() => gfAsset.Id)
                .Select(x => x.AssetNo).WithAlias(() => gfAsset.AssetNo)
                .Select(x => x.GeoFenceNo).WithAlias(() => gfAsset.GeoFenceNo)
                .Select(x => x.AreaToGF).WithAlias(() => gfAsset.AreaToGF)
                .Select(x => x.GFMargin).WithAlias(() => gfAsset.GFMargin))
                .TransformUsing(Transformers.AliasToBean<GFAsset>())
                .List<GFAsset>();
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public EdrGeo GetFirstEdrGeoByRtcdttmAndUpdatePullDt(int oc)
        {
            EdrGeo edrGeo = null;

            var edrGeoRec = _repositoryEdrGeo.QueryOver(() => edrGeo)
                .WhereRestrictionOn(x => x.PullDt).IsNull

                .WithSubquery.WhereProperty(x => x.CdrNo).In(QueryOver.Of<EDRFuel>()
                    .WithSubquery.WhereProperty(x => x.UnitID).In(QueryOver.Of<CompleteUnitVw>().Where(x => x.ORGCODE == oc.ToString())
                    .Select(Projections.Distinct(Projections.Property<CompleteUnitVw>(p => p.Id))))
                    .Select(Projections.Distinct(Projections.Property<EDRFuel>(p => p.Id))))

                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => edrGeo.Id)
                    .Select(x => x.CdrNo).WithAlias(() => edrGeo.CdrNo)
                    .Select(x => x.RTCDTTM).WithAlias(() => edrGeo.RTCDTTM)
                    .Select(x => x.Latitude).WithAlias(() => edrGeo.Latitude)
                    .Select(x => x.Longitude).WithAlias(() => edrGeo.Longitude)
                )
                .OrderBy(x => x.RTCDTTM).Asc.OrderBy(x => x.CdrNo).Asc
                .TransformUsing(Transformers.AliasToBean<EdrGeo>())
                .Take(1).SingleOrDefault();

            edrGeoRec.PullDt = DateTime.Now;
            _repositoryEdrGeo.Merge(edrGeoRec);
            return edrGeoRec;
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public bool UpdateEdrGeo(int id, int geoFenceNo, string iOType)
        {
            try
            {
                var edrGeo = _repositoryEdrGeo.FindBy(id);
                edrGeo.GeoFenceNo = geoFenceNo;
                edrGeo.IoType = iOType.ToCharArray(0,1)[0];
                edrGeo.UpdateDt = DateTime.Now;
                _repositoryEdrGeo.Merge(edrGeo);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public IList<GeoFence> GetGeoFencesByOrgCode(int oc)
        {
            GeoFence geoFence = null;
            GFMst gfMst = null;
            return _repositoryGfDtl.QueryOver()
                .JoinAlias(c => c.GFMst, () => gfMst)
                .Where(() => gfMst.OrgCode == oc)
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
    }
}