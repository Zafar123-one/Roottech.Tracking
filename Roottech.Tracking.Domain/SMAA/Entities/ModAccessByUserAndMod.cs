using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMAA.Entities
{
    public class ModAccessByUserAndMod : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string ObjCode { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual char Allow { get; set; }
        public virtual int MomNo { get; set; }
        //public ModMst ModMst { get; set; }
    }
}