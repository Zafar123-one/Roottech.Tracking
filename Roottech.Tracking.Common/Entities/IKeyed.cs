namespace Roottech.Tracking.Common.Entities
{
    public interface IKeyed<TKey>
    {
        TKey Id { get; }    
    }
}