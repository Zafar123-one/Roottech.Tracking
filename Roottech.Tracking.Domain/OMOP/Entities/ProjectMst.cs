using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.OMOP.Entities {
    
    public class ProjectMst : IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual string ContrType { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual double? ORGCODE { get; set; }
        public virtual string Company_code { get; set; }
        public virtual string ComdtyCD { get; set; }
        public virtual string CalendarID { get; set; }
        public virtual double? Contract { get; set; }
        public virtual double? PRJMGREmp { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual double? OPMGREmp { get; set; }
        public virtual DateTime? FromDate { get; set; }
        public virtual DateTime? Todate { get; set; }
        public virtual string User_Code { get; set; }
        public virtual string DML_Type { get; set; }
        public virtual DateTime? DML_Date { get; set; }
    }
}
