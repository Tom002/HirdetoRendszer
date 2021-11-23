using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Interfaces;
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
    }
}
