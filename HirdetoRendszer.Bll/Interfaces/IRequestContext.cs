using HirdetoRendszer.Common.Enum;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IRequestContext
    {
        public int FelhasznaloId { get; }

        public string FelhasznaloEmail { get; }

        public FelhasznaloTipus FelhasznaloTipus { get; set; }
    }
}
