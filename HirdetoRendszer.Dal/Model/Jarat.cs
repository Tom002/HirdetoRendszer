using HirdetoRendszer.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirdetoRendszer.Dal.Model
{
    public class Jarat
    {
        [Key]
        public int JaratId { get; set; }

        public int VonalId { get; set; }
        [ForeignKey("VonalId")]
        public Vonal Vonal { get; set; }

        public int JarmuId { get; set; }
        [ForeignKey("JarmuId")]
        public Jarmu Jarmu { get; set; }

        [Column(TypeName = "Date")]
        public DateTime Datum { get; set; }

        public TimeSpan JaratIndulas { get; set; }

        public TimeSpan JaratErkezes { get; set; }

        public ICollection<HirdetesFolyamatban> HirdetesekFolyamatban { get; set; } = new List<HirdetesFolyamatban>();

        public JaratAllapot JaratAllapot { get; set; } = JaratAllapot.IndulasElott;
    }
}
