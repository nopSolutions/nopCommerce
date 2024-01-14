using AO.Services.Domain;
using AO.Services.Models;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AO.Services.Utilities;

namespace AO.Services.Services
{
    public class InventoryListService : IInventoryListService
    {
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<AOProductExtensionData> _aoProductExtensionDataRepository;

        private readonly IRepository<AOInvProductRelation> _invProductRelationRepository;
        private readonly IRepository<AOInvPosition> _invPositionRepository;
        private readonly IRepository<AOInvShelf> _invShelfRepository;
        private readonly IRepository<AOInvRack> _invRackRepository;

        private readonly IRepository<Product> _productRepository;
        private readonly IProductService _productService;

        private IList<Product> _fetchedProducts;
        private List<ProductAttributeCombination> _combinations;

        public InventoryListService(IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IProductService productService, IRepository<Product> productRepository, IRepository<AOProductExtensionData> aoProductExtensionDataRepository, IRepository<AOInvProductRelation> invProductRelationRepository, IRepository<AOInvPosition> invPositionRepository, IRepository<AOInvShelf> invShelfRepository, IRepository<AOInvRack> invRackRepository)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _productService = productService;
            _productRepository = productRepository;

            _fetchedProducts = new List<Product>();
            _aoProductExtensionDataRepository = aoProductExtensionDataRepository;
            _invProductRelationRepository = invProductRelationRepository;
            _invPositionRepository = invPositionRepository;
            _invShelfRepository = invShelfRepository;
            _invRackRepository = invRackRepository;
        }

        public async Task<InventoryListModel> GetInventoryListAsync()
        {
            _combinations = _productAttributeCombinationRepository.GetAll().ToList();

            var inventoryListModel = new InventoryListModel
            {
                InventoryListItems = new List<InventoryListItem>()
            };

            var products = _productRepository.GetAll();
            int totalQuantity = 0;
            decimal totalCostPrice = 0;
            int totalCountWithoutCostPrice = 0;

            // Fetch data from AOInvProductRelation table using _invProductRelationRepository
            var relations = _invProductRelationRepository.Table.ToList();

            // Fetch data from AOInvPosition table using _yourPositionRepository
            var positions = _invPositionRepository.Table.ToList(); // Replace with the correct repository

            // Fetch data from AOInvShelf table using _yourShelfRepository
            var shelves = _invShelfRepository.Table.ToList(); // Replace with the correct repository

            // Fetch data from AOInvRack table using _yourRackRepository
            var racks = _invRackRepository.Table.ToList();

            foreach (var product in products)
            {
                // Find the relation in AOInvProductRelation based on ProductId
                var relation = relations.FirstOrDefault(r => r.ProductId == product.Id);

                AOInvPosition position = null;
                AOInvShelf shelf = null;
                AOInvRack rack = null;

                if (relation != null)
                {
                    // Find the position in AOInvPosition based on AOInvPositionId
                    position = positions.FirstOrDefault(p => p.Id == relation.AOInvPositionId);

                    // Find the shelf in AOInvShelf based on AOInvShelfId
                    shelf = shelves.FirstOrDefault(s => s.Id == position?.AOInvShelfId);

                    // Find the rack in AOInvRack based on AOInvRackId
                    rack = racks.FirstOrDefault(r => r.Id == shelf?.AOInvRackId);
                }

                var combinations = _combinations.Where(c => c.ProductId == product.Id).ToList();
                int stockQuantity = 0;

                // Now run through all combinations of this product and sum the quantity
                foreach (var combination in combinations)
                {
                    if (combination.StockQuantity <= 0)
                    {
                        // Nothing in stock of this combinatiom, move on
                        continue;
                    }

                    stockQuantity += combination.StockQuantity;
                    totalQuantity += combination.StockQuantity;
                }

                // Only if we have any in stock will we populate it
                if (stockQuantity > 0)
                {
                    var inventoryListItem = new InventoryListItem()
                    {
                        ProductId = product.Id,
                        CostPrice = product.ProductCost,
                        ProductName = product.Name,
                        StockQuantity = stockQuantity,
                        TotalCostPrice = product.ProductCost * stockQuantity,
                        PositionName = position?.Name, // Change 'Name' to the actual property you want
                        ShelfName = shelf?.Name, // Change 'Name' to the actual property you want
                        RackName = rack?.Name // Change 'Name' to the actual property you want
                    };

                    inventoryListModel.InventoryListItems.Add(inventoryListItem);
                    totalCostPrice += product.ProductCost * stockQuantity;

                    if (product.ProductCost <= 0)
                    {
                        totalCountWithoutCostPrice++;
                    }
                }
            }

            inventoryListModel.InventoryListDatetime = DateTime.UtcNow;
            inventoryListModel.ListTitle = $"Andersen Outdoor ApS - Lagerliste {DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy")}";
            inventoryListModel.TotalQuantity = totalQuantity;
            inventoryListModel.TotalCostPrice = totalCostPrice;

            if (totalCountWithoutCostPrice > 0)
            {
                inventoryListModel.WarningMessage = $"{totalCountWithoutCostPrice:N0} produkter mangler kostpris!";
            }

            return inventoryListModel;
        }

        public IList<InventoryStockCountedItem> GetInventoryListWithStockCountedDate()
        {
            var aoProductExtensionData = from p in _productRepository.Table
                                         join ex in _aoProductExtensionDataRepository.Table on p.Id equals ex.ProductId into exProduct
                                         from ex in exProduct.DefaultIfEmpty()
                                         orderby ex.InventoryCountLastDone, p.Name
                                         
                                         select new InventoryStockCountedItem
                                         {
                                             AOProductStatus = GetProductStatusName(ex.StatusId),
                                             ProductId = p.Id,
                                             ProductTitle = p.Name,
                                             Published = p.Published ? "Ja" : "Nej",
                                             StatusCountDoneTime = ex.InventoryCountLastDone
                                         };

            return aoProductExtensionData.ToList();
        }

        private string GetProductStatusName(int statusId)
        {
            if (statusId <= 0)
            {
                return "Ingen status";
            }

            return ((ProductStatus)statusId).ToString();
        }
    }
}