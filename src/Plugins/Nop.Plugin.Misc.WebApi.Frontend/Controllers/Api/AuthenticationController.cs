using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Authentication;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers.Api;

/// <summary>
/// Authentication API controller
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    #region Fields

    private readonly IAuthenticationService _authenticationService;
    private readonly ICustomerService _customerService;
    private readonly ICustomerRegistrationService _customerRegistrationService;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public AuthenticationController(
        IAuthenticationService authenticationService,
        ICustomerService customerService,
        ICustomerRegistrationService customerRegistrationService,
        IWebHelper webHelper)
    {
        _authenticationService = authenticationService;
        _customerService = customerService;
        _customerRegistrationService = customerRegistrationService;
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    /// <param name="loginModel">Login model</param>
    /// <returns>JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginModel)
    {
        if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password))
            return BadRequest("Username and password are required");

        var loginResult = await _customerRegistrationService.ValidateCustomerAsync(loginModel.Username, loginModel.Password);
        if (loginResult != CustomerLoginResults.Successful)
            return Unauthorized("Invalid username or password");

        var customer = await _customerService.GetCustomerByUsernameAsync(loginModel.Username);
        if (customer == null)
            return Unauthorized("Customer not found");

        // Sign in customer
        await _authenticationService.SignInAsync(customer, false);

        // Generate JWT token
        var token = GenerateJwtToken(customer);

        return Ok(new LoginResponse
        {
            Token = token,
            Customer = new CustomerDto
            {
                Id = customer.Id,
                Email = customer.Email,
                Username = customer.Username,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            }
        });
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Generate JWT token for customer
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <returns>JWT token</returns>
    private string GenerateJwtToken(Customer customer)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, customer.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", customer.Username ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-change-in-production"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _webHelper.GetStoreLocation(),
            audience: _webHelper.GetStoreLocation(),
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    #endregion

    #region DTOs

    /// <summary>
    /// Login request
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Login response
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Customer information
        /// </summary>
        public CustomerDto Customer { get; set; }
    }

    /// <summary>
    /// Customer DTO
    /// </summary>
    public class CustomerDto
    {
        /// <summary>
        /// Customer ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }
    }

    #endregion
}