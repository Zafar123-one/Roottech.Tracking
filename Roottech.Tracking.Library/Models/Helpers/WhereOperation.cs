namespace Roottech.Tracking.Library.Models.Helpers
{
    /// <summary>
    /// The supported operations in where-extension
    /// </summary>
    public enum WhereOperation
    {
        [StringValue("eq")]Equal,
        [StringValue("ne")]NotEqual,
        [StringValue("lt")]Less,
        [StringValue("le")]LessEqual,
        [StringValue("gt")]Greater,
        [StringValue("ge")]GreaterOrEqual,
        [StringValue("bw")]BeginsWith,
        [StringValue("bn")]NotBeginsWith,
        [StringValue("in")]IsIn,
        [StringValue("ni")]IsNotIn,
        [StringValue("ew")]EndsWith,
        [StringValue("en")]NotEndsWith,
        [StringValue("cn")]Contains,
        [StringValue("nc")]NotContains,
        [StringValue("nu")]Null,
        [StringValue("nn")]NotNull
    }
}