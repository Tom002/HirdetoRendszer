﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class Hirdetes
    {
        [Key]
        public int HirdetesId { get; set; }

        public ICollection<HirdetesToVonal> HirdetesToVonal { get; set; } = new List<HirdetesToVonal>();

        public int FelhasznaloId { get; set; }
        [ForeignKey("FelhasznaloId")]
        public Felhasznalo Felhasznalo { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan? ErvenyessegKezdet { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan? ErvenyessegVeg { get; set; }

        public ICollection<KepToHirdetes> HirdetesKepek { get; set; } = new List<KepToHirdetes>();

        public ICollection<HirdetesFolyamatban> HirdetesekFolyamatban { get; set; } = new List<HirdetesFolyamatban>();

        public Elofizetes Elofizetes { get; set; }
    }
}