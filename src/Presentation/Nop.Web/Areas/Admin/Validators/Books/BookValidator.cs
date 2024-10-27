using FluentValidation;
using Nop.Core.Domain.Books;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Books;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Books
{
    /// <summary>
    /// Represents a book validator for CRUD operation
    /// </summary>
    public partial class BookValidator: BaseNopValidator<BookModel>
    {
        public BookValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.ContentManagement.Book.Books.Fields.Name.Required"));  

            SetDatabaseValidationRules<Book>(dataProvider);
        }
    }
}
