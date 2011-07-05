using AutoMapper;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Topics;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Topics;

namespace Nop.Web.Extensions
{
    public static class MappingExtensions
    {
        //category
        public static CategoryModel ToModel(this Category entity)
        {
            return Mapper.Map<Category, CategoryModel>(entity);
        }

        //manufacturer
        public static ManufacturerModel ToModel(this Manufacturer entity)
        {
            return Mapper.Map<Manufacturer, ManufacturerModel>(entity);
        }

        //language
        public static LanguageModel ToModel(this Language entity)
        {
            return Mapper.Map<Language, LanguageModel>(entity);
        }


        //currency
        public static CurrencyModel ToModel(this Currency entity)
        {
            return Mapper.Map<Currency, CurrencyModel>(entity);
        }

        //product
        public static ProductModel ToModel(this Product entity)
        {
            return Mapper.Map<Product, ProductModel>(entity);
        }

        //address
        public static AddressModel ToModel(this Address entity)
        {
            return Mapper.Map<Address, AddressModel>(entity);
        }
        public static Address ToEntity(this AddressModel model)
        {
            return Mapper.Map<AddressModel, Address>(model);
        }
        public static Address ToEntity(this AddressModel model, Address destination)
        {
            return Mapper.Map(model, destination);
        }

        //topics
        public static TopicModel ToModel(this Topic entity)
        {
            return Mapper.Map<Topic, TopicModel>(entity);
        }
    }
}