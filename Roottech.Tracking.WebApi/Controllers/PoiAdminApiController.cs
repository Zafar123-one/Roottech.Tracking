using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using NHibernate;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSGI.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;
using Roottech.Tracking.Library.Models;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.Models.Helpers;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    //http://localhost:82/api/PoiAdminApi/GetPoiIdByOrgCodeAndPoiTypeNoAndLatAndLngAndBuffer?orgCode=&poiTypeNo=7&lat=24.86611333&lng=67.308735&bufferInMeters=100
    public class PoiAdminApiController : ApiController
    {
        private readonly IKeyedRepository<int, Poi> _repositoryPoi;
        private readonly IKeyedRepository<int, PoiType> _repositoryPoiType;
        private readonly IKeyedRepository<string, AreaDtl> _repositoryAreaDtl;
        private readonly IKeyedRepository<int, PoiImage> _repositoryPoiImage;
        private readonly IKeyedRepository<int, PoiImageAccess> _repositoryPoiImageAccess;
        private readonly IKeyedRepository<string, UserProfile> _repositoryUserProfile;
        private readonly ISession _session;

        public PoiAdminApiController(IKeyedRepository<int, Poi> repositoryPoi,
            IKeyedRepository<int, PoiType> repositoryPoiType,
            IKeyedRepository<string, AreaDtl> repositoryAreaDtl,
            IKeyedRepository<int, PoiImage> repositoryPoiImage,
            IKeyedRepository<int, PoiImageAccess> repositoryPoiImageAccess,
            IKeyedRepository<string, UserProfile> repositoryUserProfile,
            ISessionFactory sessionFactory)
        {
            _repositoryPoi = repositoryPoi;
            _repositoryPoiType = repositoryPoiType;
            _repositoryAreaDtl = repositoryAreaDtl;
            _repositoryPoiImage = repositoryPoiImage;
            _repositoryPoiImageAccess = repositoryPoiImageAccess;
            _repositoryUserProfile = repositoryUserProfile;
            _session = sessionFactory.GetCurrentSession();
        }

        public int GetPoiIdByOrgCodeAndPoiTypeNoAndLatAndLngAndBuffer(int? orgCode, int poiTypeNo, double lat, double lng, int bufferInMeters)
        {
            var queryOverPoi = _repositoryPoi.QueryOver().Where(x => x.PoiTypeNo == poiTypeNo);

            if (orgCode != null)
                queryOverPoi.And(x => x.OrgCode == orgCode);

            var buffer = 0;
            foreach (var poi in queryOverPoi.List())
            {
                if (poi.GF_YN == "Y" && poi.GF_Val != 0)
                    buffer = poi.GF_Val;
                else
                    buffer = bufferInMeters;

                if (GetDistanceBetweenTwoPoints((double)poi.Lati, (double)poi.Longi, lat, lng) < buffer)
                    return poi.Id;
            }
            return 0;
        }

        private double GetDistanceBetweenTwoPoints(double lat1, double lng1, double lat2, double lng2)
        {
            const double deg2Rad = 0.017453292519943295; // === Math.PI/180;//
            const int diam = 12742; // Diameter of the earth in km (2 * 6371)
            lat1 *= deg2Rad;
            lng1 *= deg2Rad;
            lat2 *= deg2Rad;
            lng2 *= deg2Rad;
            var dLat = lat2 - lat1;
            var dLon = lng2 - lng1;
            var a = (
                (1 - Math.Cos(dLat)) +
                (1 - Math.Cos(dLon)) *
                Math.Cos(lat1) * Math.Cos(lat2)
                ) / 2;

            return diam * Math.Asin(Math.Sqrt(a));
        }

        [HttpGet]
        public IList<OrganizationsByUserCode> GetOrganizationsByUserCode()
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            return _session.GetNamedQuery("GetOrganizationsByUserCode")
                .SetString("User_Code", principal.FindFirst("User_Code").Value)
                .List<OrganizationsByUserCode>();
        }

        [HttpGet]
        public IList<CompaniesByUserAndOrg> GetCompaniesbyOrgCode(string orgCode)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            return _session.GetNamedQuery("GetCompaniesByUserAndOrg")
                .SetString("User_Code", principal.FindFirst("User_Code").Value)
                .SetString("Org_Code", orgCode)
                .List<CompaniesByUserAndOrg>();
        }

        [Queryable]
        [HttpGet]
        public IQueryable<PoiType> GetPoiTypes()//(int orgCode)
        {
            PoiType poiType = null;

            return _repositoryPoiType.QueryOver()
                .SelectList(list => list
                .Select(x => x.Id).WithAlias(() => poiType.Id)
                .Select(x => x.TypeName).WithAlias(() => poiType.TypeName))
                //.Where(x => x.OrgCode == orgCode) // not specific to any organization or company code its for all according to Mr. Habib
                // .And(x => x.Company_Code == Company_code)
                .TransformUsing(Transformers.AliasToBean<PoiType>()).List().AsQueryable();
        }

        [HttpGet]
        public dynamic GetPois(GridSettings grid, string sidx, string sord, int page, int rows, int orgCode,
                              string companyCode)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            grid.SortColumn = grid.SortColumn.Replace("TypeName", "User_Code");//"poiType.TypeName");
            string sOrgCode = principal.FindFirst("loginOrgCode").Value;
            Poi poi = null;
            PoiType poiType = null;
            int totalRecords;
            if (sOrgCode != "%")
            {
                var companiesByUserAndOrgs = GetCompaniesbyOrgCode(sOrgCode);
                if (companiesByUserAndOrgs.Count > 0)
                    companyCode = companiesByUserAndOrgs[0].Id;
            }
            KeyValue<int, string> keyValue = null;
            var userProfileFuture = _repositoryUserProfile.QueryOver()
                .Where(x => x.Orgcode == (sOrgCode == "%" ? orgCode : Convert.ToInt16(sOrgCode)))
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => keyValue.Id)
                    .Select(x => x.User_Name).WithAlias(() => keyValue.Name))
                    .TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>()).Future<KeyValue<string, string>>();// .List<KeyValue<int, string>>();

            var queryOverPoi = _repositoryPoi.QueryOver()
                .JoinAlias(c => c.PoiType, () => poiType);
            queryOverPoi.Where(x => x.OrgCode == (sOrgCode == "%" ? orgCode : Convert.ToInt16(sOrgCode))
                && x.Company_Code == companyCode);

            var pois = queryOverPoi.SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => poi.Id)
                    .Select(x => x.PoiName).WithAlias(() => poi.PoiName)
                    .Select(x => x.PoiTypeNo).WithAlias(() => poi.PoiTypeNo)
                    .Select(() => poiType.TypeName).WithAlias(() => poi.User_Code)
                    .Select(x => x.PostalAddr).WithAlias(() => poi.PostalAddr)
                    .Select(x => x.Block_Code).WithAlias(() => poi.Block_Code)
                    .Select(x => x.AreaId).WithAlias(() => poi.AreaId)
                    .Select(x => x.CityId).WithAlias(() => poi.CityId)
                    .Select(x => x.StateId).WithAlias(() => poi.StateId)
                    .Select(x => x.CountryId).WithAlias(() => poi.CountryId)
                    .Select(x => x.Lati).WithAlias(() => poi.Lati)
                    .Select(x => x.Longi).WithAlias(() => poi.Longi)
                    .Select(x => x.GF_YN).WithAlias(() => poi.GF_YN)
                    .Select(x => x.GF_Val).WithAlias(() => poi.GF_Val)
                    .Select(x => x.CustomerRefNo).WithAlias(() => poi.CustomerRefNo)
                    .Select(x => x.ContactPerson).WithAlias(() => poi.ContactPerson)
                    .Select(x => x.Phone).WithAlias(() => poi.Phone)
                    .Select(x => x.Cell).WithAlias(() => poi.Cell)
                    .Select(x => x.Email).WithAlias(() => poi.Email)
                ).TransformUsing(Transformers.AliasToBean<Poi>()).Future<Poi>().AsQueryable().SearchGrid(grid, out totalRecords); //

            var users = userProfileFuture.ToList();
            var pageSize = (rows == 0) ? pois.Count() : rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                userdata = (from props in users select new { props.Id, props.Name }),
                rows = (from props in pois
                        select new
                        {
                            props.Id,
                            props.PoiName,
                            props.PoiTypeNo,
                            TypeName = props.User_Code,
                            props.PostalAddr,
                            props.Block_Code,
                            props.AreaId,
                            props.CityId,
                            props.StateId,
                            props.CountryId,
                            props.Lati,
                            props.Longi,
                            props.GF_YN,
                            props.GF_Val,
                            props.CustomerRefNo,
                            props.ContactPerson,
                            props.Phone,
                            props.Cell,
                            props.Email
                        })
            };
        }

        [HttpGet]
        public string GetGImagePathByPoiTypeNo(int poiTypeNo)
        {
            GImage gImage = null;
            PoiType poiType = null;
            return _repositoryPoiType.QueryOver()
                .JoinAlias(c => c.GImage, () => gImage)
                .Where(x => x.Id == poiTypeNo)
                .SelectList(list => list
                    .Select(() => gImage.ImgPath).WithAlias(() => poiType.Title)).SingleOrDefault<string>();
            //.SingleOrDefault().GImage.ImgPath;
        }

        public dynamic getCompleteLocationsByOrgCode(string orgCode, string q)
        {
            return _session.GetNamedQuery("GetCompleteLocationByOrgCode")
                           .SetString("Org_Code", orgCode)
                           .SetString("SearchText", q)
                           .SetString("BlockCode", "%")
                           .List<CompleteLocationByOrgCode>().Select(x => new
                           {
                               id = x.Id, //jquery tokeninput wants the small case id and name
                               name = x.Name
                           });
        }

        public string getCompleteLocationByOrgCodeAndBlockCode(string orgCode, string blockCode)
        {
            var location =
                _session.GetNamedQuery("GetCompleteLocationByOrgCode")
                        .SetString("Org_Code", orgCode)
                        .SetString("SearchText", "%")
                        .SetString("BlockCode", blockCode)
                        .List<CompleteLocationByOrgCode>();
            if (location.Count > 0)
                return location[0].Name;
            return null;
        }
        //http://techbrij.com/add-edit-delete-jqgrid-asp-net-web-api
        [HttpPut]
        public HttpResponseMessage EditPoi(int id, Poi poi)
        {
            poi.Id = id;
            var foundPoi = _repositoryPoi.FindBy(poi.Id);
            if (foundPoi == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            foundPoi.PoiName = poi.PoiName;
            foundPoi.PoiTypeNo = poi.PoiTypeNo;
            foundPoi.PostalAddr = poi.PostalAddr;
            if (foundPoi.Block_Code != poi.Block_Code)
            {
                foundPoi.Block_Code = poi.Block_Code;
                var areaDtl = _repositoryAreaDtl.FindBy(poi.Block_Code);
                foundPoi.AreaId = areaDtl.AreaId;
                foundPoi.CityId = areaDtl.CityId;
                foundPoi.StateId = areaDtl.StateId;
                foundPoi.CountryId = areaDtl.CountryId;
            }
            foundPoi.Lati = poi.Lati;
            foundPoi.Longi = poi.Longi;
            foundPoi.GF_YN = poi.GF_YN;
            foundPoi.GF_Val = poi.GF_Val;
            foundPoi.CustomerRefNo = poi.CustomerRefNo;
            foundPoi.ContactPerson = poi.ContactPerson;
            foundPoi.Phone = poi.Phone;
            foundPoi.Cell = poi.Cell;
            foundPoi.Email = poi.Email;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage AddPoi(Poi poi)
        {
            //var sOrgCode = SystemLibrary.GetCookies("loginOrgCode");
            //poi.OrgCode = (sOrgCode != "%") ? Convert.ToInt16(sOrgCode) : 0;
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            var areaDtl = _repositoryAreaDtl.FindBy(poi.Block_Code);
            poi.AreaId = areaDtl.AreaId;
            poi.CityId = areaDtl.CityId;
            poi.StateId = areaDtl.StateId;
            poi.CountryId = areaDtl.CountryId;
            poi.User_Code = principal.FindFirst("User_Code").Value;
            _repositoryPoi.Add(poi);

            var response = Request.CreateResponse(HttpStatusCode.Created, poi);
            //string uri = Url.Link("DefaultApi", new { id = poi.Id });
            //response.Headers.Location = new Uri(uri);
            return response;
        }

        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeletePoi(int id)
        {
            _repositoryPoi.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public IQueryable<Poi> GetPoiByPoiNo(int PoiNo)
        {
            Poi poidtl = null;
            return _repositoryPoi.QueryOver()
                .SelectList(list => list
                                        .Select(x => x.Id).WithAlias(() => poidtl.Id)
                                        .Select(x => x.Lati).WithAlias(() => poidtl.Lati)
                                        .Select(x => x.Longi).WithAlias(() => poidtl.Longi)
                                        .Select(x => x.PoiType).WithAlias(() => poidtl.PoiType))
                .Where(x => x.Id == PoiNo)
                .TransformUsing(Transformers.AliasToBean<Poi>()).List().AsQueryable();
        }

        public IQueryable<Poi> GetAllPoisByType(int? orgCode, string companyCode, int poiType)
        {
            Poi poidtl = null;
            var poiQueryOver = _repositoryPoi.QueryOver().Where(x => x.PoiTypeNo == poiType);
            if (orgCode != null) poiQueryOver.And(x => x.OrgCode == orgCode);
            if (companyCode != null) poiQueryOver.And(x => x.Company_Code == companyCode);

            return poiQueryOver.SelectList(list => list
                                        .Select(x => x.Id).WithAlias(() => poidtl.Id)
                                        .Select(x => x.Lati).WithAlias(() => poidtl.Lati)
                                        .Select(x => x.Longi).WithAlias(() => poidtl.Longi)
                                        .Select(x => x.PoiType).WithAlias(() => poidtl.PoiType))
                .TransformUsing(Transformers.AliasToBean<Poi>()).List().AsQueryable();
        }

        public dynamic GetPoiImagesByPoiNo(GridSettings grid, string sidx, string sord, int page, int rows, int poiNo)
        {
            PoiImage poiImage = null;
            UserProfile userProfile = null;
            int totalRecords;

            var poiImages = _repositoryPoiImage.QueryOver()
                .Where(x => x.PoiNo == poiNo)
                .JoinAlias(c => c.AddedByUser, () => userProfile)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => poiImage.Id)
                    .Select(x => x.Title).WithAlias(() => poiImage.Title)
                    .Select(x => x.Name).WithAlias(() => poiImage.Name)
                    .Select(x => x.ImagePath).WithAlias(() => poiImage.ImagePath)
                    .Select(x => x.ForAll).WithAlias(() => poiImage.ForAll)
                    .Select(x => x.AddedByUserCode).WithAlias(() => poiImage.AddedByUserCode)
                    .Select(() => userProfile.User_Name).WithAlias(() => poiImage.UserCode)

                ).TransformUsing(Transformers.AliasToBean<PoiImage>()).SearchGrid(grid, out totalRecords); //

            var pageSize = (rows == 0) ? poiImages.Count : rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from props in poiImages
                        select new
                        {
                            props.Id,
                            props.Title,
                            props.Name,
                            props.ImagePath,
                            props.ForAll,
                            //props.AddedByUserCode,
                            UserCode = props.AddedByUserCode + " - " + props.UserCode
                        })
            };
        }

        [HttpPut]
        public HttpResponseMessage EditPoiImage(int id, PoiImage poiImage)
        {
            poiImage.Id = id;
            var foundPoiImage = _repositoryPoiImage.FindBy(poiImage.Id);
            if (foundPoiImage == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            foundPoiImage.Name = poiImage.Name;
            foundPoiImage.Title = poiImage.Title;
            foundPoiImage.ImagePath = poiImage.ImagePath;
            foundPoiImage.ForAll = poiImage.ForAll;
            //foundPoiImage.AddedByUserCode = poiImage.AddedByUserCode;
            foundPoiImage.UserCode = principal.FindFirst("User_Code").Value;
            foundPoiImage.DmlDate = DateTime.Now;
            foundPoiImage.DmlType = "U";

            return new HttpResponseMessage(HttpStatusCode.OK);

        }

        [HttpPost]
        public HttpResponseMessage AddPoiImage(PoiImage poiImage)
        {
            string[] validFileTypes = { "bmp", "gif", "png", "jpg", "jpeg" };//, "doc", "xls" };
            var ext = Path.GetExtension(poiImage.ImagePath);
            var isValidFile = validFileTypes.Any(t => ext == "." + t);
            if (!isValidFile)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            //"Invalid File. Please upload a File with extension " + string.Join(",", validFileTypes);
            poiImage.AddedByUserCode = principal.FindFirst("User_Code").Value;
            poiImage.DmlType = "I";
            poiImage.DmlDate = DateTime.Now;
            poiImage.UserCode = principal.FindFirst("User_Code").Value;

            string imagePath = $"/Images/Organizations/{poiImage.OrgCode}";
            if (!string.IsNullOrEmpty(poiImage.CompanyCode))
                imagePath = $"{imagePath}/{poiImage.CompanyCode}";
            poiImage.ImagePath = $"{imagePath}/{DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + poiImage.ImagePath}";
            _repositoryPoiImage.Add(poiImage);

            return Request.CreateResponse(HttpStatusCode.OK, poiImage);
        }

        [HttpDelete]
        public HttpResponseMessage DeletePoiImage(int id)
        {
            var poiImage = _repositoryPoiImage.FindBy(id);
            if (poiImage == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            string filePath = poiImage.ImagePath;
            string foldername = filePath.Substring(0, filePath.LastIndexOf("/"));
            string fileName = filePath.Substring(filePath.LastIndexOf("/") + 1);
            //HttpContext.Current.Server.MapPath("");
            filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath(foldername), fileName);

            if (!File.Exists(filePath))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            File.Delete(filePath);

            _repositoryPoiImage.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public dynamic GetPoiImageAccessesByPoiImageNo(GridSettings grid, string sidx, string sord, int page, int rows, int poiImageNo)
        {
            PoiImageAccess poiImageAccess = null;
            UserProfile userProfile = null;
            int totalRecords;

            var poiImageAccesses = _repositoryPoiImageAccess.QueryOver()
                .Where(x => x.PoiImageNo == poiImageNo)
                .JoinAlias(c => c.User, () => userProfile)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => poiImageAccess.Id)
                    .Select(x => x.UserCode).WithAlias(() => poiImageAccess.UserCode)
                    .Select(() => userProfile.User_Name).WithAlias(() => poiImageAccess.UserCodeName)

                ).TransformUsing(Transformers.AliasToBean<PoiImageAccess>()).SearchGrid(grid, out totalRecords);

            var pageSize = (rows == 0) ? poiImageAccesses.Count : rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from props in poiImageAccesses
                        select new
                        {
                            props.Id,
                            props.UserCode,
                            UserCodeName = props.UserCode + " - " + props.UserCodeName
                        })
            };
        }

        [HttpPost]
        public HttpResponseMessage AddPoiImageAccess(PoiImageAccess poiImageAccess)
        {
            _repositoryPoiImageAccess.Add(poiImageAccess);
            return Request.CreateResponse(HttpStatusCode.Created, poiImageAccess);
        }

        [HttpDelete]
        public HttpResponseMessage DeletePoiImageAccess(int id)
        {
            _repositoryPoiImageAccess.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
