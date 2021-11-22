using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class HaviElofizetesReszletek
    {
        [Key]
        public int HaviElofizetesReszletekId { get; set; }

        
        public int HaviElofizetesId { get; set; }

        [ForeignKey("HaviElofizetesId")]
        public HaviElofizetes HaviElofizetes { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Honap { get; set; }

        public int ElhasznaltPercek { get; set; }
    }
}
