using HirdetoRendszer.Bll.Dto.Fedelzeti;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IFedelzetiService {
        public Task<HirdetesCsoportDto> JaratInditas(int jaratId);

        public Task MegjelenitettHirdetesekKonyvelese(int jaratId, List<MegjelenitettHirdetesDto> megjelenitettHirdetesek);
    }
}
