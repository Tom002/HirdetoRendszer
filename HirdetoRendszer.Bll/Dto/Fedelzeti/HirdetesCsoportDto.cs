using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Fedelzeti {
    public class HirdetesCsoportDto {
        public int TervezettMenetidoPerc { get; set; }

        public List<MegjelenitendoHirdetesDto> MegjelenitendoKepek { get; set; } = new List<MegjelenitendoHirdetesDto>();

        public List<MegjelenitendoHirdetesHelyettesitoDto> MegjelenitendoHirdetesHelyettesitok { get; set; } = new List<MegjelenitendoHirdetesHelyettesitoDto>();
    }
}
