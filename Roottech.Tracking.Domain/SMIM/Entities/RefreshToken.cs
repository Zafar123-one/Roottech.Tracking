using System;
using Roottech.Tracking.Common.Entities;


namespace Roottech.Tracking.Domain.SMIM.Entities {
    
    public class RefreshToken: IKeyed<string> {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string ClientId { get; set; }
        public virtual DateTime? IssuedTime { get; set; }
        public virtual DateTime? ExpiredTime { get; set; }
        public virtual string ProtectedTicket { get; set; }
    }
}
