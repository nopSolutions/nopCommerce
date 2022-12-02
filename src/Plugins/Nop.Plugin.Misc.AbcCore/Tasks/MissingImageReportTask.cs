using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Services.Tasks;
using OfficeOpenXml;
using System.IO;
using Nop.Data;
using Nop.Core.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Nop.Services.Media;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Extensions;
using System.Collections.Generic;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore
{
    class MissingImageReportTask : IScheduleTask
    {
        private readonly string _excelPath;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductAbcDescription> _productAbcRepository;
        private readonly IPictureService _pictureService;
        private readonly IAbcProductService _customProductService;

        private static class ProductTable
        {
            public static int IdIdx = 0;
            public static int NameIdx = 1;
            public static int SkuIdx = 2;
            public static int ItemNoIdx = 3;
        }

        public MissingImageReportTask(
            IRepository<Product> productRepository,
            IRepository<ProductAbcDescription> productAbcRepository,
            IPictureService pictureService,
            IAbcProductService customProductService
        )
        {
            _productRepository = productRepository;
            _productAbcRepository = productAbcRepository;
            _pictureService = pictureService;
            _customProductService = customProductService;

            var env = EngineContext.Current.Resolve<IWebHostEnvironment>();
            _excelPath = Path.Combine(env.WebRootPath, "ImageReport.xlsx");
        }

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            

            var prodsInfo = from prod in (await _customProductService.GetProductsWithoutImagesAsync())
                            from pAbc in _productAbcRepository.Table.Where(pA => pA.Product_Id == prod.Id).ToList()
                            select new
                            {
                                prod.Id,
                                prod.Name,
                                prod.Sku,
                                ItemNo = pAbc == null ? "" : pAbc.AbcItemNumber
                            };
            ExcelPackage ex = GetPackage();

            var prodSheet = ex.Workbook.Worksheets[0];
            int rowIdx = 2;
            foreach (var prodInfo in prodsInfo)
            {
                prodSheet.Cells[rowIdx, ProductTable.IdIdx + 1].Value = prodInfo.Id;
                prodSheet.Cells[rowIdx, ProductTable.NameIdx + 1].Value = prodInfo.Name;
                prodSheet.Cells[rowIdx, ProductTable.SkuIdx + 1].Value = prodInfo.Sku;
                prodSheet.Cells[rowIdx, ProductTable.ItemNoIdx + 1].Value = prodInfo.ItemNo;
                ++rowIdx;
            }
            ex.Save();
            
        }

        private ExcelPackage GetPackage()
        {
            if (File.Exists(_excelPath))
            {
                File.Delete(_excelPath);
            }

            ExcelPackage ex = new ExcelPackage(new FileInfo(_excelPath));
            var productsSheet = ex.Workbook.Worksheets.Add("Products");
            var prodTable = productsSheet.Tables.Add(new ExcelAddressBase("$A:$D"), "Products");
            prodTable.ShowHeader = true;
            prodTable.Columns[ProductTable.IdIdx].Name = "Id";
            prodTable.Columns[ProductTable.NameIdx].Name = "Name";
            prodTable.Columns[ProductTable.SkuIdx].Name = "Sku";
            prodTable.Columns[ProductTable.ItemNoIdx].Name = "Item No";
            return ex;
        }
    }
}
