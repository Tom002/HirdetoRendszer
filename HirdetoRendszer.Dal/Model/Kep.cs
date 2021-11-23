using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class Kep
    {
        [Key]
        public int KepId { get; set; }

        [Required]
        [StringLength(200)]
        public string Url { get; set; }

        public int FeltoltoFelhasznaloId { get; set; }

        [ForeignKey("FeltoltoFelhasznaloId")]
        public Felhasznalo FeltoltoFelhasznalo { get; set; }

        public ICollection<KepToHirdetes> KepToHirdetes { get; set; } = new List<KepToHirdetes>();

        public ICollection<KepToHirdetesHelyettesito> KepToHirdetesHelyettesito { get; set; } = new List<KepToHirdetesHelyettesito>();
    }
}
