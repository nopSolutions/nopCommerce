using System;
using System.Collections.Generic;
using System.Text;

namespace AO.Services.Logging
{
    public class UpdatingStatus
    {
        private List<UpdatedProducts> _updatedProducts;
        private List<UpdatedProducts> _updatedProductsSetActive;
        private string _updaterName;

        public int CreatedProducts { get; set; }
        public int CreatedVariants { get; set; }
        public int UpdatedVariants { get; set; }
        public int SameQuantityVariants { get; set; }        
        public int NewActiveVariants { get; set; }
        public int NewRemovedVariants { get; set; }
        public int LeftoutDueToStockType { get; set; }
        public int LeftOutDueToBrand { get; set; }
        public int LeftOutDueToLowStock { get; set; }
        public int LeftOutDueToMissingEAN { get; set; }
        public int LeftOutDueToMissingSizeOrColor { get; set; }
        public string ExtraInfo { get; set; }

        public UpdatingStatus(List<UpdatedProducts> updatedProducts, List<UpdatedProducts> updatedProductsSetActive, string updaterName)
        {
            _updatedProducts = updatedProducts;
            _updatedProductsSetActive = updatedProductsSetActive;
            _updaterName = updaterName;
        }

        public string BuildStatus()
        {
            string status = BuildUpdateStatus();
            status += BuildOnHoldStatus();
            status += BuildSetActiveStatus();

            return status;
        }

        private string BuildUpdateStatus()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ExtraInfo))
                sb.Append("<span style='color:red;font-weight:bold;font-size:14px;'>" + ExtraInfo + "</span>\r\n\r\n");

            sb.Append("\r\nNye produkter: " + CreatedProducts.ToString("N0") +
                       "\r\n\r\nOpdaterede varianter: " + UpdatedVariants.ToString("N0") +
                       "\r\nNye varianter: " + CreatedVariants.ToString("N0") +
                       "\r\nNye aktive varianter   (gået fra ikke afkrydset til afkrydset): " + NewActiveVariants.ToString("N0") +
                       "\r\nNye inaktive varianter (gået fra afkrydset til ikke afkrydset): " + NewRemovedVariants.ToString("N0"));

            if (LeftoutDueToStockType > 0)
            {
                sb.Append("\r\nUdeladte varianter pga. lagertype: " + LeftoutDueToStockType.ToString("N0") + " (f.eks. 'På lager', eller lagertype: Internal)");
            }

            if (LeftOutDueToMissingEAN > 0)
            {
                sb.Append("\r\nUdeladte varianter pga. manglende EAN: " + LeftOutDueToMissingEAN.ToString("N0"));
            }

            if (LeftOutDueToLowStock > 0)
            {
                sb.Append("\r\nUdeladte varianter pga. for lidt på lager: " + LeftOutDueToLowStock.ToString("N0"));
            }

            if (LeftOutDueToMissingSizeOrColor > 0)
            {
                sb.Append("\r\nUdeladte varianter pga. manglende størrelse eller farve: " + LeftOutDueToMissingSizeOrColor.ToString("N0"));
            }

            sb.Append("\r\n");

            return sb.ToString();
        }

        private string BuildOnHoldStatus()
        {
            if (_updatedProducts != null && _updatedProducts.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Ny status til disse " + _updatedProducts.Count + " produkter:\r\n\r\n");
                foreach (UpdatedProducts upd in _updatedProducts)
                {
                    sb.Append("\r\n" + upd.ProductStatus + ": " + upd.ProductId + " " + upd.Title + " (Variants: " + upd.NumberOfVariants + ")");
                }

                return "\r\n\r\n" + sb.ToString();
            }

            return string.Empty;
        }

        private string BuildSetActiveStatus()
        {
            if (_updatedProductsSetActive.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Ny status til disse " + _updatedProductsSetActive.Count + " produkter:\r\n\r\n");
                foreach (UpdatedProducts upd in _updatedProductsSetActive)
                {
                    sb.Append("\r\n" + upd.ProductStatus + ": " + upd.ProductId + " " + upd.Title);
                }

                return "\r\n\r\n" + sb.ToString();
            }

            return string.Empty;
        }
    }

    public struct UpdatedProducts
    {
        public int ProductId;
        public string Title;
        public int NumberOfVariants;
        public string ProductStatus;
    }
}
