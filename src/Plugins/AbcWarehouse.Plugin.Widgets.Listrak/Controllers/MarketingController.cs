using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/marketingSms")]
public class MarketingController : ControllerBase
{
    private readonly IListrakService _listrakService;

    public MarketingController(IListrakService listrakService)
    {
        _listrakService = listrakService;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] MarketingSmsModel request)
    {
        try
        {
            if (!Regex.IsMatch(request.PhoneNumber, @"^\d{10}$"))
                return BadRequest(new { message = "Invalid phone number." });

            var response = await _listrakService.SubscribePhoneNumberAsync(request.PhoneNumber);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Successfully subscribed!" });
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, new { message = $"Subscription failed: {content}" });
        }
        catch (Exception ex)
        {
            // TODO: Use your plugin's logger
            Console.WriteLine(ex);
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}

public class MarketingSmsModel
{
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
}
