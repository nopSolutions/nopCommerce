using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using System.IO;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Payment;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.BusinessLogic.QuickBooks
{
    /// <summary>
    /// QuickBooks service
    /// </summary>
    public partial interface IQBService
    {
        #region Methods

        /// <summary>
        /// Reques order synchronization
        /// </summary>
        /// <param name="order">Order</param>
        void RequestSynchronization(Order order);

        /// <summary>
        /// Request customer syncronization
        /// </summary>
        /// <param name="customer">Customer</param>
        void RequestSynchronization(Customer customer);

        /// <summary>
        /// Request entity synchronization
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="nopEntityId">nopCommerce entity ID</param>
        void RequestSynchronization(EntityTypeEnum entityType, int nopEntityId);

        /// <summary>
        /// Creates a new QBEntity
        /// </summary>
        /// <param name="entity">QBEntity</param>
        void CreateQBEntity(QBEntity entity);

        /// <summary>
        /// Updates QBEntity
        /// </summary>
        /// <param name="entity">QBEntity</param>
        void UpdateQBEntity(QBEntity entity);

        /// <summary>
        /// Gets QBEntity by entity ID
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <returns>QBEntity</returns>
        QBEntity GetQBEntityById(int entityId);

        /// <summary>
        /// Gets QBEntity by QuickBooks entity ID
        /// </summary>
        /// <param name="qbEntityId">QuickBooks entity ID</param>
        /// <returns>QBEntity</returns>
        QBEntity GetQBEntityByQBEntityId(string qbEntityId);

        /// <summary>
        /// Gets QBEntity by nopCommerce entity ID
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="nopEntityId">nopCommerce entity ID</param>
        /// <returns>QBEntity</returns>
        QBEntity GetQBEntityByNopId(EntityTypeEnum entityType, int nopEntityId);

        /// <summary>
        /// Gets all QBEntities
        /// </summary>
        /// <param name="entityType">Entity type or null</param>
        /// <param name="synState">Synchronization state or null</param>
        /// <param name="count">Number of records to return</param>
        /// <returns>QBEntity collection</returns>
        List<QBEntity> GetAllQBEntities(EntityTypeEnum? entityType, SynStateEnum? synState, int count);

        /// <summary>
        /// Gets the top entity wich are required synchronization
        /// </summary>
        /// <returns>QBEntity or null</returns>
        QBEntity GetQBEntityForSynchronization();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating when QuickBooks synchronization enabled
        /// </summary>
        bool QBIsEnabled {get;set;}

        /// <summary>
        /// Gets or sets the username wich will be used to connect to service
        /// </summary>
        string QBUsername { get; set; }

        /// <summary>
        /// Gets or sets the password wich will be used to connect to service
        /// </summary>
        string QBPassword { get; set; }

        /// <summary>
        /// Gets or sets the QuickBooks edition
        /// </summary>
        QBEditionEnum QBEdition { get; set; }

        /// <summary>
        /// Gets or sets the QuickBooks item reference
        /// </summary>
        string QBItemRef { get; set; }

        /// <summary>
        /// Gets or sets the QuickBooks dicsount account reference
        /// </summary>
        string QBDiscountAccountRef { get; set; }

        /// <summary>
        /// Gets or sets the QuickBooks shipping account reference
        /// </summary>
        string QBShippingAccountRef { get; set; }

        /// <summary>
        /// Gets or sets the QuickBooks sales tax account reference
        /// </summary>
        string QBSalesTaxAccountRef { get; set; }

        /// <summary>
        /// Gets or sets the QuickBooks working culture
        /// </summary>
        string QBCultureName { get; set; }

        #endregion
    }
}
