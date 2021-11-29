using HirdetoRendszer.Common.Enum;

namespace HirdetoRendszer.Bll.Dto.Jarmu {
    public class JarmuDto {
        public int JarmuId { get; set; }

        public string Azonosito { get; set; }

        public JarmuTipus JarmuTipus { get; set; }
    }
}
