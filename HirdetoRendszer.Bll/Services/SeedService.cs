using Bogus;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Dal.DbContext;
using HirdetoRendszer.Dal.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class SeedService : ISeedService
    {
        private readonly UserManager<Felhasznalo> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly HirdetoRendszerDbContext _dbContext;

        private int allomasokSzama = 100;

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
                    Email = "hirdeto@test.hu",
                    UserName = "hirdeto@test.hu",
                    KeresztNev = "Béla",
                    VezetekNev = "Hirdető",
                    EmailConfirmed = true,
                    CegCim = "1111 Kis utca 6.",
                    CegNev = "Sörgyár Kft.",
                    FelhasznaloTipus = FelhasznaloTipus.Hirdeto
                };
                await _userManager.CreateAsync(hirdeto, "passWORD123!");
                await _userManager.AddToRoleAsync(hirdeto, FelhasznaloTipus.Hirdeto.ToString());
                await _dbContext.SaveChangesAsync();

                var kozlekedesiVallalat = new Felhasznalo
                {
                    Email = "bkv@test.hu",
                    UserName = "bkv@test.hu",
                    KeresztNev = "Bálint",
                    VezetekNev = "Nagy",
                    EmailConfirmed = true,
                    FelhasznaloTipus = FelhasznaloTipus.KozlekedesiVallalat
                };
                await _userManager.CreateAsync(kozlekedesiVallalat, "passWORD123!");
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
                await _userManager.CreateAsync(hirdetesKezelo, "passWORD123!");
                await _userManager.AddToRoleAsync(kozlekedesiVallalat, FelhasznaloTipus.HirdetesSzervezoCeg.ToString());
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SeedAllomasok()
        {
            if (!await _dbContext.Allomasok.AnyAsync())
            {
                var allomasFaker = new Faker<Allomas>()
                .RuleFor(a => a.Nev, f => f.Address.StreetName());

                for (int i = 0; i < allomasokSzama; i++)
                {
                    var allomas = allomasFaker.Generate();

                    _dbContext.Allomasok.Add(allomas);
                }

                await _dbContext.SaveChangesAsync();
            }
        }

        //public async Task SeedVonalak()
        //{
        //    for (int i = 0; i < allomasokSzama; i++)
        //    {
        //        var allomas = allomasFaker.Generate();

        //        _dbContext.Allomasok.Add(allomas);
        //    }

        //    await _dbContext.SaveChangesAsync();
        //}

        //public async Task SeedJarmuvek()
        //{

        //}
    }
}
