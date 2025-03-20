using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSE.Entities
{
    public class Organization : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string DFLT { get; set; }
        public virtual string DSCR { get; set; }
        public virtual int CONTACT { get; set; }
        public virtual int CalendarID { get; set; }
        public virtual int ParentOrg { get; set; }
        public virtual string Title { get; set; }
    }
}