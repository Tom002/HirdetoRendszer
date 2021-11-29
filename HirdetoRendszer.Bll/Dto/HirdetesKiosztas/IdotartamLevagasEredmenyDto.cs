using HirdetoRendszer.Bll.Dto.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.HirdetesKiosztas
{
    public class IdotartamLevagasEredmenyDto
    {
        public List<IdotartamDto> LevagottIdotartamokHirdetesnek { get; set; }

        public List<IdotartamDto> MaradekSzabadIdotartamok { get; set; }
    }
}
