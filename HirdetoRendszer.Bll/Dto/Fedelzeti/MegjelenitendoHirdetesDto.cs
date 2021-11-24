using HirdetoRendszer.Bll.Dto.Common;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Fedelzeti {
    public class MegjelenitendoHirdetesDto
        {
        public int HirdetesId { get; set; }

        public string KepUrl { get; set; }

        public double MegjelenitesiSzazalek { get; set; }

        public List<IdotartamDto> engedelyezettIdotartamok { get; set; } = new List<IdotartamDto>();
    }
}
