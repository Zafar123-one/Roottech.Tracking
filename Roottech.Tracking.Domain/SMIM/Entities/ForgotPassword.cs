using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities
{
    public class ForgotPassword : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string Msg { get; set; }

    }
}
