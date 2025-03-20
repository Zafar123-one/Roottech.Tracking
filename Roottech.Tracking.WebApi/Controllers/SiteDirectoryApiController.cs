using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class SiteDirectoryApiController : ApiController
    {
        private readonly IKeyedRepository<int, SiteDirectory> _repositorySiteDirectory;
        // GET api/sitedirectoryapi
        public SiteDirectoryApiController(IKeyedRepository<int, SiteDirectory> repositorySiteDirectory)
        {
            _repositorySiteDirectory = repositorySiteDirectory;
        }

/*        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        public IList<SiteDirectory> GetSiteDirectoriesBySiteCode(int siteCode)
        {
            SiteDirectory siteDirectory = null;
            return _repositorySiteDirectory.QueryOver()
                .Where(x => x.SiteCode == siteCode)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => siteDirectory.Id)
                    .Select(x => x.SiteCode).WithAlias(() => siteDirectory.SiteCode)
                    .Select(x => x.Contname).WithAlias(() => siteDirectory.Contname)
                    .Select(x => x.Desig).WithAlias(() => siteDirectory.Desig)
                    .Select(x => x.Cell).WithAlias(() => siteDirectory.Cell)
                    .Select(x => x.Directphone).WithAlias(() => siteDirectory.Directphone)
                    .Select(x => x.Extention).WithAlias(() => siteDirectory.Extention)
                    .Select(x => x.Email).WithAlias(() => siteDirectory.Email)
                    .Select(x => x.Salutation).WithAlias(() => siteDirectory.Salutation)
                    .Select(x => x.Status).WithAlias(() => siteDirectory.Status)
                ).TransformUsing(Transformers.AliasToBean<SiteDirectory>()).List();
        }

        // GET api/sitedirectoryapi/5
/*        public string Get(int id)
        {
            return "value";
        }*/

        // POST api/sitedirectoryapi
        public HttpResponseMessage Post(SiteDirectory siteDirectory)
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            siteDirectory.UserCode = principal.FindFirst("User_Code").Value;
            siteDirectory.DmlType = "I";
            siteDirectory.DmlDate = DateTime.Now;
            _repositorySiteDirectory.Add(siteDirectory);

            return Request.CreateResponse(HttpStatusCode.Created, siteDirectory); //return siteDirectory;
        }

        // PUT api/sitedirectoryapi/5
        public HttpResponseMessage Put(SiteDirectory siteDirectory)
        {
            //siteDirectory.Id = id;
            var foundPoi = _repositorySiteDirectory.FindBy(siteDirectory.Id);
            if (foundPoi == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            siteDirectory.UserCode = principal.FindFirst("User_Code").Value;
            siteDirectory.DmlType = "U";
            siteDirectory.DmlDate = DateTime.Now;
            _repositorySiteDirectory.Merge(siteDirectory);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE api/sitedirectoryapi/5
        public HttpResponseMessage Delete(int id)
        {
            //_repositorySiteDirectory.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
