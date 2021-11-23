using HirdetoRendszer.Bll.Dto.Hirdetes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IHirdetesService
    {
        public Task<HirdetesDto> HirdetesFeladas(HirdetesHozzaadasDto hirdetesHozzaadas);
        Task<List<HirdetesDto>> HirdetesekListazasa();
        Task<ActionResult> HirdetesLemondas(int id);
    }
}
