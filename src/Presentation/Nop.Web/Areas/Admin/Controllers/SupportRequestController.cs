using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Support;
using Nop.Web.Areas.Admin.Models.Support;
using Nop.Services.Support;

namespace Nop.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class SupportRequestController : BaseAdminController
{
    protected readonly ISupportRequestService _supportRequestService;
    protected readonly IWorkContext _workContext;
    protected readonly int _currentUserId;

    public SupportRequestController(ISupportRequestService supportRequestService, IWorkContext workContext)
    {
        _supportRequestService = supportRequestService;
        _workContext = workContext;
        _currentUserId = _workContext.GetCurrentCustomerAsync().Result.Id;
    }
    
    #region utilities

    private static List<SelectListItem> GetAvailableStatuses()
    {
        var availableStatuses = new List<SelectListItem>();
        var enumValues = Enum.GetValues(typeof(StatusEnum));

        foreach (var status in enumValues)
        {
            availableStatuses.Add(new SelectListItem(status.ToString(), status.ToString()));
        }
        
        // Empty status to allow removal of filter
        availableStatuses.Add(new SelectListItem("", ""));
        
        return availableStatuses;
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
        
        var requestList = _supportRequestService.GetAllSupportRequests(
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
            AvailableStatuses = GetAvailableStatuses(),
            FilterByStatus = filterByStatus,
            SearchTerm = searchTerm
        };
        
        return View(viewModel);
    }
    
    #endregion
    
    #region Chat
    
    public IActionResult Chat(int requestId)
    { 
        var supportRequest = _supportRequestService.GetSupportRequestById(requestId);
        var baseMessages = _supportRequestService.GetSupportRequestMessages(requestId);
        var viewModel = new SupportChatViewModel()
        {
            RequestId = supportRequest.Id,
            Subject = supportRequest.Subject,
            Status = supportRequest.Status,
            Messages = baseMessages.Select(message => new SupportMessageModel(message)).ToList()
        };
        
        return View(viewModel);
        
    }

    [HttpPost]
    public IActionResult AddMessage(SupportChatViewModel model)
    {
        var entityModel = new SupportMessage()
        {
            RequestId = model.RequestId,
            AuthorId = _currentUserId,
            Message = model.NewMessage
        };
        
        _supportRequestService.CreateSupportMessage(entityModel);
        
        return RedirectToAction("Chat", new { requestId = model.RequestId });
    }

    
    #endregion
    
}