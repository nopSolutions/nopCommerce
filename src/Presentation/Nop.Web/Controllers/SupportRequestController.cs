using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Support;
using Nop.Web.Models.Support;
using Nop.Services.Support;
using Nop.Web.Controllers;

namespace Nop.Web.Controllers;

public class SupportRequestController : BasePublicController
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
    
    #region Request List
    
    public async Task<IActionResult> List()
    {
        var requestList = _supportRequestService.GetUserSupportRequests(_currentUserId);
        
        var requests = requestList.Select(request => new SupportRequestModel(request)).ToList();
        
        return View(requests);
    }
    
    #endregion
    
    #region Create New Request
    
    public IActionResult Create()
    {
        var model = new SupportRequestModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(SupportRequestModel model)
    {
        if (ModelState.IsValid)
        {
            var request = new SupportRequest()
            {
                CustomerId = _workContext.GetCurrentCustomerAsync().Result.Id,
                Subject = model.Subject,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                Read = false,
                Status = StatusEnum.AwaitingSupport
            };

            _supportRequestService.CreateSupportRequest(request);
            
            return RedirectToAction("Chat", new { requestId = request.Id });
        }
        return View(model);
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
            IsAdmin = false,
            CreatedOnUtc = DateTime.UtcNow,
            Message = model.NewMessage
        };
        
        _supportRequestService.CreateSupportMessage(entityModel);
        
        return RedirectToAction("Chat", new { requestId = model.RequestId });
    }

    
    #endregion
    
}