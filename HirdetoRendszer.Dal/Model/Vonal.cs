using HirdetoRendszer.Common.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HirdetoRendszer.Dal.Model
{
    public class Vonal
    {
        [Key]
        public int VonalId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nev { get; set; }

        public JarmuTipus JarmuTipus { get; set; }

        public ICollection<AllomasToVonal> AllomasToVonal { get; set; } = new List<AllomasToVonal>();

        public ICollection<HirdetesToVonal> HirdetesToVonal { get; set; } = new List<HirdetesToVonal>();
    }
}
