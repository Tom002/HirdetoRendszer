using HirdetoRendszer.Bll.Dto.Common;
using HirdetoRendszer.Bll.Dto.Fedelzeti;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Exceptions;
using HirdetoRendszer.Dal.DbContext;
using HirdetoRendszer.Dal.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class FedelzetiService : IFedelzetiService {
        private readonly HirdetoRendszerDbContext _dbContext;

        public FedelzetiService(HirdetoRendszerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HirdetesCsoportDto> JaratInditas(int jaratId) {
            var jarat = await _dbContext.Jaratok.FindAsync(jaratId)
                ?? throw new EntityNotFoundException($"Járat {jaratId} nem található");

            if (jarat.JaratAllapot != Common.Enum.JaratAllapot.IndulasElott)
                throw new BusinessException($"Járat {jaratId} már elindult korábban");

            jarat.JaratAllapot = Common.Enum.JaratAllapot.Uton;

            var menetidoPerc = jarat.JaratErkezes.TotalMinutes - jarat.JaratIndulas.TotalMinutes;

            var hirdetesCsoport = new HirdetesCsoportDto() {
                TervezettMenetidoPerc = (int)menetidoPerc
            };

            #region HirdetesHelyettesito
            var hirdetesHelyettesitokToJarmuvek = _dbContext.HirdetesHelyettesitoToJarmu
                .Where(hhj => hhj.JarmuId == jarat.JarmuId)
                .Select(hhj => hhj.HirdetesHelyettesitoId)
                .ToHashSet();

            var hirdetesHelyettesitok = await _dbContext.HirdetesHelyettesitok
                .Include(h => h.HirdetesHelyettesitoKepek).ThenInclude(hhk => hhk.Kep)
                .Where(hh => hh.Aktiv
                    && (
                        hh.MindenJarmure || hirdetesHelyettesitokToJarmuvek.Contains(hh.HirdetesHelyettesitoId)
                    )
                )
                .OrderBy(hh => hh.CreatedAt)
                .ToListAsync();

            var megjelenitendoHirdetesHelyettesitok = new List<MegjelenitendoHirdetesHelyettesitoDto>();

            foreach (var hirdetesHelyettesito in hirdetesHelyettesitok)
            {
                var megjelenitendoHirdetesHelyettesito = new MegjelenitendoHirdetesHelyettesitoDto()
                {
                    HirdetesHelyettesitoId = hirdetesHelyettesito.HirdetesHelyettesitoId,
                    KepUrlek = new List<string>(hirdetesHelyettesito.HirdetesHelyettesitoKepek.Select(hhk => hhk.Kep.Url)),
                    EloirtIdotartamok = new List<IdotartamDto>(),
                };

                if (hirdetesHelyettesito.IdohozKotott)
                {
                    megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.Add(new IdotartamDto()
                    {
                        Kezdet = hirdetesHelyettesito.ErvenyessegKezdet.Value,
                        Veg = hirdetesHelyettesito.ErvenyessegVeg.Value,
                    });
                }
                else
                {
                    megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.Add(new IdotartamDto()
                    {
                        Kezdet = new TimeSpan(0, 0, 0),
                        Veg = new TimeSpan(23, 59, 0),
                    });
                }

                for (int i = 0; i < megjelenitendoHirdetesHelyettesitok.Count; i++)
                {
                    var mmh = megjelenitendoHirdetesHelyettesitok[i];
                    mmh.EloirtIdotartamok = IdotartamKivagas(mmh.EloirtIdotartamok, megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.First());
                }

                megjelenitendoHirdetesHelyettesitok.Add(megjelenitendoHirdetesHelyettesito);
            }

            megjelenitendoHirdetesHelyettesitok.RemoveAll(hh => !hh.EloirtIdotartamok.Any());

            #endregion

            // TODO: hirdetések

            var helyettesitoIdoablakok = megjelenitendoHirdetesHelyettesitok.SelectMany(hh => hh.EloirtIdotartamok).OrderBy(i => i.Kezdet).ToList();
            var szukitettHelyettesitoIdoablakok = IdotartamSzukites(helyettesitoIdoablakok, jarat.JaratIndulas, jarat.JaratErkezes);
            var lehetsegesHirdetesIdotartamok = SzadadIdotartamok(helyettesitoIdoablakok, jarat.JaratIndulas, jarat.JaratErkezes);

            var lehetsegesHirdetesek = await _dbContext.Hirdetesek
                .Include(h => h.Elofizetes)
                .Include(h => h.HirdetesekFolyamatban)
                .Where(h => h.MindenVonalra || h.HirdetesToVonal.Any(h => h.VonalId == jarat.VonalId))
                .Where(h => 
                    !h.IdohozKotott ||
                    (h.ErvenyessegKezdet.Value < jarat.JaratErkezes && h.ErvenyessegKezdet.Value >= jarat.JaratIndulas) ||
                    (h.ErvenyessegVeg.Value > jarat.JaratIndulas && h.ErvenyessegVeg.Value <= jarat.JaratErkezes))
                .ToListAsync();


            // await _dbContext.SaveChangesAsync();

            return hirdetesCsoport;
        }

        private List<IdotartamDto> IdotartamSzukites(List<IdotartamDto> foglaltIdotartamok, TimeSpan idoszakKezdet, TimeSpan idoszakVeg)
            => foglaltIdotartamok
                .Select(i =>
                    new IdotartamDto
                    {
                        Kezdet = i.Kezdet < idoszakKezdet ? idoszakKezdet : i.Kezdet,
                        Veg = i.Veg > idoszakVeg ? idoszakVeg : i.Veg
                    })
                .ToList();

        private List<IdotartamDto> SzadadIdotartamok(List<IdotartamDto> foglaltIdotartamok, TimeSpan idoszakKezdet, TimeSpan idoszakVeg)
        {
            var szabadIdotartamok = new List<IdotartamDto>();
            if (foglaltIdotartamok.Any())
            {
                var elsoFoglaltIdotartam = foglaltIdotartamok.First();
                szabadIdotartamok.Add(new IdotartamDto { Kezdet = idoszakKezdet, Veg = elsoFoglaltIdotartam.Kezdet });

                foreach (var foglaltIdotartam in foglaltIdotartamok.Select((Ertek, Index) => new { Ertek, Index }))
                {
                    // Ha az utolsó elemet dolgozzuk fel
                    if (foglaltIdotartam.Index == foglaltIdotartamok.Count - 1)
                    {
                        szabadIdotartamok.Add(new IdotartamDto { Kezdet = foglaltIdotartam.Ertek.Veg, Veg = idoszakVeg });
                    }
                    else
                    {
                        var kovetkezoFoglaltIdotartam = foglaltIdotartamok[foglaltIdotartam.Index + 1];
                        szabadIdotartamok.Add(new IdotartamDto { Kezdet = foglaltIdotartam.Ertek.Veg, Veg = kovetkezoFoglaltIdotartam.Kezdet });
                    }
                }

                szabadIdotartamok.RemoveAll(i => i.Kezdet == i.Veg);
            }
            else
            {
                szabadIdotartamok.Add(new IdotartamDto { Kezdet = idoszakKezdet, Veg = idoszakVeg });
            }

            return szabadIdotartamok;
        }

        private List<IdotartamDto> IdotartamKivagas(List<IdotartamDto> regiIdotartamok, IdotartamDto ujIdotartam)
        {
            var ujIdotartamok = new List<IdotartamDto>();
            foreach (var regiIdotartam in regiIdotartamok)
            {
                if (ujIdotartam.Veg < regiIdotartam.Kezdet || ujIdotartam.Kezdet > regiIdotartam.Veg)
                {
                    // Nincs átfedés
                    ujIdotartamok.Add(regiIdotartam);
                }
                else if (ujIdotartam.Kezdet >= regiIdotartam.Kezdet && ujIdotartam.Veg <= regiIdotartam.Veg)
                {
                    // Régi időtartam teljesen tartalmazza az új időtartamot
                    ujIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = regiIdotartam.Kezdet,
                        Veg = ujIdotartam.Kezdet
                    });
                    ujIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = ujIdotartam.Veg,
                        Veg = regiIdotartam.Veg,
                    });
                }
                else if (ujIdotartam.Veg > regiIdotartam.Kezdet && ujIdotartam.Veg <= regiIdotartam.Veg)
                {
                    // Részleges átfedés, a régi időtartam elejét vágjuk le
                    ujIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = ujIdotartam.Veg,
                        Veg = regiIdotartam.Veg,
                    });
                }
                else if (ujIdotartam.Kezdet >= regiIdotartam.Kezdet && ujIdotartam.Kezdet < regiIdotartam.Veg)
                {
                    // Részleges átfedés, a régi időtartam végét vágjuk le
                    ujIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = regiIdotartam.Kezdet,
                        Veg = ujIdotartam.Kezdet
                    });
                }
                else if (ujIdotartam.Kezdet <= regiIdotartam.Kezdet && ujIdotartam.Kezdet >= regiIdotartam.Veg)
                {
                    // Új időtartam teljesen tartalmazaz a régit
                }

                // Ha keletkezett üres időtartam, megszabadulunk tőle
                ujIdotartamok.RemoveAll(i => i.Kezdet == i.Veg);
            }
            return ujIdotartamok;
        }

        public async Task MegjelenitettHirdetesekKonyvelese(int jaratId, List<MegjelenitettHirdetesDto> megjelenitettHirdetesek) {
            var jarat = await _dbContext.Jaratok.FindAsync(jaratId)
                ?? throw new EntityNotFoundException($"Járat {jaratId} nem található");

            if (jarat.JaratAllapot != Common.Enum.JaratAllapot.Uton) {
                throw new BusinessException($"Járat {jaratId} még nem indult el vagy már visszaért");
            }

            jarat.JaratAllapot = Common.Enum.JaratAllapot.Visszaert;

            foreach (var megjelenitettHirdetes in megjelenitettHirdetesek) {
                var hirdetesFolyamatban = await _dbContext.HirdetesekFolyamatban.SingleOrDefaultAsync()
                    ?? throw new EntityNotFoundException($"Hirdetés {megjelenitettHirdetes.HirdetesId} nem kerülhetett megjelenítésre a járaton");

                if (hirdetesFolyamatban.Hirdetes.Elofizetes.ElofizetesTipus == Common.Enum.ElofizetesTipus.Mennyisegi) {
                    var mennyisegiElofizetes = (MennyisegiElofizetes)hirdetesFolyamatban.Hirdetes.Elofizetes;
                    mennyisegiElofizetes.ElhasznaltIdotartam += megjelenitettHirdetes.IdotartamPerc;
                } else if (hirdetesFolyamatban.Hirdetes.Elofizetes.ElofizetesTipus == Common.Enum.ElofizetesTipus.Havi) {
                    var haviElofizetes = (HaviElofizetes)hirdetesFolyamatban.Hirdetes.Elofizetes;
                    var most = new DateTime();
                    haviElofizetes.HaviElofizetesReszletek.Add(new HaviElofizetesReszletek {
                        ElhasznaltPercek = megjelenitettHirdetes.IdotartamPerc,
                        Honap = new DateTime(most.Year, most.Month, 1),
                    });
                }
            }

            _dbContext.HirdetesekFolyamatban.RemoveRange(
                _dbContext.HirdetesekFolyamatban.Where(hf => hf.Jarat == hf.Jarat)
            );

            await _dbContext.SaveChangesAsync();
        }
    }
}
