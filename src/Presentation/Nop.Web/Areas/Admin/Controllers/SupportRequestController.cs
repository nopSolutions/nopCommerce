using Microsoft.AspNetCore.Mvc;
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
    
    #region Request List
    
    public async Task<IActionResult> List()
    {
        var requestList = _supportRequestService.GetAllSupportRequests();
        
        var requests = requestList.Select(request => new SupportRequestModel
        {
            Id = request.Id,
            CustomerId = request.CustomerId,
            Status = request.Status,
            Subject = request.Subject,
            Read = request.Read,
            CreatedOnUtc = request.CreatedOnUtc,
            UpdatedOnUtc = request.UpdatedOnUtc
        }).ToList();
        
        return View(requests);
    }
    
    #endregion
    
    #region Chat
    
    public IActionResult Chat(int requestId)
    { 
        var supportRequest = _supportRequestService.GetSupportRequestById(requestId);
        var baseMessages = _supportRequestService.GetSupportRequestMessages(requestId);
        
        var messages = baseMessages.Select(message => new SupportMessageModel
        {
            Id = message.Id ,
            RequestId = message.RequestId,
            AuthorId = message.AuthorId, 
            IsAdmin = message.IsAdmin, 
            CreatedOnUtc = message.CreatedOnUtc,
            Message = message.Message
        }).ToList();

        var viewModel = new SupportChatViewModel()
        {
            RequestId = supportRequest.Id,
            Subject = supportRequest.Subject,
            Status = supportRequest.Status,
            Messages = messages
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
            IsAdmin = true,
            CreatedOnUtc = DateTime.UtcNow,
            Message = model.NewMessage
        };
        
        _supportRequestService.CreateSupportMessage(entityModel);
        
        return RedirectToAction("Chat", new { requestId = model.RequestId });
    }

    
    #endregion
    
}