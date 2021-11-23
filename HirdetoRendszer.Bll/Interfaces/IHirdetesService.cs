using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Pagination;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IHirdetesService
    {
        public Task<HirdetesDto> HirdetesFeladas(HirdetesHozzaadasDto hirdetesHozzaadas);

        public Task<PageResponse<HirdetesDto>> HirdetesListazas(PageRequest pageRequest);

        public Task HirdetesTorles(int hirdetesId);
    }
}
