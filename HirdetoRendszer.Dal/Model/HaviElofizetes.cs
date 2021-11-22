using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class HaviElofizetes : Elofizetes
    {
        [Required]
        public bool Aktiv { get; set; }

        [Required]
        public int HaviLimit { get; set; }

        public List<HaviElofizetesReszletek> HaviElofizetesReszletek { get; set; } = new List<HaviElofizetesReszletek>();
    }
}
