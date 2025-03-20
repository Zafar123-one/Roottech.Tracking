using System;
using Roottech.Tracking.Common.Entities;

namespace Roottech.Tracking.Domain.RSSP.Entities
{
    public class RptAssetOperation : IKeyed<int>
    {
        //exec SP_Rpt_AssetOperation @fromdate='2016-03-01',@todate='2016-03-03',@Region='%',@City='%',@Territory='%',@SiteCode='1814',@Orgcode=33,@Resource='131',@EventType='C',@levelTYpe='D'
        public virtual int Id { get; set; }
        public virtual string RegionName { get; set; }
        public virtual string CityName { get; set; }
        //public virtual string Territory { get; set; }
        public virtual string SiteName { get; set; }
        public virtual string AssetName { get; set; }
        public virtual DateTime OpenDt { get; set; }
        public virtual DateTime CloseDt { get; set; }
        public virtual string AssetTotalDuration { get; set; }
        public virtual decimal Consumption { get; set; }
        public virtual int Hour { get; set; }
        public virtual int Minute { get; set; }
        public virtual int Second { get; set; }
    }
}