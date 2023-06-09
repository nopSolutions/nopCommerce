using Nop.Core.Domain.Common;
using Nop.Web.Models.Common;

namespace Nop.Web.Extensions
{
    public static class MappingExtensions
    {
        //address
        public static Address ToEntity(this AddressModel model, bool trimFields = true)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity, trimFields);
        }

        public static Address ToEntity(this AddressModel model, Address destination, bool trimFields = true)
        {
            if (model == null)
                return destination;

            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName;
                if (model.LastName != null)
                    model.LastName = model.LastName;
                if (model.Email != null)
                    model.Email = model.Email;
                if (model.Company != null)
                    model.Company = model.Company;
                if (model.County != null)
                    model.County = model.County;
                if (model.City != null)
                    model.City = model.City;
                if (model.Address1 != null)
                    model.Address1 = model.Address1;
                if (model.Address2 != null)
                    model.Address2 = model.Address2;
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode;
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber;
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber;
            }
            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId == 0 ? null : model.CountryId;
            destination.StateProvinceId = model.StateProvinceId == 0 ? null : model.StateProvinceId;
            destination.County = model.County;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            return destination;
        }
    }
}