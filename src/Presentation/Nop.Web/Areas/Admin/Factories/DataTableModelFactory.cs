namespace Nop.Web.Areas.Admin.Factories
{
    using System.Collections.Generic;
    using Nop.Services.Events;
    using Nop.Services.Localization;
    using Nop.Services.Stores;
    using Nop.Web.Areas.Admin.Models.Orders;
    using Nop.Web.Framework.Events;
    using Nop.Web.Framework.Models.DataTables;

    public class DataTableModelFactory : IDataTableModelFactory
    {
        #region Fields

        private readonly IStoreService _storeService;
        private readonly ILocalizationService _localizationService;
        private readonly IEventPublisher _eventPublisher;
        
        #endregion

        #region Ctor

        public DataTableModelFactory(IStoreService storeService, ILocalizationService localizationService, IEventPublisher eventPublisher)
        {
            _storeService = storeService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public DataTablesModel PrepareOrderListDataTablesModel(OrderSearchModel model)
        {
            var tablesModel = new DataTablesModel
            {
                Name = "orders-grid",
                UrlRead = new DataUrl("OrderList", "Order", null),
                SearchButtonId = "search-orders",
                Length = model.PageSize,
                LengthMenu = model.AvailablePageSizes,
                FooterCallback = !model.IsLoggedInAsVendor ? "ordersfootercallback" : null,
                FooterColumns = !model.IsLoggedInAsVendor ? 10 : 0,
                Filters = new List<FilterParameter>
                {
                    new FilterParameter(nameof(model.StartDate)),
                    new FilterParameter(nameof(model.EndDate)),
                    new FilterParameter(nameof(model.OrderStatusIds)),
                    new FilterParameter(nameof(model.PaymentStatusIds)),
                    new FilterParameter(nameof(model.ShippingStatusIds)),
                    new FilterParameter(nameof(model.StoreId)),
                    new FilterParameter(nameof(model.VendorId)),
                    new FilterParameter(nameof(model.WarehouseId)),
                    new FilterParameter(nameof(model.BillingEmail)),
                    new FilterParameter(nameof(model.BillingPhone)),
                    new FilterParameter(nameof(model.BillingLastName)),
                    new FilterParameter(nameof(model.BillingCountryId)),
                    new FilterParameter(nameof(model.PaymentMethodSystemName)),
                    new FilterParameter(nameof(model.ProductId)),
                    new FilterParameter(nameof(model.OrderNotes))
                }
            };

            tablesModel.ColumnCollection = new List<ColumnProperty>
            {
                new ColumnProperty(nameof(OrderModel.Id))
                {
                    IsMasterCheckBox = true,
                    Render = new RenderCheckBox("checkbox_orders"),
                    ClassName =  NopColumnClassDefaults.CenterAll,
                    Width = "50",
                },
                new ColumnProperty(nameof(OrderModel.CustomOrderNumber))
                {
                    Title = _localizationService.GetResource("Admin.Orders.Fields.CustomOrderNumber"),
                    Width = "80"
                }
            };

            //a vendor does not have access to this functionality
            if (!model.IsLoggedInAsVendor)
            {
                tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.OrderStatus))
                {
                    Title = _localizationService.GetResource("Admin.Orders.Fields.OrderStatus"),
                    Width = "100",
                    Render = new RenderCustom("renderColumnOrderStatus")
                });
            }

            tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.PaymentStatus))
            {
                Title = _localizationService.GetResource("Admin.Orders.Fields.PaymentStatus"),
                Width = "150"
            });

            //a vendor does not have access to this functionality
            if (!model.IsLoggedInAsVendor)
            {
                tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.ShippingStatus))
                {
                    Title = _localizationService.GetResource("Admin.Orders.Fields.ShippingStatus"),
                    Width = "150"
                });
            }

            tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.CustomerEmail))
            {
                Title = _localizationService.GetResource("Admin.Orders.Fields.Customer"),
                Width = "250",
                Render = new RenderLink(new DataUrl("~/Admin/Customer/Edit", nameof(OrderModel.CustomerId)))
            });

            tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.StoreName))
            {
                Title = _localizationService.GetResource("Admin.Orders.Fields.Store"),
                Width = "100",
                Visible = _storeService.GetAllStores().Count > 1
            });

            tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.CreatedOn))
            {
                Title = _localizationService.GetResource("Admin.Orders.Fields.CreatedOn"),
                Width = "100",
                Render = new RenderDate()
            });

            //a vendor does not have access to this functionality
            if (!model.IsLoggedInAsVendor)
            {
                tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.OrderTotal))
                {
                    Title = _localizationService.GetResource("Admin.Orders.Fields.OrderTotal"),
                    Width = "100",
                });
            }

            tablesModel.ColumnCollection.Add(new ColumnProperty(nameof(OrderModel.Id))
            {
                Title = _localizationService.GetResource("Admin.Common.View"),
                Width = "50",
                ClassName = NopColumnClassDefaults.Button,
                Render = new RenderButtonEdit(new DataUrl("Edit"))
            });

            // publish event
            _eventPublisher.ModelPrepared(tablesModel);

            return tablesModel;
        }

        #endregion
    }
}