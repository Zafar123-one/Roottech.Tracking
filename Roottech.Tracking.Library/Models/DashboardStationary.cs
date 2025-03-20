namespace Roottech.Tracking.Library.Models
{
    public class DashboardStationary
    {
        public string Longi { get; set; }
        public string Lati { get; set; }
        public string EventName { get; set; }
        public string Id { get; set; }
        //public string Description { get; set; }
        public string SiteName { get; set; }
        public string AssetName { get; set; }
        public string RegionName { get; set; }
        public string AssetType { get; set; }
        public double? Capacity { get; set; }
        public int? BaseVolume { get; set; }
        public double? MinLevel { get; set; }
        public double? Currents { get; set; }
        public string LevelType { get; set; }
        public string TotDuration { get; set; }
    }
}