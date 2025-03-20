using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSPT.Entities
{
    public class SiteDirectory : IKeyed<int>
    {
        public virtual int Id { get; set; }
        public virtual double? SiteCode { get; set; }
        public virtual string Contname { get; set; }
        public virtual string Desig { get; set; }
        public virtual string Cell { get; set; }
        public virtual string Directphone { get; set; }
        public virtual string Extention { get; set; }
        public virtual string Email { get; set; }
        public virtual string UserCode { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string Salutation { get; set; }
        public virtual string Status { get; set; }
    }
}