using Microsoft.AspNetCore.Mvc;

namespace FlowCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}

