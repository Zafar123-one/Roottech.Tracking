namespace Roottech.Tracking.Library.Models
{
    /// <summary>
    /// Class which represents node for jQuery TreeView plugin.
    /// </summary>
    public class TreeViewNode
    {
        #region Properties
        public string id { get; set; }
        public string text { get; set; }
        public bool expanded { get; set; }
        public bool hasChildren { get; set; }
        public string classes { get; set; }
        public string fullName { get; set; }
        public byte level { get; set; }
        public TreeViewNode[] children { get; set; }
        #endregion
    }
}