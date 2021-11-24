using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.Fedelzeti {
    public class MegjelenitendoHirdetesDto
        {
        public int HirdetesId { get; set; }

        public string KepUrl { get; set; }

        public double MegjelenitesiSzazalek { get; set; }

        // TODO: engedélyezett időtartamok
    }
}
