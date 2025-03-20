using System.Collections.Generic;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities
{
    public class VehicleTree : IKeyed<string>
    {
        public virtual string Id { get; set; } //User_Code
        //public virtual string OrgName { get; set; }
        //public virtual IList<ResourceType> ResourceTypes { get; set; }
    }
    /*
    public class ResourceType
    {
        public virtual string ResourceTypeName { get; set; }
        public virtual IList<Title> Titles { get; set; }
    }
    */
    public class Title
    {
        public virtual string TitleName { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<PlateId> PlateIds { get; set; }
    }

    public class PlateId
    {
        public virtual string PlateIdName { get; set; }
    }
}
