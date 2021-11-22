using System.ComponentModel.DataAnnotations;

namespace HirdetoRendszer.Bll.Dto.Auth
{
    public class BejelentkezesDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Jelszo { get; set; }
    }
}
