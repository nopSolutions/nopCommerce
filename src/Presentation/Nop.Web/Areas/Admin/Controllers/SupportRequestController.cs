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

    public SupportRequestController(ISupportRequestService supportRequestService, IWorkContext workContext)
    {
        _supportRequestService = supportRequestService;
        _workContext = workContext;
    }
    
    // List action to display notes for a specific customer
    public async Task<IActionResult> List()
    {
        var requestList = _supportRequestService.GetSupportRequests();
        
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
    
}