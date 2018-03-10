
namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Represents an event that occurs after the model is prepared for view
    /// </summary>
    /// <typeparam name="T">Type of the model</typeparam>
    public class ModelPrepared<T>
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="model">Model</param>
        public ModelPrepared(T model)
        {
            this.Model = model;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a model
        /// </summary>
        public T Model { get; private set; }

        #endregion
    }
}
