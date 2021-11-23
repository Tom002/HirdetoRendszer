using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class AllomasToVonal
    {
        [ForeignKey("AllomasId")]
        public Allomas Allomas { get; set; }
        public int AllomasId { get; set; }

        [ForeignKey("VonalId")]
        public Vonal Vonal { get; set; }
        public int VonalId { get; set; }

        public int Sorrend { get; set; }
    }
}
