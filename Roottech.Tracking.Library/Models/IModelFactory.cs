namespace Roottech.Tracking.Library.Models
{
    public interface IModelFactory
    {
        /// <summary>
        /// Creates a new model
        /// </summary>
        TModel CreateModel<TModel>();

        /// <summary>
        /// Releases the instance to get 
        /// garbage collected
        /// </summary>
        void FreeUpModel(object oldModel);
    }
}