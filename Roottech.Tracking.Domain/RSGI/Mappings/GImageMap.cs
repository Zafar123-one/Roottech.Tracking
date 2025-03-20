using NHibernate.Mapping.ByCode.Conformist;
using Roottech.Tracking.Domain.RSGI.Entities;

namespace Roottech.Tracking.Domain.RSGI.Mappings
{
    public class GImageMap : ClassMapping<GImage>
    {
        public GImageMap()
        {
            Table("RSGI_Images_ST");
            Lazy(false);
            Id(x => x.Id, map => map.Column("GImage#"));
            Property(x => x.OrgCode);
            Property(x => x.Title);
            Property(x => x.Iname);
            //Property(x => x.GImage);
            Property(x => x.ImgPath);
            Property(x => x.User_Code);
        }
    }
}