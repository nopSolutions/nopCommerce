using Nop.Services.Localization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace Nop.Services.Common.Pdf;

/// <summary>
/// Represents the shipment document
/// </summary>
public partial class ShipmentDocument : PdfDocument<ShipmentSource>
{
    #region Ctor

    public ShipmentDocument(ShipmentSource shipmentSource, ILocalizationService localizationService) : base(shipmentSource, localizationService)
    {
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Compose the shipment
    /// </summary>
    /// <param name="container">Content placement container</param>
    protected void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(20);

            column.Item().Row(row => row.AutoItem().Element(ComposeAddress));

            column.Item().Element(ComposeProducts);
        });
    }

    /// <summary>
    /// Compose the header
    /// </summary>
    /// <param name="container">Content placement container</param>
    protected void ComposeHeader(IContainer container)
    {
        container.DefaultTextStyle(tStyle => tStyle.SemiBold()).Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(t => ComposeField(t, Source, x => x.OrderNumberText, delimiter: " #"));
                column.Item().Text(t => ComposeField(t, Source, x => x.ShipmentNumberText, delimiter: " #"));
            });
        });
    }

    /// <summary>
    /// Compose shipment products
    /// </summary>
    /// <param name="container">Content placement container</param>
    protected void ComposeProducts(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(cellHeaderStyle).Text(t => ComposeLabel<ProductItem>(t, x => x.Name));
                header.Cell().Element(cellHeaderStyle).Text(t => ComposeLabel<ProductItem>(t, x => x.Sku));
                header.Cell().Element(cellHeaderStyle).AlignRight().Text(t => ComposeLabel<ProductItem>(t, x => x.Quantity));
            });

            foreach (var product in Source.Products)
            {
                table.Cell().Element(cellContentStyle).Element(productContainer =>
                {
                    productContainer.Column(pColumn =>
                    {
                        pColumn.Item().Text(product.Name);

                        foreach (var attribute in product.ProductAttributes)
                            pColumn.Item().DefaultTextStyle(s => s.Italic().FontSize(9)).Text(attribute);
                    });
                });

                table.Cell().Element(cellContentStyle).Text(product.Sku);
                table.Cell().Element(cellContentStyle).AlignRight().Text(product.Quantity);
            }

            static IContainer cellHeaderStyle(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
            }

            static IContainer cellContentStyle(IContainer container)
            {
                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            }
        });
    }

    /// <summary>
    /// Compose an address
    /// </summary>
    protected void ComposeAddress(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);
            column.Item()
                .BorderBottom(1)
                .PaddingBottom(5)
                .DefaultTextStyle(style => style.SemiBold())
                .Text(t => ComposeLabel<ShipmentSource>(t, x => x.Address));

            column.Item().Text(t => ComposeField(t, Source.Address, x => x.Company, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, Source.Address, x => x.Name, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, Source.Address, x => x.Phone, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, Source.Address, x => x.AddressLine, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, Source.Address, x => x.VATNumber, delimiter: ": "));

            foreach (var attribute in Source.Address.AddressAttributes)
                column.Item().Text(attribute);

            column.Item().Text(t => ComposeField(t, Source.Address, x => x.PaymentMethod, delimiter: ": "));
            column.Item().Text(t => ComposeField(t, Source.Address, x => x.ShippingMethod, delimiter: ": "));

            foreach (var (key, value) in Source.Address.CustomValues)
            {
                column.Item().Text(text =>
                {
                    text.Span(key);
                    text.Span(":");
                    text.Span(value?.ToString());
                });
            }
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

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
            });
    }

    #endregion
}