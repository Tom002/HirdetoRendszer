using HirdetoRendszer.Bll.Dto.Auth;
using HirdetoRendszer.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HirdetoRendszer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public Task<BejelentkezesValaszDto> Bejelentkezes(BejelentkezesDto bejelentkezesDto)
            => _authService.Bejelentkezes(bejelentkezesDto);

        [HttpPost("hirdetoRegisztracio")]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public Task<BejelentkezesValaszDto> HirdetoRegisztracio(HirdetoRegisztracioDto signInDto)
            => _authService.HirdetoRegisztracio(signInDto);
    }
}
