using HirdetoRendszer.Common.Enum;
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
    }
}
