using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesHelyettesito
    {
        [Key]
        public int HirdetesHelyettesitoId { get; set; }

        [Required]
        public bool Aktiv { get; set; }

        public DateTime? ErvenyessegKezdet { get; set; }

        public DateTime? ErvenyessegVeg { get; set; }

        public ICollection<HirdetesHelyettesitoKep> HirdetesHelyettesitoKepek { get; set; }

        public ICollection<HirdetesHelyettesitoToJarmu> HirdetesHelyettesitokToJarmuvek { get; set; }
    }
}
