using HirdetoRendszer.Bll.Dto.Fedelzeti;
using HirdetoRendszer.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HirdetoRendszer.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FedelzetiController : ControllerBase {
        private readonly IFedelzetiService _fedelzetiService;

        public FedelzetiController(IFedelzetiService fedelzetiService) {
            _fedelzetiService = fedelzetiService;
        }

        [HttpPost("{jaratId:int}/indul")]
        [Authorize(Roles = "FedelzetiRendszer")]
        public Task<HirdetesCsoportDto> JaratIndul([FromRoute] int jaratId)
            => _fedelzetiService.JaratInditas(jaratId);

        [HttpPost("{jaratId:int}/vege")]
        [Authorize(Roles = "FedelzetiRendszer")]
        public Task JaratVege([FromRoute] int jaratId, [FromBody] List<MegjelenitettHirdetesDto> megjelenitettHirdetesek)
            => _fedelzetiService.MegjelenitettHirdetesekKonyvelese(jaratId, megjelenitettHirdetesek);
    }
}
