﻿using HirdetoRendszer.Bll.Dto.Elofizetes;
using HirdetoRendszer.Bll.Dto.Kep;
using HirdetoRendszer.Bll.Dto.Vonal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.Hirdetes
{
    public class HirdetesDto
    {
        public int HirdetesId { get; set; }

        public int FelhasznaloId { get; set; }

        public TimeSpan? ErvenyessegKezdet { get; set; }

        public TimeSpan? ErvenyessegVeg { get; set; }

        public List<KepDto> HirdetesKepek { get; set; } = new List<KepDto>();

        public List<VonalDto> Vonalak { get; set; } = new List<VonalDto>();

        public ElofizetesDto Elofizetes { get; set; }
    }
}