//Contributor: Nicolas Muniere


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel.Activation;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.WebServices.Security;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.WebServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class NopService : INopService
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IPermissionService _permissionSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IWorkContext _workContext;
        private readonly IPluginFinder _pluginFinder;
        
        #endregion 

        #region Ctor

        public NopService()
        {
            _addressService = EngineContext.Current.Resolve<IAddressService>();
            _encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
            _customerService = EngineContext.Current.Resolve<ICustomerService>();
            _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
            _permissionSettings = EngineContext.Current.Resolve<IPermissionService>();
            _orderProcessingService = EngineContext.Current.Resolve<IOrderProcessingService>();
            _orderService = EngineContext.Current.Resolve<IOrderService>();
            _authenticationService = EngineContext.Current.Resolve<IAuthenticationService>();
            _workContext = EngineContext.Current.Resolve<IWorkContext>();
            _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
        }

        #endregion 

        #region Utilities

        protected void CheckAccess(string usernameOrEmail, string userPassword)
        {
            //check whether web service plugin is installed
            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Misc.WebServices");
            if (pluginDescriptor == null || !pluginDescriptor.Installed)
                throw new ApplicationException("Web services plugin cannot be loaded");
            
            if  (!_customerService.ValidateCustomer(usernameOrEmail, userPassword))
                    throw new ApplicationException("Not allowed");
            
            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(usernameOrEmail) : _customerService.GetCustomerByEmail(usernameOrEmail);

            _workContext.CurrentCustomer = customer;
            _authenticationService.SignIn(customer, true);

            //valdiate whether we can access this web service
            if (!_permissionSettings.Authorize(WebServicePermissionProvider.AccessWebService))
                throw new ApplicationException("Not allowed to access web service");
        }

        protected List<Order> GetOrderCollection(int[] ordersId)
        {
            List<Order> orders = new List<Order>();
            foreach (int id in ordersId)
            {
                orders.Add(_orderService.GetOrderById(id));
            }
            return orders;
        }

        #endregion

        #region Orders

        public DataSet GetPaymentMethod(string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);

            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            var plugins = from p in _pluginFinder.GetPlugins<IPaymentMethod>()
                          select p;

            DataSet dataset = new DataSet();
            DataTable datatable = dataset.Tables.Add("PaymentMethod");
            datatable.Columns.Add("SystemName");
            datatable.Columns.Add("Name");
            foreach (var plugin in plugins)
            {
                datatable.LoadDataRow(new object[] { plugin.PluginDescriptor.SystemName, plugin.PluginDescriptor.FriendlyName }, true);
            }
            return dataset;
        }

        public DataSet ExecuteDataSet(string[] sqlStatements, string usernameOrEmail, string userPassword)
        {
            //uncomment lines below in order to allow execute any SQL
            //CheckAccess(usernameOrEmail, userPassword);

            //if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
            //    throw new ApplicationException("Not allowed to manage orders");

            //DataSet dataset = new DataSet();

            //var dataSettingsManager = new DataSettingsManager();
            //var dataProviderSettings = dataSettingsManager.LoadSettings();
            //using (SqlConnection connection = new SqlConnection(dataProviderSettings.DataConnectionString))
            //{
            //    foreach (var sqlStatement in sqlStatements)
            //    {
            //        DataTable dt = dataset.Tables.Add();
            //        SqlDataAdapter adapter = new SqlDataAdapter();
            //        adapter.SelectCommand = new SqlCommand(sqlStatement, connection);
            //        adapter.Fill(dt);

            //    }
            //}

            //return dataset;




            return null;
        }
        
        public Object ExecuteScalar(string sqlStatement, string usernameOrEmail, string userPassword)
        {
            //uncomment lines below in order to allow execute any SQL
            //CheckAccess(usernameOrEmail, userPassword);
            //if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
            //    throw new ApplicationException("Not allowed to manage orders");

            //Object result;
            //var dataSettingsManager = new DataSettingsManager();
            //var dataProviderSettings = dataSettingsManager.LoadSettings();
            //using (SqlConnection connection = new SqlConnection(dataProviderSettings.DataConnectionString))
            //{
            //    SqlCommand cmd = new SqlCommand(sqlStatement, connection);
            //    connection.Open();
            //    result = cmd.ExecuteScalar();

            //}

            //return result;



            return null;
        }
        
        public void ExecuteNonQuery(string sqlStatement, string usernameOrEmail, string userPassword)
        {
            //uncomment lines below in order to allow execute any SQL
            //CheckAccess(usernameOrEmail, userPassword);
            //if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
            //    throw new ApplicationException("Not allowed to manage orders");

            //var dataSettingsManager = new DataSettingsManager();
            //var dataProviderSettings = dataSettingsManager.LoadSettings();
            //using (SqlConnection connection = new SqlConnection(dataProviderSettings.DataConnectionString))
            //{
            //    SqlCommand cmd = new SqlCommand(sqlStatement, connection);
            //    connection.Open();
            //    cmd.ExecuteScalar();
            //}
        }

        public List<OrderError> DeleteOrders(int[] ordersId, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            List<OrderError> errors = new List<OrderError>();
            foreach (var order in GetOrderCollection(ordersId))
            {
                try
                {
                    _orderService.DeleteOrder(order);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return errors;
        }

        public void AddOrderNote(int orderId, string note, bool displayToCustomer, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = note,
                    DisplayToCustomer = displayToCustomer,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        
        public void UpdateOrderBillingInfo(int orderId, string firstName, string lastName, string phone, string email, string fax, string company, string address1, string address2,
            string city, string region, string country, string postalCode, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                Address a = order.BillingAddress;
                a.FirstName = firstName;
                a.LastName = lastName;
                a.PhoneNumber = phone;
                a.Email = email;
                a.FaxNumber = fax;
                a.Company = company;
                a.Address1 = address1;
                a.Address2 = address2;
                a.City = city;
                //a.StateProvince = region;
                a.ZipPostalCode = postalCode;
                //a.Country = country;

                _addressService.UpdateAddress(a);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        
        public void UpdateOrderShippingInfo(int orderId, string firstName, string lastName, string phone, string email, string fax, string company, string address1, string address2,
            string city, string region, string country, string postalCode, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                Address a = order.ShippingAddress;
                a.FirstName = firstName;
                a.LastName = lastName;
                a.PhoneNumber = phone;
                a.Email = email;
                a.FaxNumber = fax;
                a.Company = company;
                a.Address1 = address1;
                a.Address2 = address2;
                a.City = city;
                //a.StateProvince = region;
                a.ZipPostalCode = postalCode;
                //a.Country = country;

                _addressService.UpdateAddress(a);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        public int SetOrderStatusActive(int orderId, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                order.OrderStatus = OrderStatus.Pending;

                //Ces tests proviennent de la fonction CheckOrderStatus dans OrderManager
                if (order.OrderStatus == OrderStatus.Pending)
                {
                    if (order.PaymentStatus == PaymentStatus.Authorized ||
                        order.PaymentStatus == PaymentStatus.Paid)
                    {
                        order.OrderStatus = OrderStatus.Processing;
                    }
                }

                if (order.OrderStatus == OrderStatus.Pending)
                {
                    if (order.ShippingStatus == ShippingStatus.Shipped ||
                        order.ShippingStatus == ShippingStatus.Delivered)
                    {
                        order.OrderStatus = OrderStatus.Processing;
                    }
                }

                if (order.OrderStatus != OrderStatus.Cancelled &&
                    order.OrderStatus != OrderStatus.Complete)
                {
                    if (order.PaymentStatus == PaymentStatus.Paid)
                    {
                        if (!_orderProcessingService.CanShip(order) && !_orderProcessingService.CanDeliver(order))
                        {
                            order.OrderStatus = OrderStatus.Complete;
                        }
                    }
                }
                _orderService.UpdateOrder(order);
                return (int)order.OrderStatus;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }
        
        public void SetOrderPaymentPending(int orderId, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                order.PaymentStatus = PaymentStatus.Pending;
                _orderService.UpdateOrder(order);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }
        
        public void SetOrderPaymentPaid(int orderId, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                _orderProcessingService.MarkOrderAsPaid(order);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }
        
        public void SetOrderPaymentPaidWithMethod(int orderId, string paymentMethodName, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                order.PaymentMethodSystemName = paymentMethodName;
                _orderService.UpdateOrder(order);
                _orderProcessingService.MarkOrderAsPaid(order);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }
        
        public void SetOrderPaymentRefund(int orderId, bool offline, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            try
            {
                Order order = _orderService.GetOrderById(orderId);
                if (offline)
                {
                    _orderProcessingService.RefundOffline(order);
                }
                else
                {
                    string error = string.Empty;
                    _orderProcessingService.Refund(order);
                    if (!String.IsNullOrEmpty(error))
                    {
                        throw new ApplicationException(error);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        public void SetOrderShippingTracking(int orderId, string trackingNumber, bool sendShippedMail, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");


            //    try
            //    {
            //        Order order = _orderService.GetOrderById(orderId);
            //        order.TrackingNumber = trackingNumber;
            //        _orderService.UpdateOrder(order);
            //        if (sendShippedMail)
            //        {
            //            int orderShippedCustomerNotificationQueuedEmailId = EngineContext.Current.Resolve<IMessageService>().SendOrderShippedCustomerNotification(order, order.CustomerLanguageId);
            //            if (orderShippedCustomerNotificationQueuedEmailId > 0)
            //            {
            //                _orderService.InsertOrderNote(order.OrderId, string.Format("\"Shipped\" email (to customer) has been queued. Queued email identifier: {0}.", orderShippedCustomerNotificationQueuedEmailId), false, DateTime.Now);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new ApplicationException(ex.Message);
            //    }

        }

        public void SetOrderShippingCanceled(int orderId, string usernameOrEmail, string userPassword)
        {
            throw new NotImplementedException();
        }
        
        public List<OrderError> SetOrdersStatusCanceled(int[] ordersId, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            List<OrderError> errors = new List<OrderError>();
            foreach (var order in GetOrderCollection(ordersId))
            {
                try
                {
                    _orderProcessingService.CancelOrder(order, true);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
            return errors;
        }
        
        public List<OrderError> SetOrdersShippingShipped(int[] ordersId, string usernameOrEmail, string userPassword)
        {
            CheckAccess(usernameOrEmail, userPassword);
            if (!_permissionSettings.Authorize(StandardPermissionProvider.ManageOrders))
                throw new ApplicationException("Not allowed to manage orders");

            List<OrderError> errors = new List<OrderError>();
            foreach (var order in GetOrderCollection(ordersId))
            {
                try
                {
                    _orderProcessingService.Ship(order, false);
                }
                catch (Exception ex)
                {
                    errors.Add(new OrderError() { OrderId = order.Id, ErrorMessage = ex.Message });
                }
            }
            return errors;
        }
        
        #endregion
    }
}
