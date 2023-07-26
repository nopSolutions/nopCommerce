using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Nop.Plugin.Misc.AbcCore.Extensions
{
    public static class ProductAttributeMappingExtensions
    {
        public static bool EqualTo(this ProductAttributeMapping pam, ProductAttributeMapping pam2)
        {
            return pam.ProductId == pam2.ProductId &&
                   pam.ProductAttributeId == pam2.ProductAttributeId &&
                   pam.DisplayOrder == pam2.DisplayOrder &&
                   pam.TextPrompt == pam2.TextPrompt &&
                   pam.ConditionAttributeXml == pam2.ConditionAttributeXml &&
                   pam.IsRequired == pam2.IsRequired;
        }
    }
}