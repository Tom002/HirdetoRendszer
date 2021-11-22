using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesHelyettesitoToJarmu
    {
        public int HirdetesHelyettesitoId { get; set; }

        [ForeignKey("HirdetesHelyettesitoId")]
        public HirdetesHelyettesito HirdetesHelyettesito { get; set; }

        public int JarmuId { get; set; }

        [ForeignKey("JarmuId")]
        public Jarmu Jarmu { get; set; }
    }
}
