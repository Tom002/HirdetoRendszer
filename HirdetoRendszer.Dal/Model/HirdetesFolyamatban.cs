using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesFolyamatban
    {
        public int JaratId { get; set; }
        [ForeignKey("JaratId")]
        public Jarat Jarat { get; set; }

        public int HirdetesId { get; set; }
        [ForeignKey("HirdetesId")]
        public Hirdetes Hirdetes { get; set; }

        public int LefoglaltPerc { get; set; }
    }
}
