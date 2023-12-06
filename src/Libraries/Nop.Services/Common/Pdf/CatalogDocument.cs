using Nop.Services.Localization;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Nop.Services.Common.Pdf
{
    /// <summary>
    /// Represents the catalog document
    /// </summary>
    public partial class CatalogDocument : PdfDocument<CatalogSource>
    {
        #region Ctor

        public CatalogDocument(CatalogSource catalogSource, ILocalizationService localizationService) : base(catalogSource, localizationService)
        {
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Compose the catalog
        /// </summary>
        /// <param name="container">Content placement container</param>
        protected void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                foreach (var item in Source.Products)
                {
                    column.Spacing(5);
                    column.Item().Element(x => ComposeProductInfo(x, item));
                    column.Item().Element(x => ComposeProductProperties(x, item));
                    column.Item().Element(x => ComposeProductImages(x, item));

                    column.Item().PageBreak();
                }
            });
        }

        /// <summary>
        /// Compose a generic product info
        /// </summary>
        /// <param name="container">Content placement container</param>
        /// <param name="catalogItem">Catalog item</param>
        protected void ComposeProductInfo(IContainer container, CatalogItem catalogItem)
        {
            container.PaddingBottom(15).Column(column =>
            {
                column.Item()
                    .DefaultTextStyle(style => style.SemiBold().FontSize(16)).PaddingBottom(10).Text(catalogItem.Name);
                column.Item().Text(catalogItem.Description);
            });
        }

        /// <summary>
        /// Compose product properties
        /// </summary>
        /// <param name="container">Content placement container</param>
        /// <param name="catalogItem">Catalog item</param>
        protected void ComposeProductProperties(IContainer container, CatalogItem catalogItem)
        {
            container.Column(column =>
            {
                column.Item().Text(t => ComposeField(t, catalogItem, x => x.Price, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, catalogItem, x => x.Sku, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, catalogItem, x => x.Weight, delimiter: ": "));
                column.Item().Text(t => ComposeField(t, catalogItem, x => x.Stock, delimiter: ": "));
            });
        }

        /// <summary>
        /// Compose product images
        /// </summary>
        /// <param name="container">Content placement container</param>
        /// <param name="catalogItem">Catalog item</param>
        protected void ComposeProductImages(IContainer container, CatalogItem catalogItem)
        {
            container
                .Padding(15)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    foreach (var path in catalogItem.PicturePaths)
                    {
                        var cell = table.Cell();

                        if (catalogItem.PicturePaths.Count % 2 != 0 && catalogItem.PicturePaths.Last() == path)
                            cell.ColumnSpan(2);

                        cell.Padding(10).AlignCenter().AlignTop().Element(x => x.Width(200).Image(path));
                    }
                });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compose a document's structure
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

                    page.Content().Element(ComposeContent);
                });
        }

        #endregion
    }
}