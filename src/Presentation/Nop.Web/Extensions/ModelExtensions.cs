using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Home;

namespace Nop.Web.Extensions
{
    public static class ModelExtensions
    {
        //category
        public static CategoryModel ToModel(this Category entity)
        {
            return AutoMapper.Mapper.Map<Category, CategoryModel>(entity);
        }

        //manufacturer
        public static ManufacturerModel ToModel(this Manufacturer entity)
        {
            return AutoMapper.Mapper.Map<Manufacturer, ManufacturerModel>(entity);
        }

        //language
        public static LanguageModel ToModel(this Language entity)
        {
            return AutoMapper.Mapper.Map<Language, LanguageModel>(entity);
        }

        //product
        public static ProductModel ToModel(this Product entity)
        {
            return AutoMapper.Mapper.Map<Product, ProductModel>(entity);
        }

        //address
        public static AddressModel ToModel(this Address entity)
        {
            return AutoMapper.Mapper.Map<Address, AddressModel>(entity);
        }
        public static Address ToEntity(this AddressModel model)
        {
            return AutoMapper.Mapper.Map<AddressModel, Address>(model);
        }
        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return AutoMapper.Mapper.Map(model, destination);
        }
    }
}