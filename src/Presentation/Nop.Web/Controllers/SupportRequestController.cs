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
    
    // GET Create action to render the form
    public IActionResult Create()
    {
        var model = new SupportRequestModel() { CustomerId = _workContext.GetCurrentCustomerAsync().Result.Id };
        return View(model);
    }

    // POST Create action to handle form submission
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
            return RedirectToAction("List");
        }
        return View(model);
    }
    
}