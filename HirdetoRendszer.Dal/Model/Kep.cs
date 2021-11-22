using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class Kep
    {
        [Key]
        public int KepId { get; set; }

        [Required]
        [StringLength(200)]
        public string Url { get; set; }
    }
}
