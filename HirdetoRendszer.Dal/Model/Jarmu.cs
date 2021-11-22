using HirdetoRendszer.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HirdetoRendszer.Dal.Model
{
    public class Jarmu
    {
        [Key]
        public int JarmuId { get; set; }

        [Required]
        [StringLength(10)]
        public string Azonosito { get; set; }

        public JarmuTipus JarmuTipus { get; set; }

        public ICollection<HirdetesHelyettesitoToJarmu> HirdetesHelyettesitokToJarmuvek { get; set; }
    }
}
