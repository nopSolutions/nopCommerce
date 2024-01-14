using Nop.Core;
using System;

namespace AO.Services.Domain
{
    public partial class AOParentProductForAPI : BaseEntity
    {
        public int ProductId
        {
            get;
            set;
        }

        public string Category
        {
            get;
            set;
        }

        public string CategoryIds
        {
            get;
            set;
        }

        public string Brand
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public decimal Price
        {
            get;
            set;
        }

        public decimal OldPrice
        {
            get;
            set;
        }

        public decimal Discount
        {
            get;
            set;
        }

        public string ProductUrl
        {
            get;
            set;
        }     

        public string Description
        {
            get;
            set;
        }

        public string ImageURL
        {
            get;
            set;
        }
     
        public int LanguageId
        {
            get;
            set;
        }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}
