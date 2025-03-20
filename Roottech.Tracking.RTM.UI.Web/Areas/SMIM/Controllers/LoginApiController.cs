using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using NHibernate;
using NHibernate.Transform;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSPT.Entities;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.Domain.SMSE.Entities;
using Roottech.Tracking.Library.Models.Grid;
using Roottech.Tracking.Library.Models.Helpers;

namespace Roottech.Tracking.RTM.UI.Web.Areas.SMIM.Controllers
{
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

        public Login Get(string userId, string password)
        {
            string ip = LocalIPAddress();
            Login loginInfo = _session.GetNamedQuery("GetLogin").SetString("Id", userId).SetString("Password", HashPass(password)).SetString("UserIP", ip).UniqueResult<Login>();

            if (!string.IsNullOrEmpty(loginInfo.Msg))
            {
                if (loginInfo.Msg.Split(';')[0] == "0")
                    _session.Transaction.Rollback();
            }
            return loginInfo;
        }
        
        public string Get(int id)
        {
            return "value";
        }

        [System.Web.Mvc.HttpGet]
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

        private string HashPass(string password)
        {
            //return password;
/*            var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var encoder = new System.Text.ASCIIEncoding();
            byte[] combined = encoder.GetBytes(password);
            string hash = BitConverter.ToString(sha.ComputeHash(combined)).Replace("-", "");*/
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
        }

        public string LocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string localIP = "";
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily.ToString() == "InterNetwork")  
                    localIP = ip.ToString();
            return localIP;
        }

        [Queryable]
        [System.Web.Mvc.HttpGet]
        public IQueryable<OrganizationsByUserCode> GetOrganizationsModWise(string userCode, byte appType)
        {
            return _session.GetNamedQuery("GetOrganizationsModWise")
                .SetString("User_Code", userCode)
                .SetInt16("AppType", (short)(appType==1?AppType.Stationary: AppType.Mobile))//SystemLibrary.GetCookies("User_Code"))
                .List<OrganizationsByUserCode>().AsQueryable();
            /*Organization organization = null;
            return _repositoryOrganization.QueryOver()
                .SelectList( list => list
                .Select(x => x.Id).WithAlias(() => organization.Id)
                .Select(x => x.DSCR).WithAlias(() => organization.DSCR))
                .TransformUsing(Transformers.AliasToBean<Organization>()).List().AsQueryable();*/
        }

        public IList<FirstLogin> GetFirstLogin(string userId, string Oldpassword, string NewPassword, string UserFAQ1, string UserAns1, string UserFAQ2, string UserAns2, string LoginCode)
        {
            return _session.GetNamedQuery("GetFirstLogin")
                .SetString("Id", userId)
                .SetString("OldPassword", HashPass(Oldpassword))
                .SetString("NewPassword", HashPass(NewPassword)) 
                .SetString("UserFAQ1", UserFAQ1)
                .SetString("UserAns1", UserAns1)
                .SetString("UserFAQ2", UserFAQ2)
                .SetString("UserAns2", UserAns2)
                .SetString("LoginCode", LoginCode) 
                .List<FirstLogin>();
        }

        public ForgotPassword GetForgotPassword(string Id, string User_FAQ1, string User_Ans1, string User_FAQ2, string User_Ans2)
        {
            var newPassword = GeneratePassword();
            var forgotPassword = _session.GetNamedQuery("GetForgotPassword")
                .SetString("Id", Id)
                .SetString("User_FAQ1", User_FAQ1)
                .SetString("User_Ans1", User_Ans1)
                .SetString("User_FAQ2", User_FAQ2)
                .SetString("User_Ans2", User_Ans2)
                .SetString("New_Password", HashPass(newPassword))
                .UniqueResult<ForgotPassword>();

            return new ForgotPassword(){Id = (forgotPassword.Id != "0" ? newPassword : "0"), Msg = forgotPassword.Msg};
        }

        public string[] getSecutiryFAQs()
        {
            return Enum.GetNames(typeof (SecutiryFAQ));
        }

        public enum SecutiryFAQ
        {
            Whats_is_your_name,
            Whats_is_your_father_name,
            What_is_your_Nick_Name,
            Who_is_your_Best_Friend
        }

        private string GeneratePassword()
        {
            string Pass = DateTime.Now.DayOfWeek.ToString().Substring(0,1) ;
            int Dt = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            Dt = Dt * 10;
            Pass = Pass + Dt.ToString().Substring(0, 2);
            Dt = Dt / 5;
            Pass = Pass + Dt.ToString().Substring(Dt.ToString().Length - 2, 2);
            Dt = Convert.ToInt32(DateTime.Now.ToString("hhmm"));
            Pass = Pass + Dt.ToString() +  (DateTime.Now.DayOfWeek.ToString().Substring(DateTime.Now.DayOfWeek.ToString().Length - 1 , 1)).ToUpper();
            return Pass;
        }

        [Queryable]
        public IQueryable<ResourceType> GetVehicleTree(int OrgCode, string UserCode)
        {
            return _repositoryResourceType.FilterBy(x => x.OrgCode == OrgCode);
        }

    }
}
