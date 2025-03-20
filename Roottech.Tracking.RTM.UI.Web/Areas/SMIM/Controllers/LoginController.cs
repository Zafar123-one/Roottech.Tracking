using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.RTM.UI.Web.Areas.SMIM.Controllers
{
    //[RequireHttps]
    public class LoginController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Login()//string userId, string password)async Task<ActionResult>
        {
            try
            {
				var client = new HttpClient();
				var userId = Request.Form["UserId"];
				var password = Request.Form["Password"];
				var requestUri = $"{Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "")}{@Url.HttpRouteUrl("Area_defaultApi", new { controller = "LoginApi", action = "Get" })}?userId={userId}&password={password}";
				//var requestUri = $"{Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "")}?userId={userId}&password={password}";
				//requestUri = $"{Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "")}/api/SMIM/loginapi?userId=103&password=N@@ruddin";
				//var response = await client.GetAsync(requestUri);

				Login login = null;
				var httpClient = new HttpClient();
				HttpResponseMessage response = httpClient.GetAsync(requestUri).Result;
				if (response.IsSuccessStatusCode)
				{
					//string stateInfo = 
					login = response.Content.ReadAsAsync<Login>().Result;//_loginApiController.Get(userId, password); //await response.Content.ReadAsAsync<Login

					if (string.IsNullOrEmpty(login.Msg))
					{
						
						//HttpContext..Current.User
						FormsAuthentication.SetAuthCookie(login.User_Name + " - " + login.OrgName, true);
						//Above line not working in 4.5.2 thats why used below one http://stackoverflow.com/questions/11882762/formsauthentication-setauthcookie-throwing-nullreferenceexception-in-async-actio
						Response.Cookies.Add(FormsAuthentication.GetAuthCookie(login.User_Name + " - " + login.OrgName, true));

						SetCookies("loginOrgCode", login.GroupCount > 0 ? "%" : login.Id.ToString());
						SetCookies("User_Code", userId);
						SetCookies("UserOrgCode", login.OrgCode);
						SetCookies("AppType", login.AppType.ToString());
					}
				}
				return View("Index", login);
			}
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void SetCookies(string cName, string cValue)
        {
            HttpCookie myCookie = Request.Cookies[cName] ?? new HttpCookie(cName);
            myCookie.Values[cName] = cValue;
            myCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(myCookie);
        }
    }
}
