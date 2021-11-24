using HirdetoRendszer.Bll.Dto.Common;
using HirdetoRendszer.Bll.Dto.Jarmu;
using HirdetoRendszer.Bll.Dto.Kep;
using System;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Hirdetes
{
    public class HirdetesHelyettesitoDto
    {
        public int HirdetesHelyettesitoId { get; set; }

        public IdotartamDto Ervenyesseg { get; set; }

        public List<KepDto> HirdetesHelyettesitoKepek { get; set; } = new List<KepDto>();

        public List<JarmuDto> HirdetesHelyettesitokToJarmuvek { get; set; } = new List<JarmuDto>();
    }
}
