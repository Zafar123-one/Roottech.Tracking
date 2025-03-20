using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Http;
using NHibernate;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.Models.Helpers;
using Roottech.Tracking.WebApi.Infrastructure.Models;

namespace Roottech.Tracking.WebApi.Controllers
{
    [Authorize]
    public class LoginApiController : ApiController
    {
        private readonly IKeyedRepository<string, UserProfile> _repository;
        private readonly IKeyedRepository<int, Organization> _repositoryOrganization;
        private readonly IKeyedRepository<int, ResourceType> _repositoryResourceType;

        private readonly ISession _session;

        public LoginApiController(ISessionFactory sessionFactory, IKeyedRepository<string, UserProfile> repository, IKeyedRepository<int, Organization> repositoryOrganization, IKeyedRepository<int, ResourceType> repositoryResourceType)
        {
            _repository = repository;
            _repositoryOrganization = repositoryOrganization;
            _repositoryResourceType = repositoryResourceType;
            _session = sessionFactory.GetCurrentSession();
        }

        [HttpPost]
        public HttpResponseMessage Login(FormDataCollection formData)// string id)
        {
            var loginInfo = _session.GetNamedQuery("GetLogin").SetString("Id", formData.Get("id")).SetString("Password", CommonFunctions.HashPass(Request.Headers.GetValues("test").First())).SetString("UserIP", CommonFunctions.LocalIPAddress()).UniqueResult<Login>();

            var respMessage = new HttpResponseMessage {Content = new ObjectContent<LoginResponse>(new LoginResponse() { message = loginInfo.Msg, data = loginInfo }, new JsonMediaTypeFormatter()) };
            if (!string.IsNullOrEmpty(loginInfo.Msg))
            {
                if (loginInfo.Msg.Split(';')[0] == "0")
                {
                    _session.Transaction.Rollback();
                    respMessage.StatusCode = HttpStatusCode.NotFound;
                }
                else if (loginInfo.Msg.Split(';')[0] == "2")
                    respMessage.StatusCode = HttpStatusCode.Forbidden;
                else if (loginInfo.Msg.Split(';')[0] == "5" || loginInfo.Msg.Split(';')[0] == "7")
                        respMessage.StatusCode = HttpStatusCode.Unauthorized;
                else
                    respMessage.StatusCode = HttpStatusCode.NotAcceptable;
            }
            else
                respMessage.StatusCode = HttpStatusCode.OK;

            var cookie = new CookieHeaderValue("session-id", new NameValueCollection { { "User_Code", formData.Get("id") }, { "UserOrgCode", "39" } })
            {
                Expires = DateTime.Now.AddDays(365),
                Domain = Request.RequestUri.Host,
                Path = "/"
            };
            respMessage.Headers.AddCookies(new[] { cookie });
            return respMessage;
        }
        public string Get(int id)
        {
            return "value";
        }

        //[System.Web.Mvc.HttpGet]
        public dynamic GetUserProfile(GridSettings grid, string sidx, string sord, int page, int rows, string userid)
        {
            int totalRecords;
            var expressions = new List<Expression<Func<UserProfile, object>>> { x => x.Id, x => x.User_Name, x => x.User_DateCreated, x => x.User_DateExpired };
            var userProfiles = _repository.QueryOver(expressions).SearchGrid(grid, out totalRecords);

            var pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            return new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = userProfiles
            };
        }

        //[Queryable]
        //[System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetOrganizationsModWise(string userCode)
        {
            var respMessage = new HttpResponseMessage();
            var principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            AppType appType;// = Convert.ToInt16();
            Enum.TryParse(principal.FindFirst("AppType").Value, out appType);

            var organizationsByUserCodes = _session.GetNamedQuery("GetOrganizationsModWise")
                .SetString("User_Code", userCode)
                .SetInt16("AppType", (short)appType)//(short)AppType.Stationary)
                .List<OrganizationsByUserCode>();
            if (organizationsByUserCodes.Any())
                respMessage.Content = new ObjectContent<IList<OrganizationsByUserCode>>(organizationsByUserCodes, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
            /*Organization organization = null;
            return _repositoryOrganization.QueryOver()
                .SelectList( list => list
                .Select(x => x.Id).WithAlias(() => organization.Id)
                .Select(x => x.DSCR).WithAlias(() => organization.DSCR))
                .TransformUsing(Transformers.AliasToBean<Organization>()).List().AsQueryable();*/
        }

        public HttpResponseMessage GetFirstLogin(string userId, string Oldpassword, string NewPassword, string UserFAQ1, string UserAns1, string UserFAQ2, string UserAns2, string LoginCode)
        {
            var respMessage = new HttpResponseMessage();
            var firstLogins = _session.GetNamedQuery("GetFirstLogin")
                .SetString("Id", userId)
                .SetString("OldPassword", CommonFunctions.HashPass(Oldpassword))
                .SetString("NewPassword", CommonFunctions.HashPass(NewPassword))
                .SetString("UserFAQ1", UserFAQ1)
                .SetString("UserAns1", UserAns1)
                .SetString("UserFAQ2", UserFAQ2)
                .SetString("UserAns2", UserAns2)
                .SetString("LoginCode", LoginCode)
                .List<FirstLogin>();
            if (firstLogins.Any())
                respMessage.Content = new ObjectContent<IList<FirstLogin>>(firstLogins, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public HttpResponseMessage GetForgotPassword(string Id, string User_FAQ1, string User_Ans1, string User_FAQ2, string User_Ans2)
        {
            var respMessage = new HttpResponseMessage();

            var newPassword = CommonFunctions.GeneratePassword();
            var forgotPassword = _session.GetNamedQuery("GetForgotPassword")
                .SetString("Id", Id)
                .SetString("User_FAQ1", User_FAQ1)
                .SetString("User_Ans1", User_Ans1)
                .SetString("User_FAQ2", User_FAQ2)
                .SetString("User_Ans2", User_Ans2)
                .SetString("New_Password", CommonFunctions.HashPass(newPassword))
                .UniqueResult<ForgotPassword>();

            if (forgotPassword != null)
                respMessage.Content = new ObjectContent<ForgotPassword>(
                    new ForgotPassword() { Id = (forgotPassword.Id != "0" ? newPassword : "0"), Msg = forgotPassword.Msg }, new JsonMediaTypeFormatter());
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No data available for the request.");
            return respMessage;
        }

        public string[] getSecutiryFAQs()
        {
            return Enum.GetNames(typeof (SecutiryFAQ));
        }

        private enum SecutiryFAQ
        {
            Whats_is_your_name,
            Whats_is_your_father_name,
            What_is_your_Nick_Name,
            Who_is_your_Best_Friend
        }

        //[Queryable]
        public IQueryable<ResourceType> GetVehicleTree(int OrgCode, string UserCode)
        {
            return _repositoryResourceType.FilterBy(x => x.OrgCode == OrgCode);
        }

        [HttpPost]
        public HttpResponseMessage ChangePassword(FormDataCollection formData)
        {
            var userProfile = _repository.FindBy(formData.Get("id"));
            if (userProfile == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            if (userProfile.User_Password != CommonFunctions.HashPass(formData.Get("oldpass")))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));

            userProfile.User_Password = CommonFunctions.HashPass(Request.Headers.GetValues("test").First());
            _repository.Merge(userProfile);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}

public class LoginResponse
{
    public string message { get; set; }
    public Login data { get; set; }
}