using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Misc.AbcCore
{
    public static class BackendDbConstants
    {
        public static readonly string InvTable = "DA1_INVENTORY_MASTER";
        public static readonly string InvItemNumber = "ITEM_NUMBER";
        public static readonly string InvStatusCode = "STATUS_CODE";
        public static readonly string InvStockFlag = "INSTOCK_FLG";
        public static readonly string InvSaleUnit = "UNIT_OF_MEASURE";
        public static readonly string InvUnitCost = "UNIT_COST";
        public static readonly string InvProductType = "PRODUCT_TYPE";
        public static readonly string InvDept = "DEPARTMENT";
        public static readonly string InvSellPrice = "SELL_PRICE";
        public static readonly string InvListPrice = "LIST_PRICE";
        public static readonly string InvUpsFlag = "UPS_FLAG";
        public static readonly string InvWebPrice = "WEB_PRICE";
        public static readonly string InvSecondDesc = "SECOND_DESC_SIZE";
        public static readonly string InvDist = "DIST_CODE";
        public static readonly string InvPriceCode = "UNI_PRICE_CODE";
        public static readonly string InvModel = "MODEL";
        public static readonly string InvDescription = "DESCRIPTION";
        public static readonly string InvWebEnable = "WEB_ENABLE";
        public static readonly string InvBrand = "BRAND";
        public static readonly string InvWarrantyCode = "WARRANTY_CODE";

        public static readonly string InvAlwaysAllowProdTypes = "'XDR'";
        public static readonly string InvWebEnableYes = "'Y'";
        public static readonly string InvAllowedStockCodes = "'Y','1','2','3','4','5','6','8','A','B'";
        public static readonly string InvAllowedStatusCodes = "'X','T','D','N'";

        public static readonly List<string> InvWebPriceDepartments = new List<string>() { "A", "D", "K", "M", "L", "O", "OB", "R", "RB" };

        public static readonly string DataTable = "DA1_INV_FACT_TAG";
        public static readonly string DataDesc1 = "MISC_LINE_INFO";
        public static readonly string DataDesc2 = "HEADER_FACT_CODE";
        public static readonly string DataDescWithFlag = "WITH_FLAG";
        public static readonly string DataDate = "EXPECTED_DATE";
        public static readonly string DataWeight = "WEIGHT";
        public static readonly string DataLength = "DEPTH";
        public static readonly string DataWidth = "WIDTH";
        public static readonly string DataHeight = "HEIGHT";
        public static readonly string DataCartPrice = "CART_PRICE";
        public static readonly string DataItemNumber = "ITEM_NUMBER";
        public static readonly string DataBrand = "BRAND_CODE";
        public static readonly string DataFeature1 = "FACT_DESC_1";
        public static readonly string DataFeature2 = "FACT_DESC_2";
        public static readonly string DataFeature3 = "FACT_DESC_3";
        public static readonly string DataFeature4 = "FACT_DESC_4";
        public static readonly string DataFeature5 = "FACT_DESC_5";
        public static readonly string DataFeature6 = "FACT_DESC_6";
        public static readonly string DataFeature7 = "FACT_DESC_7";
        public static readonly string DataFeature8 = "FACT_DESC_8";

        public static readonly string BrandTable = "DA6_BRAND_MASTER";
        public static readonly string BrandName = "BRAND_DESC";
        public static readonly string BrandCode = "BRAND";

        public static readonly string SnapTable = "DA1_SNAPITEM";
        public static readonly string SnapItemNumber = "ITEM_NUMBER";

        public static readonly string PromoFileTable = "DA1_PRFILE";
        public static readonly string PromoFileItemNum = "KEY_ITEM_NUMBER";
        public static readonly string PromoFileBranch = "KEY_BRANCH";
        public static readonly string PromoFileBeginDate = "BEG_DATE_YYMMDD";
        public static readonly string PromoFileEndDate = "END_DATE_YYMMDD";
        public static readonly string PromoFilePrice = "PROMO_PRICE";
        public static readonly string PromoFileName = "PROMO_NAME";
        public static readonly string PromoFileAltFlag = "ALT_FLAG";

        public static readonly string ItemToAccessoryTable = "DA1_ITEM_TO_ACCESSORY";
        public static readonly string AccessoryToItemTable = "DA1_ACCESSORY_TO_ITEM";

        public static readonly string CategoryTable = "DA1_CATEGORY_MASTER";
        public static readonly string CategoryId = "KEY_CAT_ID";
        public static readonly string CategoryProductType = "PRODUCT_TYPE";
        public static readonly string CategoryMattressCode = "MATT_CODE";

        public static readonly string ScandownTable = "DA1_SCNDNWK";
        public static readonly string ScandownItemNumber = "KEY_ITEM_NUMBER";
        public static readonly string ScandownBeginDate = "MARKDOWN_BEG_DATE";
        public static readonly string ScandownEndDate = "MARKDOWN_END_DATE";

        public static readonly string WarrantyTable = "DA1_WARRID";
        public static readonly string WarrantyCode = "KEY_WARR_ID";
        public static readonly string WarrantyDescription = "DESC";

        public static readonly string RebateTable = "DA1_REBATE_MAIL_HEAD";
        public static readonly string RebateBrand = "KEY_BRAND";
        public static readonly string RebateId = "KEY_REBATE_ID";
        public static readonly string RebateDescription = "REBATE_DESC";
        public static readonly string RebateStartDate = "START_DATE_CCYYMMDD";
        public static readonly string RebateEndDate = "END_DATE_CCYYMMDD";
        public static readonly string RebateAmount = "REB_AMOUNT";

        public static class Promo
        {
            public static readonly string Table = "DA1_PROMO_MASTER";
            public static readonly string Buyer = "KEY_BUYERID";
            public static readonly string Department = "KEY_DEPT";
            public static readonly string Code = "KEY_PROMO_CODE";
            public static readonly string Name = "PROMO_DESC";
            public static readonly string StartDate = "START_DATE";
            public static readonly string EndDate = "END_DATE";
            public static readonly string IncludedBrand1 = "INC_BRAND_1";
            public static readonly string IncludedBrand2 = "INC_BRAND_2";
            public static readonly string IncludedBrand3 = "INC_BRAND_3";
            public static readonly string IncludedBrand4 = "INC_BRAND_4";
            public static readonly string IncludedBrand5 = "INC_BRAND_5";
            public static readonly string IncludedBrand6 = "INC_BRAND_6";
            public static readonly string IncludedBrand7 = "INC_BRAND_7";
            public static readonly string IncludedBrand8 = "INC_BRAND_8";
            public static readonly string IncludedBrand9 = "INC_BRAND_9";
            public static readonly string IncludedBrand10 = "INC_BRAND_10";
            public static readonly string IncludedProductType1 = "INC_PROD_1";
            public static readonly string IncludedProductType2 = "INC_PROD_2";
            public static readonly string IncludedProductType3 = "INC_PROD_3";
            public static readonly string IncludedProductType4 = "INC_PROD_4";
            public static readonly string IncludedProductType5 = "INC_PROD_5";
            public static readonly string IncludedProductType6 = "INC_PROD_6";
            public static readonly string IncludedProductType7 = "INC_PROD_7";
            public static readonly string IncludedProductType8 = "INC_PROD_8";
            public static readonly string IncludedProductType9 = "INC_PROD_9";
            public static readonly string IncludedProductType10 = "INC_PROD_10";
            public static readonly string IncludedProductType11 = "INC_PROD_11";
            public static readonly string IncludedProductType12 = "INC_PROD_12";
            public static readonly string IncludedProductType13 = "INC_PROD_13";
            public static readonly string IncludedProductType14 = "INC_PROD_14";
            public static readonly string IncludedProductType15 = "INC_PROD_15";
            public static readonly string DiscountType = "DISC_FLAG";
            public static readonly string DiscountAmount = "DISC_AMT";
        }

        public static class PromoInclude
        {
            public static readonly string Table = "DA1_PROMO_ITEMS_INCLUDE";
            public static readonly string Buyer = "KEY_BUYERID";
            public static readonly string Department = "KEY_DEPT";
            public static readonly string Code = "KEY_PROMO_CODE";
            public static readonly string ItemNumber = "KEY_ITEM_NUMBER";
        }

        public static class PromoExclude
        {
            public static readonly string Table = "DA1_PROMO_ITEMS_EXCLUDE";
            public static readonly string Buyer = "KEY_BUYERID";
            public static readonly string Department = "KEY_DEPT";
            public static readonly string Code = "KEY_PROMO_CODE";
            public static readonly string ItemNumber = "KEY_ITEM_NUMBER";
        }

        public static class RebateInventory
        {
            public static readonly string Table = "DA1_REBATE_MAIL_DETAIL";
            public static readonly string Brand = "KEY_BRAND";
            public static readonly string Id = "KEY_REBATE_ID";
            public static readonly string ItemNumber = "ITEM_NUMBER";
        }
    }
}