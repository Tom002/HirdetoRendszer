using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.Auth
{
    public class HirdetoRegisztracioDto
    {
        [Required]
        [StringLength(100)]
        public string VezetekNev { get; set; }

        [Required]
        [StringLength(100)]
        public string KeresztNev { get; set; }

        [Required]
        [StringLength(100)]
        public string CegNev { get; set; }

        [Required]
        [StringLength(200)]
        public string CegCim { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; }
        [Required]
        public string Jelszo { get; set; }
        [Required]
        [Compare("Jelszo", ErrorMessage = "Password and Confirmation Password must match.")]
        public string JelszoUjra { get; set; }
    }
}
