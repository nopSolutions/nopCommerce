using Nop.Core.Domain.Tax;
using Nop.Services.Tax;
using NUnit.Framework;

namespace Nop.Tests.Nop.Services.Tests.Tax;

[TestFixture]
public class TaxCategoryServiceTests :ServiceTest<TaxCategory>
{
    private ITaxCategoryService _taxCategoryService;

    public TaxCategoryServiceTests()
    {
        _taxCategoryService = GetService<ITaxCategoryService>();
    }

    protected override CrudData<TaxCategory> CrudData
    {
        get
        {
            var baseEntity = new TaxCategory
            {
                Name = "Test tax category",
                DisplayOrder = 1
            };

            var updatedEntity = new TaxCategory
            {
                Name = "Updated test tax category",
                DisplayOrder = 2
            };

            return new CrudData<TaxCategory>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _taxCategoryService.InsertTaxCategoryAsync,
                Update = _taxCategoryService.UpdateTaxCategoryAsync,
                Delete = _taxCategoryService.DeleteTaxCategoryAsync,
                GetById = _taxCategoryService.GetTaxCategoryByIdAsync,
                IsEqual = (first, second) => first.Name.Equals(second.Name) && first.DisplayOrder == second.DisplayOrder
            };
        }
    }
}
