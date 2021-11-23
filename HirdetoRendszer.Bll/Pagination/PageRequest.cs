using HirdetoRendszer.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Bll.Pagination
{
    public class PageRequest
    {
        [Range(1, 50)]
        public int OldalMeret { get; set; } = 10;

        [Range(1, int.MaxValue)]
        public int OldalSzam { get; set; } = 1;

        public string Rendezes { get; set; }

        public RendezesIrany RendezesIrany { get; set; } = RendezesIrany.Novekvo;
    }
}
