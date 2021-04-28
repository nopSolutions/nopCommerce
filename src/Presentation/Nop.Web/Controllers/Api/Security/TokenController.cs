using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Nop.Core;

namespace Nop.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public string GetRandomToken()
        {
            var numberGenerate = CommonHelper.GenerateRandomDigitCode(24);
            var jwt = new JwtService(_config);
            var token = jwt.GenerateSecurityToken(numberGenerate + "yo!Token");
            return token;
        }
    }
}