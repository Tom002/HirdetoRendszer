using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesToVonal
    {
        [ForeignKey("HirdetesId")]
        public Hirdetes Hirdetes { get; set; }

        public int HirdetesId { get; set; }

        [ForeignKey("VonalId")]
        public Vonal Vonal { get; set; }

        public int VonalId { get; set; }
    }
}
