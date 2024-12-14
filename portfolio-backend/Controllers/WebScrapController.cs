using Microsoft.AspNetCore.Mvc;

namespace portfolio_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebScrapController : ControllerBase
    {
        [HttpGet(Name = "Test")]
        public string Get()
        {
            return "Hello World";
        }
    }
}
