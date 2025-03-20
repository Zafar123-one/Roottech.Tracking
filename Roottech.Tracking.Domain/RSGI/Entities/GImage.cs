using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class GImage : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string Title { get; set; }
        public virtual string Iname { get; set; }
        //public virtual string GImage { get; set; }
        public virtual string ImgPath { get; set; }
        public virtual string User_Code { get; set; }
    }
}
