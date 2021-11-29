using HirdetoRendszer.Bll.Dto.Common;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Fedelzeti {
    public class MegjelenitendoHirdetesDto
        {
        public int HirdetesId { get; set; }

        public List<string> KepUrlek { get; set; }

        public double MegjelenitesiSzazalek { get; set; }

        public int MaxMegjelenitesPerc { get; set; }

        public List<IdotartamDto> EngedelyezettIdotartamok { get; set; } = new List<IdotartamDto>();
    }
}
