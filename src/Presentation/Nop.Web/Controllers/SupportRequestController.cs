using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Support;
using Nop.Web.Models.Support;
using Nop.Services.Support;

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
        var requestList = await _supportRequestService.GetUserSupportRequestsAsync(_currentUserId);
        
        var requests = requestList.Result
            .Select(request => new SupportRequestModel(request))
            .ToList();
        
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
    public async Task<IActionResult> Create(SupportRequestModel model)
    {
        if (ModelState.IsValid)
        {
            var request = new SupportRequest()
            {
                CustomerId = _currentUserId,
                Subject = model.Subject,
            };

            await _supportRequestService.CreateSupportRequestAsync(request);
            
            return RedirectToAction("Chat", new { requestId = request.Id });
        }
        return View(model);
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