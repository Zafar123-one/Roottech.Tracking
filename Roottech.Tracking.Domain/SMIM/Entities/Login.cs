using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.SMIM.Entities
{
    public class Login : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual string User_Name { get; set; }
        public virtual string Msg { get; set; }
        public virtual string RegionId { get; set; }
        public virtual string OrgCode { get; set; }
        public virtual string OrgName { get; set; }
        public virtual short GroupCount { get; set; }
        public virtual string UGRP_Code { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual AppType? AppType { get; set; }
    }

    public enum AppType
    {
        All,
        Stationary,
        Mobile
    }
}