using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesHelyettesito
    {
        [Key]
        public int HirdetesHelyettesitoId { get; set; }

        [Required]
        public bool Aktiv { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan? ErvenyessegKezdet { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan? ErvenyessegVeg { get; set; }

        public ICollection<KepToHirdetesHelyettesito> HirdetesHelyettesitoKepek { get; set; } = new List<KepToHirdetesHelyettesito>();

        public ICollection<HirdetesHelyettesitoToJarmu> HirdetesHelyettesitokToJarmuvek { get; set; } = new List<HirdetesHelyettesitoToJarmu>();
    }
}
