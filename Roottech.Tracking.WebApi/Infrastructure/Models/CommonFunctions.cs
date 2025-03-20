using System;
using System.Net;
using System.Web.Security;

namespace Roottech.Tracking.WebApi.Infrastructure.Models
{
    public static class CommonFunctions
    {
        public static string GeneratePassword()
        {
            string Pass = DateTime.Now.DayOfWeek.ToString().Substring(0, 1);
            int Dt = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            Dt = Dt * 10;
            Pass = Pass + Dt.ToString().Substring(0, 2);
            Dt = Dt / 5;
            Pass = Pass + Dt.ToString().Substring(Dt.ToString().Length - 2, 2);
            Dt = Convert.ToInt32(DateTime.Now.ToString("hhmm"));
            Pass = Pass + Dt.ToString() + (DateTime.Now.DayOfWeek.ToString().Substring(DateTime.Now.DayOfWeek.ToString().Length - 1, 1)).ToUpper();
            return Pass;
        }

        public static string HashPass(string password)
        {
            //return password;
            /*            var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                        var encoder = new System.Text.ASCIIEncoding();
                        byte[] combined = encoder.GetBytes(password);
                        string hash = BitConverter.ToString(sha.ComputeHash(combined)).Replace("-", "");*/
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, "sha1");
        }

        public static string LocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            string localIP = "";
            foreach (IPAddress ip in host.AddressList)
                if (ip.AddressFamily.ToString() == "InterNetwork")
                    localIP = ip.ToString();
            return localIP;
        }
    }
}