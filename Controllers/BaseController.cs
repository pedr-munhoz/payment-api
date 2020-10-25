using Microsoft.AspNetCore.Mvc;

namespace payment_api.Controllers
{
    /// <summary>
    /// Basic controller that defines the routing of the API
    /// </summary>
    [ApiController]
    [Route("api/v2/[controller]")]
    public abstract class BaseController : ControllerBase
    {

    }
}