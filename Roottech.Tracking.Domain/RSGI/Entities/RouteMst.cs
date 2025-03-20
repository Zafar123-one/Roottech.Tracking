using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSGI.Entities
{
    public class RouteMst : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual int OrgCode { get; set; }
        public virtual string Company_Code { get; set; }
        public virtual string Title { get; set; }
        public virtual string RouteName { get; set; }
        public virtual string Comments { get; set; }
        public virtual string LineColor { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime DML_Date { get; set; }
    }
}