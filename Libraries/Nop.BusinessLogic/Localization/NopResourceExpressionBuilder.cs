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
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.IoC;


namespace NopSolutions.NopCommerce.BusinessLogic.Localization
{
    /// <summary>
    /// Represents a resource expression builder
    /// </summary>
    [ExpressionPrefix("NopResources")]
	public partial class NopResourceExpressionBuilder : ExpressionBuilder
    {
        #region Ctor
        /// <summary>
        /// Creates a new instance of the NopResourceExpressionBuilder class
        /// </summary>
        public NopResourceExpressionBuilder()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets value
        /// </summary>
        /// <param name="resourceKey">Resource key</param>
        /// <returns>Resource value</returns>
        public static string GetVal(string resourceKey)
        {
            string resourceValue = string.Empty;
            try
            {
                resourceValue = IoCFactory.Resolve<ILocalizationManager>().GetLocaleResourceString(resourceKey);
            }
            catch
            {
            }
            if (!String.IsNullOrEmpty(resourceValue))
                return resourceValue;
            else
                return resourceKey;
        }
        /// <summary>
        /// Returns a code expression to evaluate during page execution.
        /// </summary>
        /// <param name="entry">The property name of the object.</param>
        /// <param name="parsedData">The parsed value of the expression.</param>
        /// <param name="context">Properties for the control or page.</param>
        /// <returns>A CodeExpression that invokes a method.</returns>
        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData, ExpressionBuilderContext context)
        {
            var lsr = (LocaleStringResource)parsedData;
            var ex = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(base.GetType()),
                "GetVal",
                new CodePrimitiveExpression(lsr.ResourceName)
            );
            return ex;
        }

        /// <summary>
        /// Returns an object that represents the parsed expression. 
        /// </summary>
        /// <param name="expression">The value of the declarative expression.</param>
        /// <param name="propertyType">The type of the property bound to by the expression.</param>
        /// <param name="context">Contextual information for the evaluation of the expression.</param>
        /// <returns>An Object that represents the parsed expression</returns>
        public override object ParseExpression(string expression, Type propertyType, ExpressionBuilderContext context)
        {
            var lsr = new LocaleStringResource();
            lsr.ResourceName = expression;
            return lsr;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns a value indicating whether the current ExpressionBuilder object supports no-compile pages. 
        /// </summary>
        public override bool SupportsEvaluate
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}
