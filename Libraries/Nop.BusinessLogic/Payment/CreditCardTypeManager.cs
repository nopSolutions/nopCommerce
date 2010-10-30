//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.BusinessLogic.Payment
{
    /// <summary>
    /// Credit card type manager
    /// </summary>
    public partial class CreditCardTypeManager : ICreditCardTypeManager
    {
        #region Constants
        private const string CREDITCARDS_ALL_KEY = "Nop.creditcard.all";
        private const string CREDITCARDS_BY_ID_KEY = "Nop.creditcard.id-{0}";
        private const string CREDITCARDS_PATTERN_KEY = "Nop.creditcard.";
        #endregion

        #region Fields

        /// <summary>
        /// object context
        /// </summary>
        protected NopObjectContext _context;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public CreditCardTypeManager(NopObjectContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets a credit card type
        /// </summary>
        /// <param name="creditCardTypeId">Credit card type identifier</param>
        /// <returns>Credit card type</returns>
        public CreditCardType GetCreditCardTypeById(int creditCardTypeId)
        {
            if (creditCardTypeId == 0)
                return null;

            string key = string.Format(CREDITCARDS_BY_ID_KEY, creditCardTypeId);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (CreditCardType)obj2;
            }

            
            var query = from cct in _context.CreditCardTypes
                        where cct.CreditCardTypeId == creditCardTypeId
                        select cct;
            var creditCardType = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, creditCardType);
            }
            return creditCardType;
        }

        /// <summary>
        /// Marks a credit card type as deleted
        /// </summary>
        /// <param name="creditCardTypeId">Credit card type identifier</param>
        public void MarkCreditCardTypeAsDeleted(int creditCardTypeId)
        {
            var creditCardType = GetCreditCardTypeById(creditCardTypeId);
            if (creditCardType != null)
            {
                creditCardType.Deleted = true;
                UpdateCreditCardType(creditCardType);
            }
            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CREDITCARDS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets all credit card types
        /// </summary>
        /// <returns>Credit card type collection</returns>
        public List<CreditCardType> GetAllCreditCardTypes()
        {
            string key = string.Format(CREDITCARDS_ALL_KEY);
            object obj2 = NopRequestCache.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<CreditCardType>)obj2;
            }

            
            var query = from cct in _context.CreditCardTypes
                        orderby cct.DisplayOrder
                        where !cct.Deleted
                        select cct;
            var creditCardTypeCollection = query.ToList();

            if (this.CacheEnabled)
            {
                NopRequestCache.Add(key, creditCardTypeCollection);
            }
            return creditCardTypeCollection;
        }

        /// <summary>
        /// Inserts a credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        public void InsertCreditCardType(CreditCardType creditCardType)
        {
            if (creditCardType == null)
                throw new ArgumentNullException("creditCardType");

            creditCardType.Name = CommonHelper.EnsureNotNull(creditCardType.Name);
            creditCardType.Name = CommonHelper.EnsureMaximumLength(creditCardType.Name, 100);
            creditCardType.SystemKeyword = CommonHelper.EnsureNotNull(creditCardType.SystemKeyword);
            creditCardType.SystemKeyword = CommonHelper.EnsureMaximumLength(creditCardType.SystemKeyword, 100);

            

            _context.CreditCardTypes.AddObject(creditCardType);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CREDITCARDS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        public void UpdateCreditCardType(CreditCardType creditCardType)
        {
            if (creditCardType == null)
                throw new ArgumentNullException("creditCardType");

            creditCardType.Name = CommonHelper.EnsureNotNull(creditCardType.Name);
            creditCardType.Name = CommonHelper.EnsureMaximumLength(creditCardType.Name, 100);
            creditCardType.SystemKeyword = CommonHelper.EnsureNotNull(creditCardType.SystemKeyword);
            creditCardType.SystemKeyword = CommonHelper.EnsureMaximumLength(creditCardType.SystemKeyword, 100);

            
            if (!_context.IsAttached(creditCardType))
                _context.CreditCardTypes.Attach(creditCardType);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                NopRequestCache.RemoveByPattern(CREDITCARDS_PATTERN_KEY);
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoCFactory.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.CreditCardTypeManager.CacheEnabled");
            }
        }

        #endregion
    }
}
