using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HirdetoRendszer.Bll.Dto.Hirdeto
{
    public class HirdetoEngedelyezesDto
    {
        [Required]
        public bool Engedelyezett { get; set; }
    }
}
