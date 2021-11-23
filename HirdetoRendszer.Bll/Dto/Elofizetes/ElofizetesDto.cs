using HirdetoRendszer.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.Elofizetes
{
    public class ElofizetesDto
    {
        public ElofizetesTipus ElofizetesTipus { get; set; }

        public bool? Aktiv { get; set; }

        public int? HaviLimit { get; set; }

        public int? VasaroltIdotartam { get; set; }

        public int ElhasznaltIdotartam { get; set; }

    }
}
