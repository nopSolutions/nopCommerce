using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Web.Models;
using Nop.Web.Models.Home;

namespace Nop.Web
{
    public static class Extensions
    {
        #region Category

        public static CategoryModel ToModel(this Category category)
        {
            return AutoMapper.Mapper.Map<Category, CategoryModel>(category);
        }

        #endregion

        #region Languages

        public static LanguageModel ToModel(this Language language)
        {
            return AutoMapper.Mapper.Map<Language, LanguageModel>(language);
        }

  
        #endregion

        #region Catalog

        public static ProductModel ToModel(this Product product)
        {
            return AutoMapper.Mapper.Map<Product, ProductModel>(product);
        }

        #endregion
    }
}