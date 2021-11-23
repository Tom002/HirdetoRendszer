using HirdetoRendszer.Bll.Dto.Hirdeto;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IHirdetoService
    {
        public Task HirdetoEngedelyezes(int felhasznaloId, HirdetoEngedelyezesDto hirdetoEngedelyezesDto);
    }
}
