using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class HirdetesKep : Kep
    {
        public int HirdetesId { get; set; }

        [ForeignKey("HirdetesId")]
        public Hirdetes Hirdetes { get; set; }
    }
}
