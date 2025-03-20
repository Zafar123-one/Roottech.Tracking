using System;
using System.Collections.Generic;
using Roottech.Tracking.Common.Entities;
using Roottech.Tracking.Domain.OMOP.Entities;

namespace Roottech.Tracking.Domain.RSMT.Entities {
    
    public class ContractMst :IKeyed<double>
    {
        public virtual double Id { get; set; }
        public virtual DateTime? ContractDate { get; set; }
        public virtual string Title { get; set; }
        public virtual double? ContrStatusNo { get; set; }
        public virtual string MasterContractNo { get; set; }
        public virtual string ContDscr { get; set; }
        public virtual DateTime? DateFrom { get; set; }
        public virtual string OrgCode { get; set; }
        public virtual DateTime? DateTo { get; set; }
        public virtual string Comdtycd { get; set; }
        public virtual double? VendorCode { get; set; }
        public virtual string ContrType { get; set; }
        public virtual string ToVendorCode { get; set; }
        public virtual string CompanyCode { get; set; }
        public virtual string UserCode { get; set; }
        public virtual string ToLeranceType { get; set; }
        public virtual double? ToLeranceQty { get; set; }
        public virtual string DmlType { get; set; }
        public virtual DateTime? DmlDate { get; set; }
        public virtual string RWithFuel { get; set; }
        public virtual string FuelRateType { get; set; }
        public virtual double? FixedRate { get; set; }
        public virtual string ServChargeType { get; set; }
        public virtual double? ServChargeRate { get; set; }
        public virtual string EmerChargeType { get; set; }
        public virtual double? EmerChargeRate { get; set; }
        public virtual double? ExpiryAlertB4 { get; set; }
        public virtual DateTime? ExpiryAlertDt { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual IList<ContSite> ContSites { get; set; }
        public virtual IList<ProjectMst> ProjectMsts { get; set; }
        
    }
}
