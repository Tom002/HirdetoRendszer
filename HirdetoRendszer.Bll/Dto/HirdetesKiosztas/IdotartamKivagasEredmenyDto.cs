using HirdetoRendszer.Bll.Dto.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.HirdetesKiosztas
{
    public class IdotartamKivagasEredmenyDto
    {
        public List<IdotartamDto> KivagasUtaniIdotartamok { get; set; }

        public List<IdotartamDto> KivagottIdotartamok { get; set; }
    }
}
