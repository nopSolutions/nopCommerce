using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Areas.Admin.Models.Books;

namespace Nop.Web.Areas.Admin.Validators.Book;

public class BookValidator : BaseNopValidator<BookModel>
{
    public BookValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Name is Required."));
    }
}
