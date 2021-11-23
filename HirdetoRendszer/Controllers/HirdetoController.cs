using HirdetoRendszer.Bll.Dto.Hirdeto;
using HirdetoRendszer.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HirdetoRendszer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HirdetoController : ControllerBase
    {
        private readonly IHirdetoService _hirdetoService;

        public HirdetoController(IHirdetoService hirdetoService)
        {
            _hirdetoService = hirdetoService;
        }

        [HttpPut("{hirdetoId:int}/engedelyezes")]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task HirdetoEngedelyezes([FromRoute] int hirdetoId, [FromBody] HirdetoEngedelyezesDto hirdetesEngedelyezes)
            => _hirdetoService.HirdetoEngedelyezes(hirdetoId, hirdetesEngedelyezes);
    }
}
