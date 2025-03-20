using System.Web;

namespace Roottech.Tracking.Library.Utils
{
    public static class SystemLibrary
    {
        public static string GetCookies(string cName)
        {
            string sOrgCode = "";
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[cName];
            if (myCookie != null)
            {
                sOrgCode = myCookie.Values[cName];
            }
            return sOrgCode;
        } 
    }
}