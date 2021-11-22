using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class Vonal
    {
        [Key]
        public int VonalId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nev { get; set; }
    }
}
