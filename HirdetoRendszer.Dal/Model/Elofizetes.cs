using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class Elofizetes
    {
        [Key]
        public int ElofizetesId { get; set; }

        [ForeignKey("HirdetesId")]
        public Hirdetes Hirdetes { get; set; }

        public int HirdetesId { get; set; }
    }
}
