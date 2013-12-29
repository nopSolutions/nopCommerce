namespace Nop.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseNopEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}