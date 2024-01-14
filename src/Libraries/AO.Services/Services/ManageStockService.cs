using AO.Services.Domain;
using AO.Services.Orders.Models;
using AO.Services.Services.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AO.Services.Services
{
    public class ManageStockService : IManageStockService
    {
        #region Private variables
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IRepository<AOInvProductRelation> _invProductRelationRepository;
        private readonly IRepository<AOInvPosition> _invPositionRepository;
        private readonly IRepository<AOInvShelf> _invShelfRepository;
        private readonly IRepository<AOInvRack> _invRackRepository;
        private readonly IRepository<AOOrder> _aoOrderRepository;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IOrderService _orderService; 
        #endregion

        public ManageStockService(IRepository<ProductAttributeCombination> productAttributeCombinationRepository, IProductAttributeService productAttributeService, IRepository<AOOrder> aoOrderRepository, IOrderService orderService, IRepository<AOInvProductRelation> invProductRelationRepository, IRepository<AOInvPosition> invPositionRepository, IRepository<AOInvShelf> invShelfRepository, IRepository<AOInvRack> invRackRepository)
        {
            _productAttributeCombinationRepository = productAttributeCombinationRepository;            
            _productAttributeService = productAttributeService;
            _aoOrderRepository = aoOrderRepository;
            _orderService = orderService;
            _invProductRelationRepository = invProductRelationRepository;
            _invPositionRepository = invPositionRepository;
            _invShelfRepository = invShelfRepository;
            _invRackRepository = invRackRepository;
        }

        public async Task<IList<Order>> IsCombinationOnOrderListAsync(ProductAttributeCombination combination)
        {
            var orderIdsByEAN = await GetOrdersByEanAsync(combination.Gtin);

            if (orderIdsByEAN != null && orderIdsByEAN.Count > 0)
            {
                var ordersByEAN = await _orderService.GetOrdersByIdsAsync(orderIdsByEAN.ToArray());
                return ordersByEAN;
            }

            return null;
        }
       
        public async Task<ProductAttributeCombination> SearchByEANAsync(string ean)
        {
            var combinations = await _productAttributeCombinationRepository.GetAllAsync(query =>
            {
                return from c in query
                       orderby c.Id
                       where c.Gtin == ean
                       select c;
            });

            if (combinations.Count > 1)
            {
                var sb = new StringBuilder();
                foreach (var combination in combinations)
                {
                    sb.AppendLine($"ProductId: {combination.ProductId}");
                }

                throw new Exception($"More than one combinatin with this EAN: '{ean}':{Environment.NewLine}{sb}");
            }

            return combinations.FirstOrDefault();
        }

        public async Task<ManageStockResultModel> UpdateStockCountAsync(int changeStockBy, ProductAttributeCombination combination, Product product)
        {
            combination.StockQuantity += changeStockBy;
            await _productAttributeCombinationRepository.UpdateAsync(combination);           

            var colorSize = await GetColorAsync(combination.AttributesXml);

            ManageStockResultModel model = new()
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ColorString = colorSize.Color,
                SizeString = colorSize.Size,
                EAN = combination.Gtin,
                StockQuantity = combination.StockQuantity
            };

            return model;
        }
      
        public async Task<string> GetStockPlacementAsync(int productId)
        {
            var rep = await _invProductRelationRepository.GetAllAsync(query =>
            {
                return from productRelation in query
                       where productRelation.ProductId == productId
                       select productRelation;
            });
            var relation = rep.FirstOrDefault();
            if (relation == null)
            {
                return string.Empty;
            }

            var repPos = await _invPositionRepository.GetAllAsync(query =>
            {
                return from position in query
                       where position.Id == relation.AOInvPositionId
                       select position;
            });
            var position = repPos.FirstOrDefault();
            if (position == null)
            {
                return string.Empty;
            }

            var repShelf = await _invShelfRepository.GetAllAsync(query =>
            {
                return from shelf in query
                       where shelf.Id == position.AOInvShelfId
                       select shelf;
            });
            var shelf = repShelf.FirstOrDefault();
            if (shelf == null)
            {
                return position.Name;
            }

            var repRack = await _invRackRepository.GetAllAsync(query =>
            {
                return from rack in query
                       where rack.Id == shelf.AOInvRackId
                       select rack;
            });
            var rack = repRack.FirstOrDefault();
            if (rack == null)
            {
                return $"{shelf.Name} > {position.Name}";
            }

            string stockPosition = $"{rack.Name} > {shelf.Name}";
            if (string.IsNullOrEmpty(position.Name) == false)
            {
                stockPosition += $" > {position.Name}";
            }
            
            return stockPosition;
        }

        public async Task<ManageStockResultModel> ResetProductStockAsync(Product product)
        {
            var allCombinations = await _productAttributeCombinationRepository.GetAllAsync(query =>
            {
                return from c in query
                       where c.ProductId == product.Id
                       select c;
            });

            foreach (var combination in allCombinations)
            {
                combination.StockQuantity = 0;
                await _productAttributeService.UpdateProductAttributeCombinationAsync(combination);
            }

            ManageStockResultModel model = new()
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ColorString = "",
                SizeString = "",
                EAN = "",
                StockQuantity = 0
            };

            return model;
        }

        #region Private methods
        private async Task<(string Color, string Size)> GetColorAsync(string attributesXml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(attributesXml);
            var productAttributeNode = doc.GetElementsByTagName("ProductAttribute");
            if (productAttributeNode.Count < 2)
            {
                throw new Exception("Missing either size or color on this combination");
            }

            var value1 = await _productAttributeService.GetProductAttributeValueByIdAsync(Convert.ToInt32(productAttributeNode[0].InnerText));
            var value2 = await _productAttributeService.GetProductAttributeValueByIdAsync(Convert.ToInt32(productAttributeNode[1].InnerText));

            (string Color, string Size) colorSize = new();
            colorSize.Color = value1.AttributeValueTypeId == 2 ? value1.Name : value2.Name;
            colorSize.Size = value1.AttributeValueTypeId == 1 ? value1.Name : value2.Name;

            return colorSize;
        }

        private async Task<IList<int>> GetOrdersByEanAsync(string ean)
        {
            // Important to leave complete path to PaymentStatus
            var orders = await _aoOrderRepository.GetAllAsync(q =>
            {
                return from order in q
                       where (
                                // Authorized (by QuickPay)
                                (Nop.Core.Domain.Payments.PaymentStatus)order.PaymentStatusId == Nop.Core.Domain.Payments.PaymentStatus.Authorized)
                                ||
                                (
                                // Paid (not QuickPay but money transfer etc.)
                                (Nop.Core.Domain.Payments.PaymentStatus)order.PaymentStatusId == Nop.Core.Domain.Payments.PaymentStatus.Paid
                                && order.PaymentMethodSystemName.ToLower().Contains("quickpay") == false
                                )
                       select order;
            });

            var filteredList = new List<AOOrder>();
            foreach (var order in orders)
            {
                if (order.OrderItems != null)
                {
                    if (order.OrderItems.Contains($";{ean}"))
                    {
                        filteredList.Add(order);
                    }
                }
            }

            return filteredList.Select(o => o.OrderId).ToList();
        } 
        #endregion
    }
}