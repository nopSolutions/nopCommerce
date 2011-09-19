//Contributor: Nicolas Muniere

using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace Nop.Plugin.Misc.WebServices
{

    [ServiceContract]
    public interface INopService
    {
        [OperationContract]
        DataSet GetPaymentMethod(string usernameOrEmail, string userPassword);
       
        
        [OperationContract]
        DataSet ExecuteDataSet(string[] sqlStatements, string usernameOrEmail, string userPassword);
        [OperationContract]
        void ExecuteNonQuery(string sqlStatement, string usernameOrEmail, string userPassword);
        [OperationContract]
        object ExecuteScalar(string sqlStatement, string usernameOrEmail, string userPassword);


        [OperationContract]
        List<OrderError> DeleteOrders(int[] ordersId, string usernameOrEmail, string userPassword);
        [OperationContract]
        void AddOrderNote(int orderId, string note, bool displayToCustomer, string usernameOrEmail, string userPassword);
        [OperationContract]
        void UpdateOrderBillingInfo(int orderId, string firstName, string lastName, string phone, string email, string fax, string company, string address1, string address2, string city, string region, string country, string postalCode, string usernameOrEmail, string userPassword);
        [OperationContract]
        void UpdateOrderShippingInfo(int orderId, string firstName, string lastName, string phone, string email, string fax, string company, string address1, string address2, string city, string region, string country, string postalCode, string usernameOrEmail, string userPassword);
        
        [OperationContract]
        void SetOrderPaymentPaid(int orderId, string usernameOrEmail, string userPassword);
        [OperationContract]
        void SetOrderPaymentPaidWithMethod(int orderId, string paymentMethodName, string usernameOrEmail, string userPassword);
        [OperationContract]
        void SetOrderPaymentPending(int orderId, string usernameOrEmail, string userPassword);
        [OperationContract]
        void SetOrderPaymentRefund(int orderId, bool offline, string usernameOrEmail, string userPassword);

        [OperationContract]
        List<OrderError> SetOrdersStatusCanceled(int[] ordersId, string usernameOrEmail, string userPassword);
        [OperationContract]
        List<OrderError> SetOrdersShippingShipped(int[] ordersId, string usernameOrEmail, string userPassword);
    }
}
