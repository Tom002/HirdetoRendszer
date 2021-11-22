using HirdetoRendszer.Common.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public FelhasznaloTipus FelhasznaloTipus { get; set; }
    }
}
