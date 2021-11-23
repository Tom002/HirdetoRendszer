using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Bll.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HirdetoRendszer.Api.Controllers
{
    public class HirdetesController : BaseController
    {
        private readonly IHirdetesService _hirdetesService;

        public HirdetesController(IHirdetesService hirdetesService)
        {
            _hirdetesService = hirdetesService;
        }

        [HttpPost]
        [Authorize(Roles = "Hirdeto")]
        public Task<HirdetesDto> HirdetesFeladas([FromBody] HirdetesHozzaadasDto hirdetesHozzaadasDto)
            => _hirdetesService.HirdetesFeladas(hirdetesHozzaadasDto);

        [HttpDelete("{hirdetesId:int}")]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task HirdetesTorles([FromRoute] int hirdetesId)
            => _hirdetesService.HirdetesTorles(hirdetesId);

        [HttpGet]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task<PageResponse<HirdetesDto>> OsszesHirdetesListazas([FromQuery] PageRequest pageRequest)
            => _hirdetesService.HirdetesListazas(pageRequest);
    }
}
