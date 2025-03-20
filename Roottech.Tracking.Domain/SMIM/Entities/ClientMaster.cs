using Roottech.Tracking.Common.Entities;


namespace Roottech.Tracking.Domain.SMIM.Entities {
    
    public class ClientMaster : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string ClientId { get; set; }
        public virtual string ClientSecret { get; set; }
        public virtual string ClientName { get; set; }
        public virtual bool Active { get; set; }
        public virtual int RefreshTokenLifeTime { get; set; }
        public virtual string AllowedOrigin { get; set; }
    }
}
