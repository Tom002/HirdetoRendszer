using Bogus;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Dal.DbContext;
using HirdetoRendszer.Dal.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class SeedService : ISeedService
    {
        private readonly UserManager<Felhasznalo> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly HirdetoRendszerDbContext _dbContext;

        private int vonalakSzama = 20;
        private int allomasVonalankent = 5;
        private string hirdetoTestFiok = "hirdeto@test.hu";
        private string kozlekedesiVallalatTestFiok = "bkv@test.hu";
        private string tesztJelszo = "passWORD123!";
        private List<int> picsumKepIdLista = new List<int> { 0, 10, 1000, 1002, 1004, 1006, 1009, 1010, 1012, 1014, 1016, 1019, 1020, 1022, 1024 };

        public SeedService(
            UserManager<Felhasznalo> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            HirdetoRendszerDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        private static List<string> GetRoles()
        {
            return new List<string>
            {
                FelhasznaloTipus.Hirdeto.ToString(),
                FelhasznaloTipus.KozlekedesiVallalat.ToString(),
                FelhasznaloTipus.HirdetesSzervezoCeg.ToString()
            };
        }

        public async Task SeedSzerepkorok()
        {
            foreach (var roleName in GetRoles())
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }

        public async Task SeedFelhasznalok()
        {
            if (!await _dbContext.Users.AnyAsync())
            {
                var hirdeto = new Felhasznalo
                {
                    Email = hirdetoTestFiok,
                    UserName = hirdetoTestFiok,
                    KeresztNev = "Béla",
                    VezetekNev = "Hirdető",
                    EmailConfirmed = true,
                    CegCim = "1111 Kis utca 6.",
                    CegNev = "Sörgyár Kft.",
                    FelhasznaloTipus = FelhasznaloTipus.Hirdeto
                };
                await _userManager.CreateAsync(hirdeto, tesztJelszo);
                await _userManager.AddToRoleAsync(hirdeto, FelhasznaloTipus.Hirdeto.ToString());
                await _dbContext.SaveChangesAsync();

                var kozlekedesiVallalat = new Felhasznalo
                {
                    Email = kozlekedesiVallalatTestFiok,
                    UserName = kozlekedesiVallalatTestFiok,
                    KeresztNev = "Bálint",
                    VezetekNev = "Nagy",
                    EmailConfirmed = true,
                    FelhasznaloTipus = FelhasznaloTipus.KozlekedesiVallalat
                };
                await _userManager.CreateAsync(kozlekedesiVallalat, tesztJelszo);
                await _userManager.AddToRoleAsync(kozlekedesiVallalat, FelhasznaloTipus.KozlekedesiVallalat.ToString());
                await _dbContext.SaveChangesAsync();

                var hirdetesKezelo = new Felhasznalo
                {
                    Email = "hirdetesKezelo@test.hu",
                    UserName = "hirdetesKezelo@test.hu",
                    KeresztNev = "Ádám",
                    VezetekNev = "Hirdetéskezelő",
                    EmailConfirmed = true,
                    FelhasznaloTipus = FelhasznaloTipus.HirdetesSzervezoCeg
                };
                await _userManager.CreateAsync(hirdetesKezelo, tesztJelszo);
                await _userManager.AddToRoleAsync(kozlekedesiVallalat, FelhasznaloTipus.HirdetesSzervezoCeg.ToString());
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SeedAllomasokAndVonalak()
        {
            var random = new Random();

            if (!await _dbContext.Allomasok.AnyAsync() && !await _dbContext.Vonalak.AnyAsync())
            {
                var allomasFaker = new Faker<Allomas>().RuleFor(a => a.Nev, f => f.Address.StreetName());
                var vonalFaker = new Faker<Vonal>().RuleFor(v => v.JarmuTipus, f => f.PickRandom<JarmuTipus>());

                for (int i = 0; i < vonalakSzama; i++)
                {
                    var vonal = vonalFaker.Generate();
                    vonal.Nev = $"{vonal.JarmuTipus} {random.Next(100, 999)}";

                    for (int j = 0; j < allomasVonalankent; j++)
                    {
                        var allomas = allomasFaker.Generate();
                        _dbContext.AllomasToVonal.Add(
                            new AllomasToVonal
                            {
                                Allomas = allomas,
                                Vonal = vonal
                            });
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SeedHirdetesKepek()
        {
            var hirdetoTestUser = await _dbContext.Users
                .SingleAsync(h => h.UserName == hirdetoTestFiok);

            var kozlekedesiVallalat = await _dbContext.Users
                .SingleAsync(k => k.UserName == kozlekedesiVallalatTestFiok);

            var hirdeteskepIdLista = picsumKepIdLista.Take(8).ToList();
            foreach (var hirdeteskepId in hirdeteskepIdLista)
            {
                _dbContext.Kepek.Add(new Kep
                {
                    FeltoltoFelhasznaloId = hirdetoTestUser.Id,
                    Url = $"https://picsum.photos/id/{hirdeteskepId}/300/300"
                });
            }

            var helyettesitoKepIdLista = picsumKepIdLista.Skip(8).ToList();
            foreach (var helyettesitoKepId in helyettesitoKepIdLista)
            {
                _dbContext.Kepek.Add(new Kep
                {
                    FeltoltoFelhasznaloId = kozlekedesiVallalat.Id,
                    Url = $"https://picsum.photos/id/{helyettesitoKepId}/300/300?grayscale"
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task SeedJarmuvek()
        {
            var jarmuFaker = new Faker<Jarmu>()
                .RuleFor(j => j.Azonosito, f => f.Vehicle.Vin())
                .RuleFor(j => j.JarmuTipus, f => f.PickRandom<JarmuTipus>());

            for (int i = 0; i < 10; i++)
            {
                var jarmu = jarmuFaker.Generate();

                _dbContext.Jarmuvek.Add(jarmu);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
