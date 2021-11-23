using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Bll.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HirdetoRendszer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HirdetesHelyettesitoController : ControllerBase
    {
        private readonly IHirdetesHelyettesitoService _hirdetesHelyettesitoService;

        public HirdetesHelyettesitoController(IHirdetesHelyettesitoService hirdetesHelyettesitoService)
        {
            _hirdetesHelyettesitoService = hirdetesHelyettesitoService;
        }

        [HttpPost]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task<HirdetesHelyettesitoDto> HirdetesHelyettesitoHozzaadas([FromBody] HirdetesHelyettesitoHozzaadasDto hirdetesHelyettesitoHozzaadas)
            => _hirdetesHelyettesitoService.HirdetesHelyettesitoLetrehozas(hirdetesHelyettesitoHozzaadas);

        [HttpGet]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task<PageResponse<HirdetesHelyettesitoDto>> HirdetesHelyettesitokListazasa([FromQuery] PageRequest pageRequest)
            => _hirdetesHelyettesitoService.HirdetesHelyettesitokListazasa(pageRequest);

        [HttpDelete("hirdetesHelyettesitoId:int")]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task HirdetesHelyettesitoTorles([FromRoute] int hirdetesHelyettesitoId)
            => _hirdetesHelyettesitoService.HirdetesHelyettesitoTorles(hirdetesHelyettesitoId);
    }
}
