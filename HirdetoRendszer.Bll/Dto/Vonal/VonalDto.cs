using HirdetoRendszer.Common.Enum;

namespace HirdetoRendszer.Bll.Dto.Vonal
{
    public class VonalDto
    {
        public int VonalId { get; set; }

        public string Nev { get; set; }

        public JarmuTipus JarmuTipus { get; set; }
    }
}
