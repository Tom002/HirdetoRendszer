using HirdetoRendszer.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesHelyettesito : ISoftDelete
    {
        [Key]
        public int HirdetesHelyettesitoId { get; set; }

        [Required]
        public bool Aktiv { get; set; }

        public bool IdohozKotott { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan? ErvenyessegKezdet { get; set; }

        [Column(TypeName = "bigint")]
        public TimeSpan? ErvenyessegVeg { get; set; }

        public ICollection<KepToHirdetesHelyettesito> HirdetesHelyettesitoKepek { get; set; } = new List<KepToHirdetesHelyettesito>();

        public bool MindenJarmure { get; set; }

        public ICollection<HirdetesHelyettesitoToJarmu> HirdetesHelyettesitokToJarmuvek { get; set; } = new List<HirdetesHelyettesitoToJarmu>();

        public bool SoftDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; }
    }
}
