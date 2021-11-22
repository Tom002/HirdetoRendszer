using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HirdetoRendszer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), 499)] // https://httpstatuses.com/499
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status501NotImplemented)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class BaseController : ControllerBase
    {
    }
}
