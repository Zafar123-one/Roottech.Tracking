using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities {
    
    public class UserProfile : IKeyed<string> {
        public virtual string Id { get; private set; } //User_Code
        public virtual string User_Name { get; set; }
        public virtual string User_Password { get; set; }
        public virtual string User_Faq1 { get; set; }
        public virtual string User_Ans1 { get; set; }
        public virtual string User_Faq2 { get; set; }
        public virtual string User_Ans2 { get; set; }
        public virtual DateTime? User_DateCreated { get; set; }
        public virtual DateTime? User_DateExpired { get; set; }
        public virtual string AppEnv_Code { get; set; }
        public virtual string TimeZone_Code { get; set; }
        public virtual string UsrStatusCode { get; set; }
        public virtual string SAC_Code { get; set; }
        public virtual string login_type { get; set; }
        public virtual string login_code { get; set; }
        public virtual double? wrong_pass { get; set; }
        public virtual DateTime? login_date { get; set; }
        public virtual int Orgcode { get; set; }
    }
}
