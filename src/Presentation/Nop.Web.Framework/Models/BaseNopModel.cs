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
            PostInitialize();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Perform additional actions for the model initialization
        /// </summary>
        /// <remarks>Developers can override this method in custom partial classes in order to add some custom initialization code to constructors</remarks>
        protected virtual void PostInitialize()
        {
        }

        #endregion
    }
}