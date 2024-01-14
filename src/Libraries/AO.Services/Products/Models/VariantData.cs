using AO.Services.Extensions;
using System;

namespace AO.Services.Products.Models
{
    public class VariantData
    {
        private string _supplierProductId;
        private string _originalTitle;
        private string _title;
        private decimal _costPrice;
        private decimal _retailPrice;
        private string _eAN;
        private string _colorStr;
        private string _sizeStr;
        private int _sizeCategoryId;
        private int _stockCount;
        private string _originalCategory;
        private int _webshopCategoryId;
        private string _orgItemNumber;
        private string _brand;
        private string _variantSize;
        private string _variantColor;

        public string SupplierProductId { get => _supplierProductId; set => _supplierProductId = value; }
        public string OriginalTitle { get => _originalTitle; set => _originalTitle = value; }
        public string Title { get => _title; set => _title = value; }
        public decimal CostPrice { get => _costPrice; set => _costPrice = value; }
        public decimal RetailPrice { get => _retailPrice; set => _retailPrice = value; }
        public string EAN { get => _eAN; set => _eAN = value; }
        public string ColorStr { get => _colorStr; set => _colorStr = value; }
        public string SizeStr { get => _sizeStr; set => _sizeStr = value; }
        public int SizeCategoryId { get => _sizeCategoryId; set => _sizeCategoryId = value; }
        public int StockCount { get => _stockCount; set => _stockCount = value; }
        public string OriginalCategory { get => _originalCategory; set => _originalCategory = value; }
        public int WebshopCategoryId { get => _webshopCategoryId; set => _webshopCategoryId = value; }
        public string OrgItemNumber { get => _orgItemNumber; set => _orgItemNumber = value; }
        public string Brand { get => _brand; set => _brand = value; }
        public string VariantSize { get => _variantSize; set => _variantSize = value; }
        public string VariantColor { get => _variantColor; set => _variantColor = value; }


        public static VariantData BuildVariantDataFromCsv(string csvLine, char splitter)
        {
            string[] props = csvLine.Split(splitter);

            if (props.Length != 9)
            {
                return null;
            }

            var variantData = new VariantData();
            try
            {
                var supplierProductId = variantData.OrgItemNumber = props[0].Clean();

                variantData.SupplierProductId = supplierProductId;
                variantData.OriginalTitle = variantData.Title = props[1].Clean();
                variantData.ColorStr = props[2].Clean();
                variantData.SizeStr = props[3].Clean();
                variantData.StockCount = string.IsNullOrEmpty(props[4].Clean()) ? 0 : Convert.ToInt32(props[4].Clean());
                variantData.EAN = props[5].Clean().Replace(" ", "");
                variantData.CostPrice = string.IsNullOrEmpty(props[6].Clean()) ? 0 : decimal.Parse(props[6].Clean());
                variantData.RetailPrice = string.IsNullOrEmpty(props[7].Clean()) ? 0 : decimal.Parse(props[7].Clean());
                variantData.OriginalCategory = props[8].Clean();
                variantData.Brand = GetBrandName(supplierProductId);
            }
            catch (Exception ex)
            {
                variantData.OriginalTitle = $"Fejl ved hentning (FromCsv): {ex.Message}";
            }
            return variantData;
        }

        private static string GetBrandName(string supplierProductId)
        {
            if (string.IsNullOrEmpty(supplierProductId) || supplierProductId.Length < 3)
            {
                return string.Empty;
            }

            int.TryParse(supplierProductId.Substring(0, 2), out int brandIndicator);

            if (brandIndicator <= 0)
            {
                return string.Empty;
            }

            string brandName = "";
            switch (brandIndicator)
            {
                case 92:
                    brandName = "Holebrook"; break;
                case 94:
                    brandName = "Orca Bay"; break;
                case 95:
                    brandName = "Mac in A Sac"; break;
                case 97:
                    brandName = "Didriksons"; break;
            }

            return brandName;
        }
    }
}
