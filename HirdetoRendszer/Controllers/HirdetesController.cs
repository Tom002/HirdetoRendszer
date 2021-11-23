using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Bll.Pagination;
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
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task<PageResponse<HirdetesDto>> HirdetesekListazasa([FromQuery] PageRequest pageRequest) => _hirdetesService.HirdetesekListazasa(pageRequest);

        [HttpPost("{hirdetesId:int}/lemondas")]
        [Authorize(Roles = "Hirdeto")]
        public Task HirdetesLemondas(int id) => _hirdetesService.HirdetesLemondas(id);
        
        [HttpDelete("{hirdetesId:int}")]
        [Authorize(Roles = "HirdetesSzervezoCeg")]
        public Task HirdetesTorles([FromRoute] int hirdetesId)
            => _hirdetesService.HirdetesTorles(hirdetesId);
    }
}
