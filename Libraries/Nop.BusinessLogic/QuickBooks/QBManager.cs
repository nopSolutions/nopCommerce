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

namespace NopSolutions.NopCommerce.BusinessLogic.QuickBooks
{
    /// <summary>
    /// QuickBooks manager
    /// </summary>
    public class QBManager
    {
        #region Methods
        /// <summary>
        /// Reques order synchronization
        /// </summary>
        /// <param name="order">Order</param>
        public static void RequestSynchronization(Order order)
        {
            RequestSynchronization(EntityTypeEnum.Customer, order.CustomerId);

            RequestSynchronization(EntityTypeEnum.Invoice, order.OrderId);

            switch (order.PaymentStatus)
            {
                case PaymentStatusEnum.Paid:
                    QBEntity qbInvPayment = GetQBEntityByNopId(EntityTypeEnum.ReceivePayment, order.OrderId);
                    if (qbInvPayment == null && order.OrderTotal > Decimal.Zero)
                    {
                        RequestSynchronization(EntityTypeEnum.ReceivePayment, order.OrderId);
                    }
                    break;
                case PaymentStatusEnum.Voided:
                    QBEntity qbInvVoid = GetQBEntityByNopId(EntityTypeEnum.TxnVoid, order.OrderId);
                    if (qbInvVoid == null)
                    {
                        RequestSynchronization(EntityTypeEnum.TxnVoid, order.OrderId);
                    }
                    break;
            }

            if (order.Deleted)
            {
                QBEntity qbInvDel = GetQBEntityByNopId(EntityTypeEnum.TxnDel, order.OrderId);
                if (qbInvDel == null)
                {
                    RequestSynchronization(EntityTypeEnum.TxnDel, order.OrderId);
                }
            }
        }

        /// <summary>
        /// Request customer syncronization
        /// </summary>
        /// <param name="customer">Customer</param>
        public static void RequestSynchronization(Customer customer)
        {
            RequestSynchronization(EntityTypeEnum.Customer, customer.CustomerId);
        }

        /// <summary>
        /// Request entity synchronization
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="nopEntityId">nopCommerce entity ID</param>
        public static void RequestSynchronization(EntityTypeEnum entityType, int nopEntityId)
        {
            if(!QBIsEnabled)
            {
                throw new NopException("QuickBooks is not enabled.");
            }

            QBEntity qbEntity = GetQBEntityByNopId(entityType, nopEntityId);
            if (qbEntity == null)
            {
                qbEntity = new QBEntity()
                {
                    QBEntityId = String.Empty,
                    EntityTypeId = (int)entityType,
                    NopEntityId = nopEntityId,
                    SynStateId = (int)SynStateEnum.Requested,
                    SeqNum = String.Empty,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };
                CreateQBEntity(qbEntity);
            }
            else if (qbEntity.SynState != SynStateEnum.Requested)
            {
                qbEntity.SynStateId = (int)SynStateEnum.Requested;
                qbEntity.UpdatedOn = DateTime.UtcNow;
                UpdateQBEntity(qbEntity);
            }
        }

        /// <summary>
        /// Creates a new QBEntity
        /// </summary>
        /// <param name="entity">QBEntity</param>
        public static void CreateQBEntity(QBEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entity.QBEntityId = CommonHelper.EnsureNotNull(entity.QBEntityId);
            entity.QBEntityId = CommonHelper.EnsureMaximumLength(entity.QBEntityId, 50);
            entity.SeqNum = CommonHelper.EnsureNotNull(entity.SeqNum);
            entity.SeqNum = CommonHelper.EnsureMaximumLength(entity.SeqNum, 20);

            NopObjectContext context = ObjectContextHelper.CurrentObjectContext;
            
            context.QBEntities.AddObject(entity);
            context.SaveChanges();
        }

        /// <summary>
        /// Updates QBEntity
        /// </summary>
        /// <param name="entity">QBEntity</param>
        public static void UpdateQBEntity(QBEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            entity.QBEntityId = CommonHelper.EnsureNotNull(entity.QBEntityId);
            entity.QBEntityId = CommonHelper.EnsureMaximumLength(entity.QBEntityId, 50);
            entity.SeqNum = CommonHelper.EnsureNotNull(entity.SeqNum);
            entity.SeqNum = CommonHelper.EnsureMaximumLength(entity.SeqNum, 20);

            NopObjectContext context = ObjectContextHelper.CurrentObjectContext;
            if (!context.IsAttached(entity))
                context.QBEntities.Attach(entity);

            context.SaveChanges();
        }

        /// <summary>
        /// Gets QBEntity by entity ID
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <returns>QBEntity</returns>
        public static QBEntity GetQBEntityById(int entityId)
        {
            if (entityId == 0)
            {
                return null;
            }

            NopObjectContext context = ObjectContextHelper.CurrentObjectContext;
            var query = from qbe in context.QBEntities
                        where qbe.EntityId == entityId
                        select qbe;
            QBEntity entity = query.SingleOrDefault();
            return entity;
        }

        /// <summary>
        /// Gets QBEntity by QuickBooks entity ID
        /// </summary>
        /// <param name="qbEntityId">QuickBooks entity ID</param>
        /// <returns>QBEntity</returns>
        public static QBEntity GetQBEntityByQBEntityId(string qbEntityId)
        {
            if (String.IsNullOrEmpty(qbEntityId))
            {
                return null;
            }

            NopObjectContext context = ObjectContextHelper.CurrentObjectContext;

            var query = from qbe in context.QBEntities
                        where qbe.QBEntityId == qbEntityId
                        select qbe;

            QBEntity entity = query.SingleOrDefault();

            return entity;
        }

        /// <summary>
        /// Gets QBEntity by nopCommerce entity ID
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="nopEntityId">nopCommerce entity ID</param>
        /// <returns>QBEntity</returns>
        public static QBEntity GetQBEntityByNopId(EntityTypeEnum entityType, int nopEntityId)
        {
            NopObjectContext context = ObjectContextHelper.CurrentObjectContext;

            var query = from qbe in context.QBEntities
                        where qbe.EntityTypeId == (int)entityType && qbe.NopEntityId == nopEntityId
                        select qbe;

            QBEntity entity = query.SingleOrDefault();

            return entity;
        }

        /// <summary>
        /// Gets all QBEntities
        /// </summary>
        /// <param name="entityType">Entity type or null</param>
        /// <param name="synState">Synchronization state or null</param>
        /// <param name="count">Number of records to return</param>
        /// <returns>QBEntity collection</returns>
        public static List<QBEntity> GetAllQBEntities(EntityTypeEnum? entityType, SynStateEnum? synState, int count)
        {
            if (count <= 0)
            {
                return new List<QBEntity>();
            }

            NopObjectContext context = ObjectContextHelper.CurrentObjectContext;

            var query = (IQueryable<QBEntity>)context.QBEntities;

            if (entityType.HasValue)
            {
                int entityTypeId = (int)entityType.Value;
                query = query.Where(qbe => qbe.EntityTypeId == entityTypeId);
            }

            if (synState.HasValue)
            {
                int synStateId = (int)synState.Value;
                query = query.Where(qbe => qbe.SynStateId == synStateId);
            }

            query = query.OrderBy(qbe => qbe.UpdatedOn);

            return query.Take(count).ToList();
        }

        /// <summary>
        /// Gets the top entity wich are required synchronization
        /// </summary>
        /// <returns>QBEntity or null</returns>
        public static QBEntity GetQBEntityForSynchronization()
        {
            List<QBEntity> qbEntityCollection = GetAllQBEntities(null, SynStateEnum.Requested, 1);
            return (qbEntityCollection.Count == 0 ? null : qbEntityCollection.FirstOrDefault());
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating when QuickBooks synchronization enabled
        /// </summary>
        public static bool QBIsEnabled
        {
            get
            {
                return SettingManager.GetSettingValueBoolean("QB.Enabled", false);
            }
            set
            {
                SettingManager.SetParam("QB.Enabled", value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the username wich will be used to connect to service
        /// </summary>
        public static string QBUsername
        {
            get
            {
                return SettingManager.GetSettingValue("QB.Username", "admin");
            }
            set
            {
                SettingManager.SetParam("QB.Username", value);
            }
        }

        /// <summary>
        /// Gets or sets the password wich will be used to connect to service
        /// </summary>
        public static string QBPassword
        {
            get
            {
                return SettingManager.GetSettingValue("QB.Password", "admin");
            }
            set
            {
                SettingManager.SetParam("QB.Password", value);
            }
        }

        /// <summary>
        /// Gets or sets the QuickBooks edition
        /// </summary>
        public static QBEditionEnum QBEdition
        {
            get
            {
                return (QBEditionEnum)SettingManager.GetSettingValueInteger("QB.Edition", (int)QBEditionEnum.SimpleStart);
            }
            set
            {
                SettingManager.SetParam("QB.Edition", ((int)value).ToString());
            }
        }

        /// <summary>
        /// Gets or sets the QuickBooks item reference
        /// </summary>
        public static string QBItemRef
        {
            get
            {
                return SettingManager.GetSettingValue("QB.ItemRef", "Sales");
            }
            set
            {
                SettingManager.SetParam("QB.ItemRef", value);
            }
        }

        /// <summary>
        /// Gets or sets the QuickBooks dicsount account reference
        /// </summary>
        public static string QBDiscountAccountRef
        {
            get
            {
                return SettingManager.GetSettingValue("QB.DiscountAccountRef", "Discounts Given");
            }
            set
            {
                SettingManager.SetParam("QB.DiscountAccountRef", value);
            }
        }

        /// <summary>
        /// Gets or sets the QuickBooks shipping account reference
        /// </summary>
        public static string QBShippingAccountRef
        {
            get
            {
                return SettingManager.GetSettingValue("QB.ShippingAccountRef", "Sales");
            }
            set
            {
                SettingManager.SetParam("QB.ShippingAccountRef", value);
            }
        }

        /// <summary>
        /// Gets or sets the QuickBooks sales tax account reference
        /// </summary>
        public static string QBSalesTaxAccountRef
        {
            get
            {
                return SettingManager.GetSettingValue("QB.SalesTaxAccountRef", "Sales");
            }
            set
            {
                SettingManager.SetParam("QB.SalesTaxAccountRef", value);
            }
        }

        /// <summary>
        /// Gets or sets the QuickBooks working culture
        /// </summary>
        public static string QBCultureName
        {
            get
            {
                return SettingManager.GetSettingValue("QB.CultureName", "en-US");
            }
            set
            {
                SettingManager.SetParam("QB.CultureName", value);
            }
        }
        #endregion
    }
}
