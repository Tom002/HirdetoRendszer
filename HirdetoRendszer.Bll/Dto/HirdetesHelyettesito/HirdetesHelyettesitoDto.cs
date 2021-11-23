using HirdetoRendszer.Bll.Dto.Jarmu;
using HirdetoRendszer.Bll.Dto.Kep;
using System;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Hirdetes
{
    public class HirdetesHelyettesitoDto
    {
        public int HirdetesHelyettesitoId { get; set; }

        public TimeSpan? ErvenyessegKezdet { get; set; }

        public TimeSpan? ErvenyessegVeg { get; set; }

        public List<KepDto> Kepek { get; set; } = new List<KepDto>();

        public List<JarmuDto> Jarmuvek { get; set; } = new List<JarmuDto>();
    }
}
