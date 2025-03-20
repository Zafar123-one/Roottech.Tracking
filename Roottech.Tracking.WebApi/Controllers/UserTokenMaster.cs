using System;

namespace Roottech.Tracking.WebApi.Controllers
{
    public class UserTokenMaster
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiredTime { get; set; }
    }
}