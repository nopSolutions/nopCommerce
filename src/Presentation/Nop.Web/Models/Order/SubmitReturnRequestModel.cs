using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Order
{
    public partial record SubmitReturnRequestModel : BaseNopModel
    {
        public SubmitReturnRequestModel()
        {
            Items = new List<OrderItemModel>();
            AvailableReturnReasons = new List<ReturnRequestReasonModel>();
            AvailableReturnActions = new List<ReturnRequestActionModel>();
        }

        public int OrderId { get; set; }
        public string CustomOrderNumber { get; set; }

        public IList<OrderItemModel> Items { get; set; }

        [NopResourceDisplayName("ReturnRequests.ReturnReason")]
        public int ReturnRequestReasonId { get; set; }
        public IList<ReturnRequestReasonModel> AvailableReturnReasons { get; set; }

        [NopResourceDisplayName("ReturnRequests.ReturnAction")]
        public int ReturnRequestActionId { get; set; }
        public IList<ReturnRequestActionModel> AvailableReturnActions { get; set; }

        [NopResourceDisplayName("ReturnRequests.Comments")]
        public string Comments { get; set; }

        public bool AllowFiles { get; set; }
        [NopResourceDisplayName("ReturnRequests.UploadedFile")]
        public Guid UploadedFileGuid { get; set; }

        public string Result { get; set; }

        #region Nested classes

        public partial record OrderItemModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string AttributeInfo { get; set; }

            public string UnitPrice { get; set; }

            public int Quantity { get; set; }
        }

        public partial record ReturnRequestReasonModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }

        public partial record ReturnRequestActionModel : BaseNopEntityModel
        {
            public string Name { get; set; }
        }

        #endregion
    }

}