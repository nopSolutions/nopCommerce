using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Represents an event that occurs after the model is received from the view
    /// </summary>
    /// <typeparam name="T">Type of the model</typeparam>
    public class ModelReceived<T>
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="model">Model</param>
        /// <param name="modelState">Model state</param>
        public ModelReceived(T model, ModelStateDictionary modelState)
        {
            this.Model = model;
            this.ModelState = modelState;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a model
        /// </summary>
        public T Model { get; private set; }

        /// <summary>
        /// Gets a model state
        /// </summary>
        public ModelStateDictionary ModelState { get; private set; }

        #endregion
    }
}
