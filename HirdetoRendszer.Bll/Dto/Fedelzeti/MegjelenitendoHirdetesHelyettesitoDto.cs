using HirdetoRendszer.Bll.Dto.Common;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Fedelzeti {
    public class MegjelenitendoHirdetesHelyettesitoDto {
        public int HirdetesHelyettesitoId { get; set; }

        public List<string> KepUrlek { get; set; }

        public List<IdotartamDto> EloirtIdotartamok { get; set; } = new List<IdotartamDto>();
    }
}
