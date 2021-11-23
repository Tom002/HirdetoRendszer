using System.Collections.Generic;

namespace HirdetoRendszer.Bll.Dto.Hirdetes
{
    public class HirdetesHelyettesitoHozzaadasDto 
    {
        public int? ErvenyessegKezdetOra { get; set; }

        public int? ErvenyessegKezdetPerc { get; set; }

        public int? ErvenyessegVegOra { get; set; }

        public int? ErvenyessegVegPerc { get; set; }

        public List<int> JarmuIdLista { get; set; } = new List<int>();

        public List<int> KepIdLista { get; set; } = new List<int>();

    }
}
