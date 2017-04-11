using System.Collections.Generic;
#if NET451
using System.Web.Mvc;
#endif

namespace Nop.Web.Framework.Mvc
{
    /// <summary>
    /// Base nopCommerce model
    /// </summary>
#if NET451
    [ModelBinder(typeof(NopModelBinder))]
#endif
    public partial class BaseNopModel
    {
        public BaseNopModel()
        {
            this.CustomProperties = new Dictionary<string, object>();
            PostInitialize();
        }

#if NET451
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }
#endif

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }
    }

    /// <summary>
    /// Base nopCommerce entity model
    /// </summary>
    public partial class BaseNopEntityModel : BaseNopModel
    {
        public virtual int Id { get; set; }
    }
}
