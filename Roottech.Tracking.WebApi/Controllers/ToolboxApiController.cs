using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web.Http;
using NHibernate;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.SMAA.Entities;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class ToolboxApiController : ApiController
    {
        private readonly IKeyedRepository<int, ModDtl> _repositoryModDtl;
        private readonly ISession _session;

        public ToolboxApiController(IKeyedRepository<int, ModDtl> repositoryModDtl, ISessionFactory sessionFactory)
        {
            _repositoryModDtl = repositoryModDtl;
            _session = sessionFactory.GetCurrentSession();
        }

        [HttpGet]
        public HttpResponseMessage GetAllToolBarItems()
        {
            ModDtl modDtl = null;
            var allToolBarItems = _repositoryModDtl.QueryOver()
                .Where(x => x.MomNo == 1)
                .SelectList(list => list
                    .Select(x => x.MomNo).WithAlias(() => modDtl.MomNo)
                    .Select(x => x.ObjCode).WithAlias(() => modDtl.ObjCode)
                    .Select(x => x.Description).WithAlias(() => modDtl.Description))
                .TransformUsing(Transformers.AliasToBean<ModDtl>()).List();

            var respMessage = new HttpResponseMessage { Content = new ObjectContent<IList<ModDtl>>(allToolBarItems, new JsonMediaTypeFormatter()) };
            respMessage.StatusCode = (allToolBarItems.Any()) ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return respMessage;
        }

        [HttpGet]
        public HttpResponseMessage GetModAccess()
        {
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            var sUserCode = principal.FindFirst("User_Code").Value;

            var modAccessByUserAndMods = _session.GetNamedQuery("GetModAccessByUserAndMod").SetString("User_Code", (sUserCode != "%") ? sUserCode : "").SetString("ModuleNo", "2")
                .List<ModAccessByUserAndMod>();

            var respMessage = new HttpResponseMessage { Content = new ObjectContent<IList<ModAccessByUserAndMod>>(modAccessByUserAndMods, new JsonMediaTypeFormatter()) };
            respMessage.StatusCode = (modAccessByUserAndMods.Any()) ? HttpStatusCode.OK : HttpStatusCode.NotFound;
            return respMessage;
        }
    }
}