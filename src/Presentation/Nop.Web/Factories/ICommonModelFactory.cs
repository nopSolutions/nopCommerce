using System.Web.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    public partial interface ICommonModelFactory
    {
        LogoModel PrepareLogoModel();

        LanguageSelectorModel PrepareLanguageSelectorModel();

        CurrencySelectorModel PrepareCurrencySelectorModel();

        TaxTypeSelectorModel PrepareTaxTypeSelectorModel();

        HeaderLinksModel PrepareHeaderLinksModel();

        AdminHeaderLinksModel PrepareAdminHeaderLinksModel();

        SocialModel PrepareSocialModel();

        FooterModel PrepareFooterModel();

        ContactUsModel PrepareContactUsModel(ContactUsModel model, bool excludeProperties);

        ContactVendorModel PrepareContactVendorModel(ContactVendorModel model, Vendor vendor,
            bool excludeProperties);

        SitemapModel PrepareSitemapModel();

        string PrepareSitemapXml(UrlHelper url, int? id);

        StoreThemeSelectorModel PrepareStoreThemeSelectorModel();

        FaviconModel PrepareFaviconModel();

        string PrepareRobotsTextFile();
    }
}
