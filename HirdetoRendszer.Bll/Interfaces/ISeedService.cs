﻿using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface ISeedService
    {
        public Task SeedSzerepkorok();

        public Task SeedFelhasznalok();

        public Task SeedAllomasokAndVonalak();

        public Task SeedHirdetesKepek();

        public Task SeedJarmuvek();
        Task SeedHirdetesek();
        Task SeedJaratok();
    }
}
