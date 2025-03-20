using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web.Http;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.MRAT.Entities;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.SMAA.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;
using Roottech.Tracking.Library.Models;
using Roottech.Tracking.Library.Utils;
using Roottech.Tracking.WebApi.Infrastructure.Attributes;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class VehicleApiController : ApiController
    {
        private readonly IKeyedRepository<int, Organization> _repositoryOrganization;
        private readonly IKeyedRepository<int, ResourceType> _repositoryResourceType;
        private readonly IKeyedRepository<int, UnitGroupMst> _repositoryUnitGroupMst;
        private readonly IKeyedRepository<double, Asset> _repositoryAsset;
        private readonly IKeyedRepository<string, CompleteUnitVw> _repositoryCompleteUnitVw;
        private readonly ISession _session;
        
        public VehicleApiController(IKeyedRepository<int, ResourceType> repositoryResourceType,
            IKeyedRepository<int, UnitGroupMst> repositoryUnitGroupMst,
            IKeyedRepository<double, Asset> repositoryAsset, IKeyedRepository<int, Organization> repositoryOrganization,
            ISessionFactory sessionFactory, IKeyedRepository<string, CompleteUnitVw> repositoryCompleteUnitVw)
        {
            _repositoryResourceType = repositoryResourceType;
            _repositoryUnitGroupMst = repositoryUnitGroupMst;
            _repositoryAsset = repositoryAsset;
            _repositoryOrganization = repositoryOrganization;
            _repositoryCompleteUnitVw = repositoryCompleteUnitVw;
            _session = sessionFactory.GetCurrentSession();
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public dynamic GetAllAssetsForMonitoring(int oc)
        {
            var respMessage = new HttpResponseMessage();
            Asset asset = null;
            CompleteUnitVw completeUnitVw = null;
            var assets = _repositoryAsset.QueryOver(() => asset)
                .JoinAlias(x => x.CompleteUnitVw, () => completeUnitVw)
                .Where(x => x.OrgCode == oc)
                .WithSubquery.WhereProperty(x => x.Id)
                .In(QueryOver.Of<MonitorAsset>().Where(z => z.AssetNo == asset.Id && z.OrgCode == asset.OrgCode && z.Monitor)
                .Select(Projections.Distinct(Projections.Property<MonitorAsset>(p => p.AssetNo))))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => asset.Id)
                    .Select(x => x.AssetName).WithAlias(() => asset.AssetName)
                    .Select(x => x.PlateId).WithAlias(() => asset.PlateId)
                    .Select(() => completeUnitVw.Id).WithAlias(() => asset.inventoryNo)
                ).TransformUsing(Transformers.AliasToBean<Asset>()).List<Asset>().ToList();

            if (assets.Any())
                respMessage.Content = new ObjectContent<dynamic>(from props in assets
                    select new { props.Id, props.AssetName, props.PlateId, props.inventoryNo}, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        /*
[Queryable]
public IQueryable<TreeView> GetResourceType(int orgCode)
{
    //ResourceType resourceType = null;
    TreeView treeView = null;
    var distinctResourceTypeNoSubQuery = QueryOver.Of<SiteResource>().Where(x => x.orgCode == orgCode)
        .WhereRestrictionOn(x => x.ResourceTypeNo).IsNotNull.Select( //x => x.ResourceTypeNo)
        Projections.Distinct(Projections.Property<SiteResource>(p => p.ResourceTypeNo)));
    return _repositoryResourceType.QueryOver().Where(x => x.orgCode == orgCode)
        .WithSubquery.WhereProperty(x => x.Id).In(distinctResourceTypeNoSubQuery)
        .SelectList(list => list
        .Select(Projections.SqlFunction(
        new SQLFunctionTemplate(NHibernateUtil.String, "convert(varchar, ?1, 110)"),NHibernateUtil.String, Projections.Property<Organization>(x => x.Id)))
        .WithAlias(() => treeView.id)
        .Select(x => x.Description).WithAlias(() => treeView.text)
        .Select(Projections.Cast(NHibernateUtil.Boolean, Projections.Constant(1))).WithAlias(() => treeView.hasChildren)
        .Select(Projections.Cast(NHibernateUtil.Boolean, Projections.Constant(1))).WithAlias(() => treeView.expanded)

        ).TransformUsing(Transformers.AliasToBean<TreeView>()).List<TreeView>().AsQueryable();

        //return _repositoryResourceType.FilterBy(x => x.orgCode == orgCode);
}
*/


        //public IQueryable<UnitGroupMst> GetVehicleTree(int orgCode, int ResourceType)
        //{
        //    orgCode = 13;
        //    ResourceType = 5;
        //    UnitGroupMst unitGroupMst = null;
        //    UnitGroupDtl unitGroupDtlAlias = null;
        //    SiteResource siteResourceAlias = null;

        //    var unitGroupDtlSubQuery = QueryOver.Of<UnitGroupDtl>(() => unitGroupDtlAlias).WhereRestrictionOn(x => x.Id).IsNotNull
        //        .WithSubquery
        //        .WhereProperty(x => x.Site_Code).In(QueryOver.Of<SiteResource>()
        //                                            .Where(x => x.ResourceTypeNo == ResourceType)
        //                                            .And(x => x.Id.ResourceNo == unitGroupDtlAlias.ResourceNo)
        //                                            .WhereRestrictionOn(x => x.ResourceTypeNo).IsNotNull.Select( //x => x.ResourceTypeNo)
        //                                            Projections.Distinct(Projections.Property<SiteResource>(p => p.Id.Site_Code)))
        //                                            )
        //        .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.Id)));

        //    var unitGroupMsts  = _repositoryUnitGroupMst.QueryOver().Where(x => x.orgCode == orgCode)
        //        .WithSubquery.WhereProperty(x => x.Id).In(unitGroupDtlSubQuery)
        //        .SelectList(list => list
        //            .Select(x => x.Id).WithAlias(() => unitGroupMst.Id)
        //            .Select(x => x.Description).WithAlias(() => unitGroupMst.Description)
        //        ).TransformUsing(Transformers.AliasToBean<UnitGroupMst>()).List();

        //    return _repositoryUnitGroupMst.FilterBy(x => x.orgCode == orgCode);
        //}


        //public IQueryable<Asset> GetPlate(int orgCode, int uGrpMstNo)
        //{
        //    orgCode = 13;
        //    uGrpMstNo = 5;
        //    Asset Assets = null;
        //    //var distinctAssetsNoSubQuery = QueryOver.Of<SiteResource>().Where(x => x.AssetNo == Assets.Id)
        //    //    .Select(Projections.Distinct(Projections.Property<SiteResource>(p => p.AssetNo)));



        //    var resourceTypes = _repositoryAsset.QueryOver(() => Assets).Where(x => x.orgCode == orgCode)
        //        .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<SiteResource>().Where(x => x.AssetNo == Assets.Id)
        //        .Select(Projections.Distinct(Projections.Property<SiteResource>(p => p.AssetNo))))
        //        .SelectList(list => list
        //            .Select(x => x.Id).WithAlias(() => Assets.Id)
        //            .Select(x => x.AssetName).WithAlias(() => Assets.AssetName)
        //        ).TransformUsing(Transformers.AliasToBean<Asset>()).List();

        //    return _repositoryAsset.FilterBy(x => x.orgCode == orgCode);
        //}


        [Queryable]
        private IQueryable<TreeViewNode> GetOrganizations(string sOrgCode, string sOrgName)
        {
            TreeViewNode treeViewNode = null;
            Organization clientL3 = null;
            var sqlFunctionClientId = Projections.Cast(NHibernateUtil.String,
                                                        Projections.SqlFunction("concat", NHibernateUtil.String,
                                                        Projections.Conditional(Restrictions.IsNotNull(Projections.Property<Organization>(c => c.ParentOrg)), Projections.Cast(NHibernateUtil.String, Projections.Property<Organization>(c => c.ParentOrg)),
                                                        Projections.Cast(NHibernateUtil.String, Projections.Property<Organization>(c => c.Id))),
                                                        Projections.Constant("-"),
                                                        Projections.Cast(NHibernateUtil.String, Projections.Property<Organization>(c => c.Id))));

            var countSubQuery = QueryOver.Of<ResourceType>().Where(c => c.OrgCode == clientL3.ParentOrg || c.OrgCode == clientL3.Id).ToRowCountQuery();

            if (sOrgCode == "%")
            {
                return _repositoryOrganization.QueryOver(() => clientL3)
                    .AndRestrictionOn(() => clientL3.DSCR).IsLike(sOrgName)
                    .SelectList(list => list
                                            .Select(sqlFunctionClientId).WithAlias(() => treeViewNode.id)
                                            .Select(x => x.DSCR).WithAlias(() => treeViewNode.text)
                                            .Select(Projections.Cast(NHibernateUtil.String, Projections.Constant(""))).
                                            WithAlias(() => treeViewNode.classes)
                                            .Select(
                                                Projections.Conditional(
                                                    Restrictions.Eq(Projections.SubQuery(countSubQuery), 0),
                                                    Projections.Constant(false, NHibernateUtil.Boolean),
                                                    Projections.Constant(true, NHibernateUtil.Boolean)
                                                    )).WithAlias(() => treeViewNode.hasChildren)
                        .Select(Projections.Cast(NHibernateUtil.Byte, Projections.Constant(1))).WithAlias(() => treeViewNode.level)
                    ).OrderBy(x => x.DSCR).Asc.TransformUsing(Transformers.AliasToBean<TreeViewNode>()).List<TreeViewNode>().AsQueryable();
            }
            else
            {
                int myOrgCode = Convert.ToInt32(sOrgCode);
                return _repositoryOrganization.QueryOver(() => clientL3).Where(c => c.Id == myOrgCode)
                    .AndRestrictionOn(() => clientL3.DSCR).IsLike(sOrgName)
                    .SelectList(list => list
                        .Select(sqlFunctionClientId).WithAlias(() => treeViewNode.id)
                        .Select(x => x.DSCR).WithAlias(() => treeViewNode.text)
                        .Select(Projections.Cast(NHibernateUtil.String, Projections.Constant(""))).WithAlias(() => treeViewNode.classes)
                        .Select(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(countSubQuery), 0),
                        Projections.Constant(false, NHibernateUtil.Boolean),
                        Projections.Constant(true, NHibernateUtil.Boolean)
                        )).WithAlias(() => treeViewNode.hasChildren)
                        .Select(Projections.Cast(NHibernateUtil.Byte, Projections.Constant(1))).WithAlias(() => treeViewNode.level)
                    ).TransformUsing(Transformers.AliasToBean<TreeViewNode>()).List<TreeViewNode>().AsQueryable();
            }

        }

        public IQueryable<TreeViewNode> GetResourceType(string clientL3Ids, int ParentOrg, string sResourceType)
        {
            int clientL3Id = Convert.ToInt32(clientL3Ids);
            TreeViewNode treeViewNode = null;
            ResourceType clientL2 = null;
            UnitGroupDtl unitGroupDtlAlias = null;
            var sqlFunctionClientId = Projections.Cast(NHibernateUtil.String,
                                                        Projections.SqlFunction("concat", NHibernateUtil.Int32,
                                                        Projections.Constant(clientL3Id + "."),
                                                        Projections.Cast(NHibernateUtil.String, Projections.Property<ResourceType>(c => c.Id))));

            var unitGroupDtlSubQuery = QueryOver.Of<UnitGroupDtl>(() => unitGroupDtlAlias).WhereRestrictionOn(x => x.Id).IsNotNull
                .WithSubquery
                .WhereProperty(x => x.Site_Code).In(QueryOver.Of<SiteResource>()
                                                    .Where(x => x.ResourceTypeNo == clientL2.Id)
                                                    .And(x => x.Id.ResourceNo == unitGroupDtlAlias.ResourceNo)
                                                    .WhereRestrictionOn(x => x.ResourceTypeNo).IsNotNull.Select( //x => x.ResourceTypeNo)
                                                    Projections.Distinct(Projections.Property<SiteResource>(p => p.Id.Site_Code)))
                                                    )

                .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.Id)));

            var countSubQuery = QueryOver.Of<UnitGroupMst>().Where(c => c.OrgCode == clientL3Id)
                .WithSubquery
                .WhereProperty(x => x.Id).In(unitGroupDtlSubQuery)
                .ToRowCountQuery();//clientL2.Id).ToRowCountQuery();
            return _repositoryResourceType.QueryOver(() => clientL2)
                .Where(x => x.OrgCode == ParentOrg)
                .AndRestrictionOn(() => clientL2.Description).IsLike(sResourceType)
                .SelectList(list => list
                    .Select(sqlFunctionClientId).WithAlias(() => treeViewNode.id)
                    .Select(x => x.Description).WithAlias(() => treeViewNode.text)
                    .Select(Projections.Cast(NHibernateUtil.String, Projections.Constant(""))).WithAlias(() => treeViewNode.classes)
                    .Select(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(countSubQuery), 0),
                    Projections.Constant(false, NHibernateUtil.Boolean),
                    Projections.Constant(true, NHibernateUtil.Boolean)
                    )).WithAlias(() => treeViewNode.hasChildren)
                    .Select(Projections.Cast(NHibernateUtil.Byte, Projections.Constant(2))).WithAlias(() => treeViewNode.level)
                ).OrderBy(x => x.Description).Asc.TransformUsing(Transformers.AliasToBean<TreeViewNode>()).List<TreeViewNode>().AsQueryable();
        }

        public IQueryable<TreeViewNode> GetUnitGroup(int OrgCode, int clientL3Id, string sGroup)
        {
            TreeViewNode treeViewNode = null;
            UnitGroupMst clientL2 = null;
            UnitGroupDtl unitGroupDtlAlias = null;
            SiteResource siteResourceAlias = null;

            var unitGroupDtlSubQuery = QueryOver.Of<UnitGroupDtl>(() => unitGroupDtlAlias).WhereRestrictionOn(x => x.Id).IsNotNull
                .WithSubquery
                .WhereProperty(x => x.Site_Code).In(QueryOver.Of<SiteResource>()
                                                    .Where(x => x.ResourceTypeNo == clientL3Id)
                                                    .And(x => x.Id.ResourceNo == unitGroupDtlAlias.ResourceNo)
                                                    .WhereRestrictionOn(x => x.ResourceTypeNo).IsNotNull.Select( //x => x.ResourceTypeNo)
                                                    Projections.Distinct(Projections.Property<SiteResource>(p => p.Id.Site_Code)))
                                                    )
                .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.Id)));

            var sqlFunctionClientId = Projections.Cast(NHibernateUtil.String, Projections.SqlFunction("concat",
                                            NHibernateUtil.String,
                                            Projections.Constant(OrgCode + "."),
                                            Projections.Constant(clientL3Id + "."),
                                            Projections.Cast(NHibernateUtil.String, Projections.Property<UnitGroupMst>(c => c.Id))));

            var countSubQuery = QueryOver.Of<SiteResource>().Where(c => c.ResourceTypeNo == clientL3Id)
                .And(c => c.OrgCode == OrgCode)
                .ToRowCountQuery();//clientL2.Id).ToRowCountQuery();
            return _repositoryUnitGroupMst.QueryOver().Where(x => x.OrgCode == OrgCode)
                .WithSubquery.WhereProperty(x => x.Id).In(unitGroupDtlSubQuery)
                .AndRestrictionOn(x => x.Description).IsLike(sGroup)
                .SelectList(list => list
                    .Select(sqlFunctionClientId).WithAlias(() => treeViewNode.id)
                    .Select(x => x.Description).WithAlias(() => treeViewNode.text)
                    .Select(Projections.Cast(NHibernateUtil.String, Projections.Constant(""))).WithAlias(() => treeViewNode.classes)
                    .Select(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(countSubQuery), 0),
                    Projections.Constant(false, NHibernateUtil.Boolean),
                    Projections.Constant(true, NHibernateUtil.Boolean)
                    )).WithAlias(() => treeViewNode.hasChildren)
                        .Select(Projections.Cast(NHibernateUtil.Byte, Projections.Constant(3))).WithAlias(() => treeViewNode.level)
                //.SelectSubQuery(countSubQuery).WithAlias(() => treeViewNode.hasChildren)
                ).OrderBy(x => x.Description).Asc.TransformUsing(Transformers.AliasToBean<TreeViewNode>()).List<TreeViewNode>().AsQueryable();

            //orgCode = 13;
            //ResourceType = 5;


            //var unitGroupMsts = _repositoryUnitGroupMst.QueryOver().Where(x => x.orgCode == orgCode)
            //    .WithSubquery.WhereProperty(x => x.Id).In(unitGroupDtlSubQuery)
            //    .SelectList(list => list
            //        .Select(x => x.Id).WithAlias(() => unitGroupMst.Id)
            //        .Select(x => x.Description).WithAlias(() => unitGroupMst.Description)
            //    ).TransformUsing(Transformers.AliasToBean<UnitGroupMst>()).List();

            //return _repositoryUnitGroupMst.FilterBy(x => x.orgCode == orgCode);
        }
        /*
                public IQueryable<TreeViewNode> GetPlate(int orgCode, int resourceType, int uGrpMstNo, string sPlateId)
                {

                    Asset Assets = null;
                    TreeViewNode treeViewNode = null;
                    var convertToStringAssetId = Projections.Cast(NHibernateUtil.String, Projections.Property<Asset>(c => c.Id));
                    return _repositoryAsset.QueryOver(() => Assets).Where(x => x.OrgCode == orgCode)
                        .WithSubquery.WhereProperty(x => x.Id).In(QueryOver.Of<SiteResource>().Where(x => x.AssetNo == Assets.Id)
                        .WithSubquery.WhereProperty(x => x.Id.Site_Code).In(QueryOver.Of<UnitGroupDtl>().Where(y=> y.Id == uGrpMstNo)
                                                                            .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.Site_Code))))
                        .WithSubquery.WhereProperty(x => x.Id.ResourceNo).In(QueryOver.Of<UnitGroupDtl>().Where(y => y.Id == uGrpMstNo)
                                                                            .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.ResourceNo))))
                        .Select(Projections.Distinct(Projections.Property<SiteResource>(p => p.AssetNo))))
                        .AndRestrictionOn(x => x.AssetName).IsLike(sPlateId)
                        .SelectList(list => list
                            .Select(convertToStringAssetId).WithAlias(() => treeViewNode.id)
                            .Select(x => x.AssetName).WithAlias(() => treeViewNode.text)
                            .Select(Projections.Cast(NHibernateUtil.String, Projections.Constant(""))).WithAlias(() => treeViewNode.classes)
                            //.Select(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(countSubQuery), 0),
                            //Projections.Constant(false, NHibernateUtil.Boolean),
                            //Projections.Constant(true, NHibernateUtil.Boolean)
                            //))
                            .Select(Projections.Cast(NHibernateUtil.Boolean, Projections.Constant(0))).WithAlias(() => treeViewNode.hasChildren)
                            .Select(convertToStringAssetId).WithAlias(() => treeViewNode.fullName)
                        ).TransformUsing(Transformers.AliasToBean<TreeViewNode>()).List<TreeViewNode>().AsQueryable();


                }
        */
        public IQueryable<TreeViewNode> GetPlate(int orgCode, int resourceType, int uGrpMstNo, string sPlateId)
        {

            Asset Assets = null;
            TreeViewNode treeViewNode = null;
            CompleteUnitVw completeUnitVw = null;
            var convertToStringAssetId = Projections.Cast(NHibernateUtil.String, Projections.Property<Asset>(c => c.Id));
            return _repositoryAsset.QueryOver(() => Assets)
                .JoinAlias(x => x.CompleteUnitVw, () => completeUnitVw)
                .Where(x => x.OrgCode == orgCode)
                .WithSubquery.WhereProperty(() => completeUnitVw.Id).In(QueryOver.Of<TrckUnit>().Select(x => x.Id)
                .Where(y => y.Orgcode == orgCode.ToString()).WithSubquery.WhereProperty(x => x.Ustatus).In(QueryOver.Of<RsptUStatus>().Select(x => x.Id).Where(x => x.StatusType == "A")))
                .WithSubquery.WhereProperty(() => completeUnitVw.SITE_Code).In(QueryOver.Of<UnitGroupDtl>().Where(y => y.Id == uGrpMstNo)
                                                                    .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.Site_Code))))
                .WithSubquery.WhereProperty(() => completeUnitVw.ResourceNo).In(QueryOver.Of<UnitGroupDtl>().Where(y => y.Id == uGrpMstNo)
                                                                    .Select(Projections.Distinct(Projections.Property<UnitGroupDtl>(p => p.ResourceNo))))
                .AndRestrictionOn(() => completeUnitVw.PlateID).IsLike(sPlateId)
                .SelectList(list => list
                    .Select(convertToStringAssetId).WithAlias(() => treeViewNode.id)
                    .Select(() => completeUnitVw.PlateID).WithAlias(() => treeViewNode.text)
                    .Select(() => completeUnitVw.Id).WithAlias(() => treeViewNode.fullName)
                    .Select(Projections.Cast(NHibernateUtil.String, Projections.Constant(""))).WithAlias(() => treeViewNode.classes)
                    //.Select(Projections.Conditional(Restrictions.Eq(Projections.SubQuery(countSubQuery), 0),
                    //Projections.Constant(false, NHibernateUtil.Boolean),
                    //Projections.Constant(true, NHibernateUtil.Boolean)
                    //))
                    .Select(Projections.Cast(NHibernateUtil.Boolean, Projections.Constant(0))).WithAlias(() => treeViewNode.hasChildren)
                        .Select(Projections.Cast(NHibernateUtil.Byte, Projections.Constant(4))).WithAlias(() => treeViewNode.level)
                //.Select(convertToStringAssetId).WithAlias(() => treeViewNode.fullName)

                ).OrderBy(() => completeUnitVw.PlateID).Asc.TransformUsing(Transformers.AliasToBean<TreeViewNode>()).List<TreeViewNode>().AsQueryable();


        }
        [Queryable]
        [HttpGet]
        public HttpResponseMessage getTreeView(string root, string sPlateId, string sOrgName, string sResourceType, string sGroup)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            string sOrgCode = principal.FindFirst("loginOrgCode").Value;
            var respMessage = new HttpResponseMessage();

            if (root == "source")
            {
                var organizations = GetOrganizations(sOrgCode, sOrgName == "" ? "%" : "%" + sOrgName + "%");
                if (organizations.Any())
                    respMessage.Content =
                        new ObjectContent<IQueryable<TreeViewNode>>(organizations, new JsonMediaTypeFormatter());
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
                    
                return respMessage;
            }

            if (root.IndexOf(".", StringComparison.Ordinal) > -1)
            {
                if (root.Split('.').Length == 2)
                {
                    var unitGroup = GetUnitGroup(Convert.ToInt32(root.Split('.')[0]), Convert.ToInt32(root.Split('.')[1]),
                        sGroup == "" ? "%" : "%" + sGroup + "%");
                    if (unitGroup.Any())
                        respMessage.Content = new ObjectContent<IQueryable<TreeViewNode>>(unitGroup, new JsonMediaTypeFormatter());
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
                    return respMessage;
                }
                var plate = GetPlate(Convert.ToInt32(root.Split('.')[0]), Convert.ToInt32(root.Split('.')[1]), Convert.ToInt32(root.Split('.')[2]), sPlateId == "" ? "%" : "%" + sPlateId + "%");
                if (plate.Any())
                    respMessage.Content = new ObjectContent<IQueryable<TreeViewNode>>(plate, new JsonMediaTypeFormatter());
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
                return respMessage;
            }
            if (root.Split('-').Length > 1)
            {
                var resourceType = GetResourceType(root.Split('-')[1], Convert.ToInt32(root.Split('-')[0]),
                    sResourceType == "" ? "%" : "%" + sResourceType + "%");
                if (resourceType.Any())
                    respMessage.Content =
                        new ObjectContent<IQueryable<TreeViewNode>>(resourceType, new JsonMediaTypeFormatter());
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            }
            else
                //respMessage.ReasonPhrase = "Wrong parameters";//new HttpRequestMessage( HttpMethod.Get, "Wrong parameters");
                respMessage.StatusCode = HttpStatusCode.BadRequest;
            return respMessage;
        }

        public HttpResponseMessage  getLastCdr(string assetNo)
        {
            var respMessage = new HttpResponseMessage();
            if (string.IsNullOrEmpty(assetNo))
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Vehicle no. should not be empty.");

            var lastCdrs = _session.GetNamedQuery("GetLastCDR").SetString("AssetNo", assetNo).List<LastCdr>();
            if (lastCdrs.Any())
                respMessage.Content = new ObjectContent<IList<LastCdr>>(lastCdrs, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getVehicleAmpleView(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var vehicleAmpleViews = _session.GetNamedQuery("GetAmpleViewInfo").SetString("AssetNo", assetNo.ToString()).List<VehicleAmpleView>();
            if (vehicleAmpleViews.Any())
                respMessage.Content = new ObjectContent<IList<VehicleAmpleView>>(vehicleAmpleViews, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getCurrentStatusAmpleView(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var currentStatusAmpleViews = _session.GetNamedQuery("GetAmpleViewCurrentStatus")
                .SetString("AssetNo", assetNo.ToString())
                .List<CurrentStatusAmpleView>();
            if (currentStatusAmpleViews.Any())
                respMessage.Content = new ObjectContent<IList<CurrentStatusAmpleView>>(currentStatusAmpleViews, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getFleetInfoAmpleView(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var fleetInfoAmpleViews = _session.GetNamedQuery("GetFleetAmpleViewInfo")
                .SetString("AssetNo", assetNo.ToString())
                .List<FleetInfoAmpleView>();
            if (fleetInfoAmpleViews.Any())
                respMessage.Content = new ObjectContent<IList<FleetInfoAmpleView>>(fleetInfoAmpleViews, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }
        public HttpResponseMessage getLastEventAmpleView(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var lastEventAmpleViews = _session.GetNamedQuery("GetLastEventAmpleView")
                .SetString("AssetNo", assetNo.ToString())
                .List<LastEventAmpleView>();
            if (lastEventAmpleViews.Any())
                respMessage.Content = new ObjectContent<IList<LastEventAmpleView>>(lastEventAmpleViews, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getMonthlyAmpleView(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var monthlyAmpleViews = _session.GetNamedQuery("GetMonthlyAmpleView")
                .SetString("AssetNo", assetNo.ToString())
                .List<MonthlyAmpleView>();
            if (monthlyAmpleViews.Any())
                respMessage.Content = new ObjectContent<IList<MonthlyAmpleView>>(monthlyAmpleViews, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getSensorBoard(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var sensorBoards = _session.GetNamedQuery("GetSensorBoard")
                .SetString("AssetNo", assetNo.ToString())
                .List<SensorBoard>();
            if (sensorBoards.Any())
                respMessage.Content = new ObjectContent<IList<SensorBoard>>(sensorBoards, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getAssetsImage(string assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var assetsImages = _session.GetNamedQuery("GetAssetsImage")
                .SetString("AssetNo", assetNo)
                .List<AssetsImage>();
            if (assetsImages.Any())
                respMessage.Content = new ObjectContent<IList<AssetsImage>>(assetsImages, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage GetVehicleList()
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var getOrgCode = principal.FindFirst("loginOrgCode").Value;

            var respMessage = new HttpResponseMessage();
            CompleteUnitVw completeUnitVw = null;
            IQueryable<CompleteUnitVw> completeUnitVws;
            if (getOrgCode == "%")
                completeUnitVws = _repositoryCompleteUnitVw.QueryOver().Where(x => x.PlateID != "")
                    .SelectList(list => list
                        .Select(x => x.Id).WithAlias(() => completeUnitVw.Id)
                        .Select(x => x.PlateID).WithAlias(() => completeUnitVw.PlateID))
                    .TransformUsing(Transformers.AliasToBean<CompleteUnitVw>()).List().AsQueryable();
            else
                completeUnitVws = _repositoryCompleteUnitVw.QueryOver().Where(x => x.ORGCODE == getOrgCode)
                    .SelectList(list => list
                        .Select(x => x.Id).WithAlias(() => completeUnitVw.Id)
                        .Select(x => x.PlateID).WithAlias(() => completeUnitVw.PlateID))
                    .TransformUsing(Transformers.AliasToBean<CompleteUnitVw>()).List().AsQueryable();

            if (completeUnitVws.Any())
                respMessage.Content = new ObjectContent<IQueryable<CompleteUnitVw>>(completeUnitVws, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage getCurrentStatus(int assetNo)
        {
            var respMessage = new HttpResponseMessage();
            var currentStatuses = _session.GetNamedQuery("GetCurrentStatus")
                .SetString("AssetNo", assetNo.ToString())
                .List<CurrentStatus>();
            if (currentStatuses.Any())
                respMessage.Content = new ObjectContent<IList<CurrentStatus>>(currentStatuses, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }
    }
}
