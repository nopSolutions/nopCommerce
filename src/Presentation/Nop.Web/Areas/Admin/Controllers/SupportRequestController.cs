using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Support;
using Nop.Web.Areas.Admin.Models.Support;
using Nop.Services.Support;

namespace Nop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class SupportRequestController : BaseAdminController
{
    #region Fields
    
    protected readonly ISupportRequestService _supportRequestService;
    protected readonly IWorkContext _workContext;
    protected readonly int _currentUserId;

    #endregion
    
    #region Ctor
    public SupportRequestController(ISupportRequestService supportRequestService, IWorkContext workContext)
    {
        _supportRequestService = supportRequestService;
        _workContext = workContext;
        _currentUserId = _workContext.GetCurrentCustomerAsync().Result.Id;
    }
    
    #endregion
    
    #region Request List
    
    public async Task<IActionResult> List(
        string sortBy = "date_dsc",
        string filterByStatus = "",
        string searchTerm = "",
        int pageIndex = 0,
        int pageSize = 5)
    {
        
        var requestList = await _supportRequestService.GetAllSupportRequestsAsync(
            sortByCreatedDateDsc: sortBy == "date_dsc",
            filterByStatus: filterByStatus,
            searchQuery: searchTerm,
            pageIndex: pageIndex,
            pageSize: pageSize
            );

        var viewModel = new SupportListViewModel()
        {
            Requests = requestList.Result.Select(request => new SupportRequestModel(request)).ToList(),
            CurrentPage = pageIndex,
            PageSize = pageSize,
            HasPreviousPage = requestList.Result.HasPreviousPage,
            HasNextPage = requestList.Result.HasNextPage,
            TotalPages = requestList.Result.TotalPages,
            SelectedSortOption = sortBy,
            FilterByStatus = filterByStatus,
            SearchTerm = searchTerm
        };
        
        return View(viewModel);
    }
    
    #endregion
    
    #region Chat
    
    public async Task<IActionResult> Chat(int requestId)
    { 
        var supportRequest = await _supportRequestService.GetSupportRequestByIdAsync(requestId);
        var baseMessages = await _supportRequestService.GetSupportRequestMessagesAsync(requestId);
        var viewModel = new SupportChatViewModel()
        {
            RequestId = supportRequest.Result.Id,
            Subject = supportRequest.Result.Subject,
            Status = supportRequest.Result.Status,
            Messages = baseMessages.Result.Select(message => new SupportMessageModel(message)).ToList()
        };
        
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Chat(SupportChatViewModel model)
    {
        if (ModelState.IsValid)
        {
            var entityModel = new SupportMessage()
            {
                RequestId = model.RequestId,
                AuthorId = _currentUserId,
                Message = model.NewMessage
            };
        
            await _supportRequestService.CreateSupportMessageAsync(entityModel);
        
            return RedirectToAction("Chat", new { requestId = model.RequestId });
        }
        return View(model);
    }

    
    #endregion
    
}