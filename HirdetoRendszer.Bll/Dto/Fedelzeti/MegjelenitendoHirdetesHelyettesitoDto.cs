using HirdetoRendszer.Bll.Dto.Common;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Fedelzeti {
    public class MegjelenitendoHirdetesHelyettesitoDto {
        public int HirdetesHelyettesitoId { get; set; }

        public string KepUrl { get; set; }

        public List<IdotartamDto> eloirtIdotartamok { get; set; } = new List<IdotartamDto>();
    }
}
