using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class PoiType: IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string Company_Code { get; set; }
        public virtual string Title { get; set; }
        public virtual string TypeName { get; set; }
        public virtual int GImageNo { get; set; }
        public virtual GImage GImage { get; set; }
        public virtual string User_Code { get; set; }
    }
}