using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class Hirdetes
    {
        [Key]
        public int HirdetesId { get; set; }

        public int VonalId { get; set; }
        [ForeignKey("VonalId")]
        public Vonal Vonal { get; set; }

        public int FelhasznaloId { get; set; }
        [ForeignKey("FelhasznaloId")]
        public Felhasznalo Felhasznalo { get; set; }

        public DateTime? ErvenyessegKezdet { get; set; }

        public DateTime? ErvenyessegVeg { get; set; }

        public ICollection<HirdetesKep> HirdetesKepek { get; set; } = new List<HirdetesKep>();

        public ICollection<HirdetesFolyamatban> HirdetesekFolyamatban { get; set; } = new List<HirdetesFolyamatban>();

        public Elofizetes Elofizetes { get; set; }
    }
}
