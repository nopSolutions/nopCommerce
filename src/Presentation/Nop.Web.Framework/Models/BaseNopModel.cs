using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Represents base nopCommerce model
    /// </summary>
    public partial record BaseNopModel
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public BaseNopModel()
        {
            CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Perform additional actions for binding the model
        /// </summary>
        /// <param name="bindingContext">Model binding context</param>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom model binding</remarks>
        public virtual void BindModel(ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Perform additional actions for the model initialization
        /// </summary>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom initialization code to constructors</remarks>
        protected virtual void PostInitialize()
        {
        }

        #endregion

        #region Properties

        ////MVC is suppressing further validation if the IFormCollection is passed to a controller method. That's why we add it to the model
        //[XmlIgnore]
        //public IFormCollection Form { get; set; }

        /// <summary>
        /// Gets or sets property to store any custom values for models 
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, object> CustomProperties { get; set; }

        #endregion

    }
}