using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.Domain.SMIM.Mappings {
    
    public class UserProfileMap : ClassMapping<UserProfile> {
        
        public UserProfileMap() {
			Table("SMIM_UserProfiile_ST");
			Lazy(false);
			Id(x => x.Id, map => map.Column("User_Code") );
			Property(x => x.User_Name, map => map.Length(25));
			Property(x => x.User_Password, map => map.Length(100));
			Property(x => x.User_Faq1, map => map.Length(40));
			Property(x => x.User_Ans1, map => map.Length(40));
			Property(x => x.User_Faq2, map => map.Length(40));
			Property(x => x.User_Ans2, map => map.Length(40));
			Property(x => x.User_DateCreated);
			Property(x => x.User_DateExpired);
			Property(x => x.AppEnv_Code, map => map.Length(10));
			Property(x => x.TimeZone_Code, map => map.Length(10));
			Property(x => x.UsrStatusCode, map => map.Length(10));
			Property(x => x.SAC_Code, map => map.Length(10));
			Property(x => x.login_type, map => map.Length(1));
			Property(x => x.login_code, map => map.Length(30));
			Property(x => x.wrong_pass);
			Property(x => x.login_date);
            Property(x => x.Orgcode, map => map.Precision(18));
        }
    }
}
