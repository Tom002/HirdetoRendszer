using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Dal.Model
{
    public class Felhasznalo : IdentityUser<int>
    {
        [Required]
        [StringLength(100)]
        public string VezetekNev { get; set; }

        [Required]
        [StringLength(100)]
        public string KeresztNev { get; set; }

        [StringLength(100)]
        public string CegNev { get; set; }

        [StringLength(200)]
        public string CegCim { get; set; }
    }
}
