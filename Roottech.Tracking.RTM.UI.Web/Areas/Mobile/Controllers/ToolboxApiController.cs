using System.Linq;
using System.Web.Http;
using NHibernate;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.SMAA.Entities;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Mobile.Controllers
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

        [Queryable]
        [HttpGet]
        public IQueryable<ModDtl> GetAllToolBarItems()
        {
            ModDtl modDtl = null;
            return _repositoryModDtl.QueryOver()
                .Where(x => x.MomNo == 1)
                .SelectList(list => list
                    .Select(x => x.MomNo).WithAlias(() => modDtl.MomNo)
                    .Select(x => x.ObjCode).WithAlias(() => modDtl.ObjCode)
                    .Select(x => x.Description).WithAlias(() => modDtl.Description))
                .TransformUsing(Transformers.AliasToBean<ModDtl>()).List().AsQueryable();
        }

        [Queryable]
        [HttpGet]
        public IQueryable<ModAccessByUserAndMod> GetModAccess()
        {
            var sUserCode = SystemLibrary.GetCookies("User_Code");
            return _session.GetNamedQuery("GetModAccessByUserAndMod").SetString("User_Code", (sUserCode != "%") ? sUserCode : "").SetString("ModuleNo", "2")
                .List<ModAccessByUserAndMod>().AsQueryable();
        }
    }
}

