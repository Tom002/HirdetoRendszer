using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesHelyettesitoKep : Kep
    {
        public int HirdetesHelyettesitoId { get; set; }

        [ForeignKey("HirdetesHelyettesitoId")]
        public HirdetesHelyettesito HirdetesHelyettesito { get; set; }
    }
}
