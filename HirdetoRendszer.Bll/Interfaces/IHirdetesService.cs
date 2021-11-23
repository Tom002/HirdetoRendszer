using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IHirdetesService
    {
        public Task<HirdetesDto> HirdetesFeladas(HirdetesHozzaadasDto hirdetesHozzaadas);
        public Task<PageResponse<HirdetesDto>> HirdetesekListazasa(PageRequest pageRequest);
        public Task HirdetesLemondas(int id);
    }
}
