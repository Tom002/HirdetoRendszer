using HirdetoRendszer.Common.Enum;
using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Hirdetes
{
    public class HirdetesHozzaadasDto 
    {
        public bool IdosavhozKotott { get; set; } = false;

        public int? ErvenyessegKezdetOra { get; set; }

        public int? ErvenyessegKezdetPerc { get; set; }

        public int? ErvenyessegVegOra { get; set; }

        public int? ErvenyessegVegPerc { get; set; }

        public List<int> VonalIdLista { get; set; } = new List<int>();

        public List<int> HirdetesKepIdLista { get; set; } = new List<int>();

        public ElofizetesTipus ElofizetesTipus { get; set; }

        public int? HaviLimit { get; set; }

        public int? VasaroltIdotartam { get; set; }
    }
}
