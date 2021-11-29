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
        private readonly IAuthService _authService;

        private int vonalakSzama = 20;
        private int allomasVonalankent = 5;
        private string hirdetoTestFiok = "hirdeto@test.hu";
        private string kozlekedesiVallalatTestFiok = "bkv@test.hu";
        private string tesztJelszo = "passWORD123!";
        private List<int> picsumKepIdLista = new List<int> { 0, 10, 1000, 1002, 1004, 1006, 1009, 1010, 1012, 1014, 1016, 1019, 1020, 1022, 1024 };

        public SeedService(
            UserManager<Felhasznalo> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            HirdetoRendszerDbContext dbContext,
            IAuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _authService = authService;
        }

        private static List<string> GetRoles()
        {
            return new List<string>
            {
                FelhasznaloTipus.Hirdeto.ToString(),
                FelhasznaloTipus.KozlekedesiVallalat.ToString(),
                FelhasznaloTipus.HirdetesSzervezoCeg.ToString(),
                FelhasznaloTipus.FedelzetiRendszer.ToString(),
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

                var hirdetesKezelo = new Felhasznalo {
                    Email = "hirdetesKezelo@test.hu",
                    UserName = "hirdetesKezelo@test.hu",
                    KeresztNev = "Ádám",
                    VezetekNev = "Hirdetéskezelő",
                    EmailConfirmed = true,
                    FelhasznaloTipus = FelhasznaloTipus.HirdetesSzervezoCeg
                };
                await _userManager.CreateAsync(hirdetesKezelo, tesztJelszo);
                await _userManager.AddToRoleAsync(hirdetesKezelo, FelhasznaloTipus.HirdetesSzervezoCeg.ToString());
                await _dbContext.SaveChangesAsync();

                var fedelzetiRendszer = new Felhasznalo {
                    Email = "fedelzet@test.hu",
                    UserName = "fedelzet@test.hu",
                    KeresztNev = "FR-001",
                    VezetekNev = "",
                    EmailConfirmed = true,
                    FelhasznaloTipus = FelhasznaloTipus.FedelzetiRendszer
                };
                await _userManager.CreateAsync(fedelzetiRendszer, tesztJelszo);
                await _userManager.AddToRoleAsync(fedelzetiRendszer, FelhasznaloTipus.FedelzetiRendszer.ToString());
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

        public async Task SeedHirdetesek() {
            var hirdeto = await _authService.GetFelhasznaloByEmail("hirdeto@test.hu");
            if (hirdeto is null) {
                return;
            }
            Console.WriteLine(hirdeto.Id);

            var haviElofizetesFaker = new Faker<HaviElofizetes>()
                .RuleFor(he => he.HaviLimit, f => f.Random.Number(0, 1000))
                .RuleFor(he => he.Aktiv, f => f.Random.Bool(0.3f));

            var haviElofizetesReszletekFaker = new Faker<HaviElofizetesReszletek>()
                .RuleFor(her => her.ElhasznaltPercek, f => f.Random.Number(20, 120))
                .RuleFor(her => her.Honap, f => f.Date.Past(1));

            var mennyisegiElofizetesFaker = new Faker<MennyisegiElofizetes>()
                .RuleFor(me => me.ElhasznaltIdotartam, f => f.Random.Number(0, 1000))
                .RuleFor(me => me.VasaroltIdotartam, (f, current) => f.Random.Number(current.ElhasznaltIdotartam, 1100));

            var hirdetesFaker = new Faker<Hirdetes>()
                .RuleFor(h => h.IdohozKotott, _ => true)
                .RuleFor(h => h.ErvenyessegKezdet, f => new TimeSpan(0, f.Random.Number(0, 22), f.Random.Number(0, 59), 0, 0))
                .RuleFor(h => h.ErvenyessegVeg, (f, current) => new TimeSpan(0, f.Random.Number(current.ErvenyessegKezdet.Value.Hours + 1, 23), f.Random.Number(0, 59), 0, 0));

            for (int i = 0; i < 5; i++) {
                var mennyisegiElofizetes = mennyisegiElofizetesFaker.Generate();
                _dbContext.MennyisegiElofizetesek.Add(mennyisegiElofizetes);
                var hirdetes = hirdetesFaker.Generate();
                hirdetes.Elofizetes = mennyisegiElofizetes;
                hirdetes.Felhasznalo = hirdeto;
                _dbContext.Hirdetesek.Add(hirdetes);

                var kepek = await _dbContext.Kepek.Where(k => k.FeltoltoFelhasznalo == hirdeto).ToListAsync();
                var kivalasztottKepek = new Faker().PickRandom(kepek, kepek.Count / 2);

                foreach (var kivalasztottKep in kivalasztottKepek) {
                    var kepToHirdetes = new KepToHirdetes() {
                        Hirdetes = hirdetes,
                        Kep = kivalasztottKep,
                    };
                    _dbContext.KepToHirdetes.Add(kepToHirdetes);
                }

                var vonalak = await _dbContext.Vonalak.ToListAsync();
                var kivalasztottVonalak = new Faker().PickRandom(vonalak, vonalak.Count / 2);
                foreach (var kivalasztottVonal in kivalasztottVonalak) {
                    var hirdetesToVonal = new HirdetesToVonal() { 
                        Hirdetes = hirdetes,
                        Vonal = kivalasztottVonal,
                    };
                    _dbContext.HirdetesToVonal.Add(hirdetesToVonal);
                }
            }

            for (int i = 0; i < 5; i++) {
                var haviElofizetes = haviElofizetesFaker.Generate();
                _dbContext.HaviElofizetesek.Add(haviElofizetes);
                var hirdetes = hirdetesFaker.Generate();
                hirdetes.Elofizetes = haviElofizetes;
                hirdetes.Felhasznalo = hirdeto;
                _dbContext.Hirdetesek.Add(hirdetes);

                var kepek = await _dbContext.Kepek.Where(k => k.FeltoltoFelhasznalo == hirdeto).ToListAsync();
                var kivalasztottKepek = new Faker().PickRandom(kepek, kepek.Count / 2);

                foreach (var kivalasztottKep in kivalasztottKepek) {
                    var kepToHirdetes = new KepToHirdetes() {
                        Hirdetes = hirdetes,
                        Kep = kivalasztottKep,
                    };
                    _dbContext.KepToHirdetes.Add(kepToHirdetes);
                }

                for (int j = 0; j < 10; j++) {
                    var haviElofizetesReszletek = haviElofizetesReszletekFaker.Generate();
                    haviElofizetesReszletek.HaviElofizetes = haviElofizetes;
                    _dbContext.HaviElofizetesReszletek.Add(haviElofizetesReszletek);
                }

                var vonalak = await _dbContext.Vonalak.ToListAsync();
                var kivalasztottVonalak = new Faker().PickRandom(vonalak, vonalak.Count / 2);
                foreach (var kivalasztottVonal in kivalasztottVonalak) {
                    var hirdetesToVonal = new HirdetesToVonal() {
                        Hirdetes = hirdetes,
                        Vonal = kivalasztottVonal,
                    };
                    _dbContext.HirdetesToVonal.Add(hirdetesToVonal);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task SeedHirdetesHelyettesitok()
        {
            var kozlekedesiVallalat = await _authService.GetFelhasznaloByEmail(kozlekedesiVallalatTestFiok);
            if (kozlekedesiVallalat is null)
            {
                return;
            }

            var jarmuIdk = await _dbContext.Jarmuvek.Select(j => j.JarmuId).ToListAsync();
            var hirdetesHelyettesitoKepIdk = await _dbContext.Kepek
                .Where(k => k.FeltoltoFelhasznaloId == kozlekedesiVallalat.Id)
                .Select(k => k.KepId)
                .ToListAsync();

            var hirdetesHelyettesitoFaker = new Faker<HirdetesHelyettesito>()
                .RuleFor(h => h.Aktiv, _ => true)
                .RuleFor(h => h.CreatedAt, _ => DateTime.Now)
                .RuleFor(h => h.MindenJarmure, _ => true)
                .RuleFor(h => h.IdohozKotott, _ => true)
                .RuleFor(h => h.ErvenyessegKezdet, f => new TimeSpan(0, f.Random.Number(0, 22), f.Random.Number(0, 59), 0, 0))
                .RuleFor(j => j.ErvenyessegVeg, (f, current) => new TimeSpan(0, f.Random.Number(current.ErvenyessegKezdet.Value.Hours + 1, Math.Min(23, current.ErvenyessegKezdet.Value.Hours + 5)), f.Random.Number(0, 59), 0, 0))
                .RuleFor(j => j.HirdetesHelyettesitoKepek, (f, current) => new List<KepToHirdetesHelyettesito>(
                    f.PickRandom(hirdetesHelyettesitoKepIdk, f.Random.Number(0, hirdetesHelyettesitoKepIdk.Count)).Select(id =>
                        new KepToHirdetesHelyettesito
                        {
                            KepId = id,
                            HirdetesHelyettesito = current
                        }
                    )
                ));

            for (int i = 0; i < 5; i++)
            {
                var hirdetesHelyettesito = hirdetesHelyettesitoFaker.Generate();
                _dbContext.HirdetesHelyettesitok.Add(hirdetesHelyettesito);
            }

            await _dbContext.SaveChangesAsync();
        }


        public async Task SeedJaratok() {
            var jarmuIdk = await _dbContext.Jarmuvek.Select(j => j.JarmuId).ToListAsync();
            var vonalIdk = await _dbContext.Vonalak.Select(j => j.VonalId).ToListAsync();
            var hirdetesIdk = await _dbContext.Hirdetesek.Select(j => j.HirdetesId).ToListAsync();

            var jaratFaker = new Faker<Jarat>()
                .RuleFor(j => j.Datum, f => f.Date.Future(yearsToGoForward: 1))
                .RuleFor(j => j.JaratIndulas, f => new TimeSpan(0, f.Random.Number(0, 22), f.Random.Number(0, 59), 0, 0))
                .RuleFor(j => j.JaratErkezes, (f, current) => new TimeSpan(0, f.Random.Number(current.JaratIndulas.Hours + 1, 23), f.Random.Number(0, 59), 0, 0))
                .RuleFor(j => j.JarmuId, f => f.PickRandom(jarmuIdk))
                .RuleFor(j => j.VonalId, f => f.PickRandom(vonalIdk));

            for (int i = 0; i < 5; i++) {
                var jarat = jaratFaker.Generate();
                _dbContext.Jaratok.Add(jarat);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
