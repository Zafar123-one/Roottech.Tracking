using System;

namespace Roottech.Tracking.Library.Models
{
    public class DashboardStationaryDetail
    {
        public string SiteName { get; set; }
        public string AssetName { get; set; }
        public string RegionName { get; set; }
        public string AssetType { get; set; }
        public double? Capacity { get; set; }
        public int? BaseVolume { get; set; }
        public double? MinLevel { get; set; }
        public double? Currents { get; set; }
        public string LevelType { get; set; }
        public string EventName { get; set; }
        public string TotDuration { get; set; }
        public double? Dgcap { get; set; }
        public DateTime? ActivationDt { get; set; }
    }
}