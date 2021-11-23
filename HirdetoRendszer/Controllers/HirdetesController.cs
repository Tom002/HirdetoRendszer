using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        [HttpGet]
        [Authorize(Roles = "Hirdeto")]
        public Task<List<HirdetesDto>> HirdetesekListazasa() => _hirdetesService.HirdetesekListazasa();

        [HttpPost("{id}/lemondas")]
        [Authorize(Roles = "Hirdeto")]
        public Task<ActionResult> HirdetesLemondas(int id) => _hirdetesService.HirdetesLemondas(id);
    }
}
