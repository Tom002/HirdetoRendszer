using HirdetoRendszer.Bll.Dto.Hirdetes;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IHirdetesService
    {
        public Task<HirdetesDto> HirdetesFeladas(HirdetesHozzaadasDto hirdetesHozzaadas);
    }
}
