using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface ISeedService
    {
        Task SeedSzerepkorok();

        Task SeedFelhasznalok();

        Task SeedAllomasokAndVonalak();

        Task SeedHirdetesKepek();

        Task SeedJarmuvek();

        Task SeedHirdetesHelyettesitok();

        Task SeedHirdetesek();

        Task SeedJaratok();
    }
}
