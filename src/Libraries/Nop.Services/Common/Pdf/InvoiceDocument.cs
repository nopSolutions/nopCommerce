using System;
using System.Globalization;
using System.Linq;
using Nop.Services.Localization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace Nop.Services.Common.Pdf
{
    /// <summary>
    /// Represents the invoice document
    /// </summary>
    public class InvoiceDocument : PdfDocument<InvoiceSource>
    {
        #region Ctor

        public InvoiceDocument(InvoiceSource invoiceSource, ILocalizationService localizationService) : base(invoiceSource, localizationService)
        {
        }

        #endregion

        #region Utils

        /// <summary>
        /// Compose the invoice
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(15);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(50);
                        columns.RelativeColumn();

                    });

                    table.Cell().Element(ComposeBillingAddress);
                    table.Cell();
                    table.Cell().Element(ComposeShippingAddress);
                });

                column.Item().Element(ComposeProducts);

                column.Item().ShowEntire().Element(ComposeCheckoutAttributes);

                column.Item().ShowEntire().Element(c => ComposeOrderTotals(c, Source.Totals));

                column.Item().Element(ComposeOrderNotes);
            });
        }

        /// <summary>
        /// Compose billing address
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeBillingAddress(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                column.Item()
                   .BorderBottom(1)
                   .DefaultTextStyle(style => style.SemiBold())
                   .Text(t => ComposeLabel<InvoiceSource>(t, x => x.BillingAddress));

                column.Item().Column(c => ComposeAddress(c, Source.BillingAddress));
            });
        }

        /// <summary>
        /// Compose shipping address
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeShippingAddress(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(2);

                column.Item()
                   .BorderBottom(1)
                   .DefaultTextStyle(style => style.SemiBold())
                   .Text(t => ComposeLabel<InvoiceSource>(t, x => x.ShippingAddress));

                column.Item().Column(c => ComposeAddress(c, Source.ShippingAddress));
            });
        }

        /// <summary>
        /// Compose the header
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeHeader(IContainer container)
        {
            container.ShowOnce().DefaultTextStyle(tStyle => tStyle.SemiBold()).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(t => ComposeField(t, Source, x => x.OrderNumberText, delimiter: "# "));
                    column.Item().Hyperlink(Source.StoreUrl).Text(Source.StoreUrl);
                    column.Item().Text(t =>
                        ComposeField(t,
                            Source,
                            x => x.OrderDateUser,
                            date => date.ToString("D", new CultureInfo(Source.Language.LanguageCulture)),
                            ": "
                        ));
                });

                var logoContainer = row.ConstantItem(65).Height(65);

                if (Source.LogoData is not null)
                    logoContainer.Image(Source.LogoData, ImageScaling.FitArea);
            });
        }

        /// <summary>
        /// Compose order products
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeProducts(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    if (Source.ShowSkuInProductList)
                        columns.RelativeColumn();
                    if (Source.ShowVendorInProductList)
                        columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text(t => ComposeLabel<ProductItem>(t, x => x.Name));

                    if (Source.ShowSkuInProductList)
                        header.Cell().Element(CellStyle).Text(t => ComposeLabel<ProductItem>(t, x => x.Sku));

                    if (Source.ShowVendorInProductList)
                        header.Cell().Element(CellStyle).Text(t => ComposeLabel<ProductItem>(t, x => x.VendorName));

                    header.Cell().Element(CellStyle).AlignRight().Text(t => ComposeLabel<ProductItem>(t, x => x.Price));
                    header.Cell().Element(CellStyle).AlignRight().Text(t => ComposeLabel<ProductItem>(t, x => x.Quantity));
                    header.Cell().Element(CellStyle).AlignRight().Text(t => ComposeLabel<ProductItem>(t, x => x.Total));

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var product in Source.Products)
                {
                    table.Cell().Element(CellStyle).Element(productContainer =>
                    {
                        productContainer.Column(pColumn =>
                        {
                            pColumn.Item().Text(product.Name);

                            foreach (var attribute in product.ProductAttributes)
                                pColumn.Item().DefaultTextStyle(s => s.Italic().FontSize(9)).Text(attribute);
                        });
                    });

                    if (Source.ShowSkuInProductList)
                        table.Cell().Element(CellStyle).Text(product.Sku);
                    if (Source.ShowVendorInProductList)
                        table.Cell().Element(CellStyle).Text(product.VendorName);
                    table.Cell().Element(CellStyle).AlignRight().Text(product.Price);
                    table.Cell().Element(CellStyle).AlignRight().Text(product.Quantity);
                    table.Cell().Element(CellStyle).AlignRight().Text(product.Total);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

        /// <summary>
        /// Compose an address
        /// </summary>
        /// <param name="address">Address item</param>
        protected ColumnDescriptor ComposeAddress(ColumnDescriptor column, AddressItem address)
        {
            column.Item().Text(t => ComposeField(t, address, x => x.Company, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, address, x => x.Name, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, address, x => x.Phone, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, address, x => x.Address, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, address, x => x.Address2, delimiter: ": "));
            column.Item().Text(address.AddressLine);
            column.Item().Text(t => ComposeField(t, address, x => x.VATNumber));
            column.Item().Text(address.Country);

            foreach (var attribute in address.AddressAttributes)
                column.Item().Text(attribute);

            column.Item().Text(t => ComposeField(t, address, x => x.PaymentMethod, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, address, x => x.ShippingMethod, delimiter: ": "));

            foreach (var (key, value) in address.CustomValues)
            {
                column.Item().Text(text =>
                {
                    text.Span(key);
                    text.Span(": ");
                    text.Span(value?.ToString());
                });
            }

            return column;
        }

        /// <summary>
        /// Compose checkout attributes
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeCheckoutAttributes(IContainer container)
        {
            if (string.IsNullOrEmpty(Source.CheckoutAttributes))
                return;

            container.DefaultTextStyle(tStyle => tStyle.SemiBold()).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text(Source.CheckoutAttributes);
            });
        }

        /// <summary>
        /// Compose the order totals
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeOrderTotals(IContainer container, InvoiceTotals totals)
        {
            container.AlignRight().Column(column =>
            {
                column.Item().Text(t => ComposeField(t, totals, x => x.SubTotal, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, totals, x => x.Discount, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, totals, x => x.Shipping, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, totals, x => x.PaymentMethodAdditionalFee, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, totals, x => x.Tax, delimiter: ": "));

                foreach (var rate in totals.TaxRates)
                    column.Item().Text(rate);

                foreach (var card in totals.GiftCards)
                    column.Item().Text(card);

                column.Item().Text(totals.RewardPoints);
                column.Item().Text(totals.OrderTotal);
            });
        }

        /// <summary>
        /// Compose order notes
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeOrderNotes(IContainer container)
        {
            if (Source.OrderNotes?.Any() != true)
                return;

            container.Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item()
                    .BorderBottom(1).BorderColor(Colors.Black)
                    .DefaultTextStyle(style => style.SemiBold())
                    .Text(t => ComposeLabel<InvoiceSource>(t, x => x.OrderNotes));

                foreach (var (createdOn, note) in Source.OrderNotes)
                {
                    column.Item()
                        .ShowEntire()
                        .DefaultTextStyle(DefaultStyle.FontSize(10))
                        .PaddingVertical(5)
                        .Row(row =>
                        {
                            row.AutoItem().Text(createdOn);
                            row.RelativeItem().PaddingLeft(10)
                                .Text(note);
                        });
                }
            });
        }

        /// <summary>
        /// Compose the footer
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected virtual void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col1 =>
                    {
                        foreach (var line in Source.FooterTextColumn1)
                            col1.Item().Text(line);
                    });

                    row.RelativeItem().Column(col2 =>
                    {
                        foreach (var line in Source.FooterTextColumn2)
                            col2.Item().Text(line);
                    });
                });

                column.Item().AlignCenter().PaddingTop(10)
                    .Text(text => text.CurrentPageNumber().Format(p => $"- {p} -"));
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compose document's structure
        /// </summary>
        /// <param name="container">Content placement container</param>
        public override void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    var titleStyle = DefaultStyle.FontSize(10).NormalWeight();
                    page.DefaultTextStyle(titleStyle);

                    if (Source.IsRightToLeft)
                        page.ContentFromRightToLeft();

                    page.Size(Source.PageSize);
                    page.Margin(35);
                    page.MarginBottom(20);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Element(ComposeFooter);
                });
        }

        #endregion
    }
}