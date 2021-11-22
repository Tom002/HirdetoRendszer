using System;
using System.Collections.Generic;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class MennyisegiElofizetes : Elofizetes
    {
        public int VasaroltIdotartam { get; set; }

        public int ElhasznaltIdotartam { get; set; }
    }
}
