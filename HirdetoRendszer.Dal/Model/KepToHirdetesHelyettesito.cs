using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class KepToHirdetesHelyettesito
    {
        [ForeignKey("KepId")]
        public Kep Kep { get; set; }
        public int KepId { get; set; }

        [ForeignKey("HirdetesHelyettesitoId")]
        public HirdetesHelyettesito HirdetesHelyettesito { get; set; }
        public int HirdetesHelyettesitoId { get; set; }
    }
}
