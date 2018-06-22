using Nop.Web.Areas.Admin.Models.Payments;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the payment method model factory
    /// </summary>
    public partial interface IPaymentModelFactory
    {
        /// <summary>
        /// Prepare payment methods model
        /// </summary>
        /// <param name="methodsModel">Payment methods model</param>        
        /// <returns>Payment methods model</returns>
        PaymentMethodsModel PreparePaymentMethodsModel(PaymentMethodsModel methodsModel);

        /// <summary>
        /// Prepare payment method search model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>Payment method search model</returns>
        PaymentMethodSearchModel PreparePaymentMethodSearchModel(PaymentMethodSearchModel searchModel);

        /// <summary>
        /// Prepare paged payment method list model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>Payment method list model</returns>
        PaymentMethodListModel PreparePaymentMethodListModel(PaymentMethodSearchModel searchModel);

        /// <summary>
        /// Prepare payment method restriction model
        /// </summary>
        /// <param name="model">Payment method restriction model</param>
        /// <returns>Payment method restriction model</returns>
        PaymentMethodRestrictionModel PreparePaymentMethodRestrictionModel(PaymentMethodRestrictionModel model);
    }
}