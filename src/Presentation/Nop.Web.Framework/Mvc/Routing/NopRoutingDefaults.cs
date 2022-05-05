namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents default values related to routing
    /// </summary>
    public static partial class NopRoutingDefaults
    {
        #region Route names

        public static partial class RouteName
        {
            public static partial class Generic
            {
                /// <summary>
                /// Gets the generic route name
                /// </summary>
                public static string GenericUrl => "GenericUrl";

                /// <summary>
                /// Gets the generic route (with language code e.g. en/) name
                /// </summary>
                public static string GenericUrlWithLanguageCode => "GenericUrlWithLanguageCode";

                /// <summary>
                /// Gets the generic catalog route (with language code e.g. en/) name
                /// </summary>
                public static string GenericCatalogUrl => "GenericCatalogUrl";

                /// <summary>
                /// Gets the generic catalog route name 
                /// </summary>
                public static string GenericCatalogUrlWithLanguageCode => "GenericCatalogUrlWithLanguageCode";

                /// <summary>
                /// Gets the generic product catalog route name
                /// </summary>
                public static string ProductCatalog => "ProductCatalog";

                /// <summary>
                /// Gets the generic product route name
                /// </summary>
                public static string Product => "ProductDetails";

                /// <summary>
                /// Gets the generic category route name
                /// </summary>
                public static string Category => "Category";

                /// <summary>
                /// Gets the generic manufacturer route name
                /// </summary>
                public static string Manufacturer => "Manufacturer";

                /// <summary>
                /// Gets the generic vendor route name
                /// </summary>
                public static string Vendor => "Vendor";

                /// <summary>
                /// Gets the generic news item route name
                /// </summary>
                public static string NewsItem => "NewsItem";

                /// <summary>
                /// Gets the generic blog post route name
                /// </summary>
                public static string BlogPost => "BlogPost";

                /// <summary>
                /// Gets the generic topic route name
                /// </summary>
                public static string Topic => "TopicDetails";

                /// <summary>
                /// Gets the generic product tag route name
                /// </summary>
                public static string ProductTag => "ProductsByTag";
            }
        }

        #endregion

        #region Route values keys

        public static partial class RouteValue
        {
            /// <summary>
            /// Gets default key for action route value
            /// </summary>
            public static string Action => "action";

            /// <summary>
            /// Gets default key for controller route value
            /// </summary>
            public static string Controller => "controller";

            /// <summary>
            /// Gets default key for permanent redirect route value
            /// </summary>
            public static string PermanentRedirect => "permanentRedirect";

            /// <summary>
            /// Gets default key for url route value
            /// </summary>
            public static string Url => "url";

            /// <summary>
            /// Gets default key for blogpost id route value
            /// </summary>
            public static string BlogPostId => "blogpostId";

            /// <summary>
            /// Gets default key for category id route value
            /// </summary>
            public static string CategoryId => "categoryid";

            /// <summary>
            /// Gets default key for manufacturer id route value
            /// </summary>
            public static string ManufacturerId => "manufacturerid";

            /// <summary>
            /// Gets default key for newsitem id route value
            /// </summary>
            public static string NewsItemId => "newsitemId";

            /// <summary>
            /// Gets default key for product id route value
            /// </summary>
            public static string ProductId => "productid";

            /// <summary>
            /// Gets default key for product tag id route value
            /// </summary>
            public static string ProductTagId => "productTagId";

            /// <summary>
            /// Gets default key for topic id route value
            /// </summary>
            public static string TopicId => "topicid";

            /// <summary>
            /// Gets default key for vendor id route value
            /// </summary>
            public static string VendorId => "vendorid";

            /// <summary>
            /// Gets language route value
            /// </summary>
            public static string Language => "language";

            /// <summary>
            /// Gets default key for se name route value
            /// </summary>
            public static string SeName => "SeName";

            /// <summary>
            /// Gets default key for catalog route value
            /// </summary>
            public static string CatalogSeName => "CatalogSeName";
        }

        #endregion

        /// <summary>
        /// Gets language parameter transformer
        /// </summary>
        public static string LanguageParameterTransformer => "lang";
    }
}