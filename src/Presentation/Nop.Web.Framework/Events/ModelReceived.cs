using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Framework.Events
{
    /// <summary>
    /// Represents an event that occurs after the model is receved from the view
    /// </summary>
    /// <typeparam name="T">Type of the model</typeparam>
    public class ModelReceived<T> where T : BaseNopModel
    {
        #region Ctor

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
