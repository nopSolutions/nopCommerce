using AO.Services.Domain;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public class ProductStatusService : IProductStatusService
    {        
        private readonly IRepository<AOProductStatus> _productStatusRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<AOProductExtensionData> _productExtensionRepository;
        private readonly IRepository<AOProductExtensionHistory> _productExtensionHistoryRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly string _preLanguageName = "Nop.Plugin.Widgets.ProductExtension.";

        public ProductStatusService(IRepository<AOProductStatus> productStatusRepository, IRepository<Product> productRepository, IRepository<AOProductExtensionData> productExtensionRepository, IRepository<AOProductExtensionHistory> productExtensionHistoryRepository, ILocalizationService localizationService, IWorkContext workContext)
        {            
            _productStatusRepository = productStatusRepository;
            _productExtensionRepository = productExtensionRepository;
            _productExtensionHistoryRepository = productExtensionHistoryRepository;
            _localizationService = localizationService;
            _workContext = workContext;
            _productRepository = productRepository;
        }

        public virtual async Task<List<AOProductStatus>> GetAllAsync()
        {
            var rep = await _productStatusRepository.GetAllAsync(query =>
            {
                return from status in query select status;
            });

            List<AOProductStatus> presentationStatuses = new List<AOProductStatus>();
            var languageId = await _workContext.GetWorkingLanguageAsync();

            foreach (var st in rep.ToList())
            {
                string statusName = await GetLocalizedStatusName(languageId, st.StatusName);

                presentationStatuses.Add(new AOProductStatus()
                {
                    Id = st.Id,
                    StatusName = statusName
                });
            }

            return presentationStatuses;
        }
        
        public virtual async Task<AOProductStatus> GetAsync(int productStatusId)
        {
            return await _productStatusRepository.GetByIdAsync(productStatusId);
        }

        public async Task<AOProductExtensionData> GetProductExtensionData(int productId)
        {
            var rep = await _productExtensionRepository.GetAllAsync(query =>
            {
                return from extensionData in query
                       where extensionData.ProductId == productId
                select extensionData;
            });
            
            return rep.FirstOrDefault();
        }

        public async Task<List<int>> GetProductIdsWithStatus(int productStatusId)
        {
            var rep = await _productExtensionRepository.GetAllAsync(query =>
            {
                return from extensionData in query
                       where extensionData.StatusId == productStatusId
                       select extensionData;
            });

            return rep.Select(p => p.ProductId).ToList();
        }

        public async Task<List<int>> GetAllProductIds()
        {
            var rep = await _productRepository.GetAllAsync(query =>
            {
                return from products in query select products;
            });

            return rep.Select(p => p.Id).ToList();
        }


        /// <summary>
        /// Will try to find new product from old friliv product id
        /// </summary>        
        public async Task<Product> GetProductByOldId(int oldProductId)
        {
            var rep = await _productRepository.GetAllAsync(query =>
            {
                return from p in query
                       where p.AdminComment.StartsWith($"Gammelt id: {oldProductId},")
                       select p;
            });
          
            return rep.FirstOrDefault();
        }

        public async Task<IList<AOProductExtensionHistory>> GetProductStatusHistory(int productId)
        {
            var rep = await _productExtensionHistoryRepository.GetAllAsync(query =>
            {
                return from history in query
                       where history.ProductId == productId
                       orderby history.InsertDate descending
                       select history;
            });

            return await rep.ToListAsync();
        }

        public async Task SaveProductStatus(AOProductExtensionData productExtension, int oldStatusId)
        {
            if (productExtension.Id <= 0)
            {
                await _productExtensionRepository.InsertAsync(productExtension, false);
            }
            else
            {
                await _productExtensionRepository.UpdateAsync(productExtension, false);
            }            
        }

        public async Task InsertNewProductStatus(AOProductExtensionData productExtension)
        {
            await _productExtensionRepository.InsertAsync(productExtension, false);
        }

        private async Task<string> GetLocalizedStatusName(Language languageId, string statusName)
        {
            var statusNameRes = await _localizationService.GetLocaleStringResourceByNameAsync($"{_preLanguageName}{statusName}", languageId.Id);
            if (statusNameRes != null && string.IsNullOrEmpty(statusNameRes.ResourceValue) == false)
            {
                statusName = statusNameRes.ResourceValue;
            }

            return statusName;
        }

        public async Task AddToProductStatusHistory(AOProductExtensionData productExtension, int oldStatusId)
        {
            var currentUser = await _workContext.GetCurrentCustomerAsync();
            var languageId = await _workContext.GetWorkingLanguageAsync();
            var updateCommentMask = await _localizationService.GetLocaleStringResourceByNameAsync($"{_preLanguageName}ProductStatusComment", languageId.Id);

            string oldStatusName = await GetLocalizedStatusName(languageId, ((Utilities.ProductStatus)oldStatusId).ToString());
            string newStatusName = await GetLocalizedStatusName(languageId, ((Utilities.ProductStatus)productExtension.StatusId).ToString());
            var comment = string.Format(updateCommentMask.ResourceValue, currentUser.Username, oldStatusName, newStatusName);

            AOProductExtensionHistory history = new()
            {
                Comment = comment,
                NewStatusId = productExtension.StatusId,
                OldStatusId = oldStatusId,
                InsertDate = DateTime.UtcNow,
                ProductId = productExtension.ProductId
            };

            await _productExtensionHistoryRepository.InsertAsync(history);
        }

        public async Task AddToProductStatusHistory(AOProductExtensionData productExtension, string comment)
        {
            var currentUser = await _workContext.GetCurrentCustomerAsync();
            var languageId = await _workContext.GetWorkingLanguageAsync();
            var updateCommentMask = await _localizationService.GetLocaleStringResourceByNameAsync($"{_preLanguageName}ProductStatusComment", languageId.Id);
            
            AOProductExtensionHistory history = new()
            {
                Comment = comment,
                NewStatusId = productExtension.StatusId,
                OldStatusId = productExtension.StatusId,
                InsertDate = DateTime.UtcNow,
                ProductId = productExtension.ProductId
            };

            await _productExtensionHistoryRepository.InsertAsync(history);
        }

        public async Task SaveLastInventoryCountAsync(AOProductExtensionData productExtension)
        {            
            await _productExtensionRepository.UpdateAsync(productExtension);
        }
    }
}