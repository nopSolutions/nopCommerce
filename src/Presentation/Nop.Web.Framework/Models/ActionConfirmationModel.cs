
namespace Nop.Web.Framework.Models
{
    /// <summary>
    /// Action confirmation model
    /// </summary>
    public partial class ActionConfirmationModel
    {
        /// <summary>
        /// Controller name
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// Action name
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// Window ID
        /// </summary>
        public string WindowId { get; set; }
        /// <summary>
        /// Additionl confirm text
        /// </summary>
        public string AdditonalConfirmText { get; set; }
    }
}