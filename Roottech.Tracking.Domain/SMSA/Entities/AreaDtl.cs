using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMSA.Entities
{
    public class AreaDtl : IKeyed<string>
    {
        public virtual string Id { get; set; }
        public virtual string AreaId { get; set; }
        public virtual string CityId { get; set; }
        public virtual string StateId { get; set; }
        public virtual string CountryId { get; set; }
        public virtual string Block_Title { get; set; }
        public virtual string Block_Description { get; set; }
        public virtual string ORGCode { get; set; }
        public virtual string User_Code { get; set; }
    }
}