using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Pagination;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IHirdetesHelyettesitoService
    {
        public Task<HirdetesHelyettesitoDto> HirdetesHelyettesitoLetrehozas(HirdetesHelyettesitoHozzaadasDto hirdetesHozzaadas);
        public Task<PageResponse<HirdetesHelyettesitoDto>> HirdetesHelyettesitokListazasa(PageRequest pageRequest);
        public Task HirdetesHelyettesitoTorles(int hirdetesHelyettesitoId);
    }
}
