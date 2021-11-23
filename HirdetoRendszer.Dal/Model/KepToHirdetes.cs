using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class KepToHirdetes
    {
        [ForeignKey("KepId")]
        public Kep Kep { get; set; }
        public int KepId { get; set; }

        [ForeignKey("HirdetesId")]
        public Hirdetes Hirdetes { get; set; }
        public int HirdetesId { get; set; }
    }
}
