using HirdetoRendszer.Bll.Dto.Common;
using HirdetoRendszer.Bll.Dto.Fedelzeti;
using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Dto.HirdetesKiosztas;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Common.Exceptions;
using HirdetoRendszer.Dal.DbContext;
using HirdetoRendszer.Dal.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<HirdetesCsoportDto> JaratInditas(int jaratId) 
        {
            // Szükséges hogy tranzakciió közben az egyszer kiolvasott adat ne változzon meg közben
            // Például lefoglalt hirdetés percek száma miatt
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);

            var jarat = await _dbContext.Jaratok.FindAsync(jaratId)
                ?? throw new EntityNotFoundException($"Járat {jaratId} nem található");

            if (jarat.JaratAllapot != JaratAllapot.IndulasElott)
                throw new BusinessException($"Járat {jaratId} már elindult korábban");

            try
            {
                jarat.JaratAllapot = JaratAllapot.Uton;

                var menetidoPerc = jarat.JaratErkezes.TotalMinutes - jarat.JaratIndulas.TotalMinutes;

                var hirdetesCsoport = new HirdetesCsoportDto()
                {
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
                        && (hh.MindenJarmure || hirdetesHelyettesitokToJarmuvek.Contains(hh.HirdetesHelyettesitoId))
                        && (!hh.IdohozKotott ||
                            (
                                (hh.ErvenyessegKezdet.Value >= jarat.JaratIndulas && hh.ErvenyessegKezdet.Value < jarat.JaratErkezes)
                                    ||
                                (hh.ErvenyessegVeg.Value > jarat.JaratIndulas && hh.ErvenyessegVeg.Value <= jarat.JaratErkezes)
                            )
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
                        var idotartamKivagasEredmeny = IdotartamKivagas(mmh.EloirtIdotartamok, megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.First());
                        mmh.EloirtIdotartamok = idotartamKivagasEredmeny.KivagasUtaniIdotartamok;
                    }

                    megjelenitendoHirdetesHelyettesitok.Add(megjelenitendoHirdetesHelyettesito);
                }

                megjelenitendoHirdetesHelyettesitok.RemoveAll(hh => !hh.EloirtIdotartamok.Any());
                hirdetesCsoport.MegjelenitendoHirdetesHelyettesitok.AddRange(megjelenitendoHirdetesHelyettesitok);

                #endregion

                var helyettesitoIdoablakok = megjelenitendoHirdetesHelyettesitok.SelectMany(hh => hh.EloirtIdotartamok).OrderBy(i => i.Kezdet).ToList();
                var szukitettHelyettesitoIdoablakok = IdotartamSzukites(helyettesitoIdoablakok, jarat.JaratIndulas, jarat.JaratErkezes);
                var lehetsegesHirdetesIdotartamok = SzadadIdotartamok(szukitettHelyettesitoIdoablakok, jarat.JaratIndulas, jarat.JaratErkezes);

                var lehetsegesHirdetesek = await _dbContext.Hirdetesek
                    .Include(h => h.Elofizetes)
                    .Include(h => h.HirdetesekFolyamatban)
                    .Include(h => h.HirdetesKepek).ThenInclude(hk => hk.Kep)
                    .Where(h => h.MindenVonalra || h.HirdetesToVonal.Any(h => h.VonalId == jarat.VonalId))
                    .Where(h =>
                        !h.IdohozKotott ||
                        (h.ErvenyessegKezdet.Value >= jarat.JaratIndulas && h.ErvenyessegKezdet.Value < jarat.JaratIndulas) ||
                        (h.ErvenyessegVeg.Value > jarat.JaratIndulas && h.ErvenyessegVeg.Value <= jarat.JaratErkezes))
                    .ToListAsync();

                var lehetsegesHirdetesekMaxHosszal = new List<LehetsegesHirdetesDto>();
                var haviElofizetesIdk = lehetsegesHirdetesek
                    .Where(l => l.Elofizetes.ElofizetesTipus == ElofizetesTipus.Havi && ((HaviElofizetes)l.Elofizetes).Aktiv)
                    .Select(l => l.Elofizetes.ElofizetesId)
                    .ToList();

                var haviElofizetesek = await _dbContext.HaviElofizetesek
                    .Include(h => h.HaviElofizetesReszletek)
                    .Where(h => haviElofizetesIdk.Contains(h.ElofizetesId) && h.Aktiv)
                    .ToListAsync();

                var mennyisegiElofizetesIdk = lehetsegesHirdetesek
                    .Where(l => l.Elofizetes.ElofizetesTipus == ElofizetesTipus.Mennyisegi && ((MennyisegiElofizetes)l.Elofizetes).ElhasznaltIdotartam < ((MennyisegiElofizetes)l.Elofizetes).VasaroltIdotartam)
                    .Select(l => l.Elofizetes.ElofizetesId)
                    .ToList();

                var mennyisegiElofizetesek = await _dbContext.MennyisegiElofizetesek
                    .Where(h => mennyisegiElofizetesIdk.Contains(h.ElofizetesId))
                    .ToListAsync();

                foreach (var lehetsegesHirdetes in lehetsegesHirdetesek)
                {
                    var folyamatbanLevoHirdetesekHossza = lehetsegesHirdetes.HirdetesekFolyamatban
                        .Select(h => h.LefoglaltPerc)
                        .DefaultIfEmpty(0)
                        .Sum();

                    if (lehetsegesHirdetes.Elofizetes.ElofizetesTipus == ElofizetesTipus.Havi)
                    {
                        var elofizetes = haviElofizetesek.SingleOrDefault(h => h.HirdetesId == lehetsegesHirdetes.HirdetesId);

                        if (elofizetes == null)
                            continue;

                        var aktualisHaviElofizetes = elofizetes.HaviElofizetesReszletek
                            .Where(h => h.Honap.Year == jarat.Datum.Year && h.Honap.Month == jarat.Datum.Month)
                            .SingleOrDefault();

                        if (aktualisHaviElofizetes == null)
                        {
                            aktualisHaviElofizetes = new HaviElofizetesReszletek
                            {
                                ElhasznaltPercek = 0,
                                Honap = new DateTime(jarat.Datum.Year, jarat.Datum.Month, 1)
                            };

                            elofizetes.HaviElofizetesReszletek.Add(aktualisHaviElofizetes);
                        }

                        var felhasznalhatoHossz = elofizetes.HaviLimit - aktualisHaviElofizetes.ElhasznaltPercek - folyamatbanLevoHirdetesekHossza;

                        lehetsegesHirdetesekMaxHosszal.Add(new LehetsegesHirdetesDto
                        {
                            HirdetesId = lehetsegesHirdetes.HirdetesId,
                            MaxIdotartam = felhasznalhatoHossz
                        });
                    }
                    else if(lehetsegesHirdetes.Elofizetes.ElofizetesTipus == ElofizetesTipus.Mennyisegi)
                    {
                        var elofizetes = mennyisegiElofizetesek.SingleOrDefault(h => h.HirdetesId == lehetsegesHirdetes.HirdetesId);

                        if (elofizetes == null)
                            continue;

                        var lh = new LehetsegesHirdetesDto
                        {
                            HirdetesId = lehetsegesHirdetes.HirdetesId,
                            MaxIdotartam = elofizetes.VasaroltIdotartam - folyamatbanLevoHirdetesekHossza - elofizetes.ElhasznaltIdotartam
                        };

                        if(lh.MaxIdotartam > 0)
                        {
                            lehetsegesHirdetesekMaxHosszal.Add(lh);
                        }
                    }
                }

                var hosszOsszeg = (double)lehetsegesHirdetesekMaxHosszal.Sum(l => l.MaxIdotartam);
                foreach (var lehetsegesHirdetesMaxHosszal in lehetsegesHirdetesekMaxHosszal)
                {
                    var hirdetes = lehetsegesHirdetesek.Single(h => h.HirdetesId == lehetsegesHirdetesMaxHosszal.HirdetesId);

                    var hirdetesIdotartam = hirdetes.IdohozKotott
                        ? new IdotartamDto { Kezdet = hirdetes.ErvenyessegKezdet.Value, Veg = hirdetes.ErvenyessegVeg.Value }
                        : new IdotartamDto { Kezdet = new TimeSpan(0, 0, 0), Veg = new TimeSpan(23, 59, 0) };

                    var idotartamKivagasEredmeny = IdotartamKivagas(lehetsegesHirdetesIdotartamok, hirdetesIdotartam);
                    if(idotartamKivagasEredmeny.KivagottIdotartamok.Any())
                    {
                        var engedelyezettIdotartamHossz = idotartamKivagasEredmeny.KivagottIdotartamok
                            .Select(i => (int)(i.Veg - i.Kezdet).TotalMinutes)
                            .Sum();

                        var megjelenitentoHirdetes = new MegjelenitendoHirdetesDto
                        {
                            HirdetesId = hirdetes.HirdetesId,
                            MaxMegjelenitesPerc = lehetsegesHirdetesMaxHosszal.MaxIdotartam,
                            KepUrlek = hirdetes.HirdetesKepek.Select(h => h.Kep.Url).ToList(),
                            EngedelyezettIdotartamok = idotartamKivagasEredmeny.KivagottIdotartamok,
                            EngedelyezettIdotartamHossz = engedelyezettIdotartamHossz
                        };

                        hirdetesCsoport.MegjelenitendoKepek.Add(megjelenitentoHirdetes);

                        _dbContext.HirdetesekFolyamatban.Add(new HirdetesFolyamatban
                        {
                            JaratId = jarat.JaratId,
                            HirdetesId = hirdetes.HirdetesId,
                            LefoglaltPerc = Math.Min(megjelenitentoHirdetes.EngedelyezettIdotartamHossz, megjelenitentoHirdetes.MaxMegjelenitesPerc)
                        });
                    }
                }

                var engedelyezettIdotartamOsszeg = hirdetesCsoport.MegjelenitendoKepek.Sum(h => h.EngedelyezettIdotartamHossz);
                if(engedelyezettIdotartamOsszeg != 0)
                {
                    foreach (var hirdetes in hirdetesCsoport.MegjelenitendoKepek)
                        hirdetes.MegjelenitesiSzazalek = (hirdetes.EngedelyezettIdotartamHossz / engedelyezettIdotartamOsszeg) * 100;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return hirdetesCsoport;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new BusinessException("Váratlan hiba történt!");
            }
        }

        private IdotartamLevagasEredmenyDto IdotartamLevagas(List<IdotartamDto> szabadIdotartamok, int idoablakPerc)
        {
            var hirdetesIdotartamok = new List<IdotartamDto>();
            while (idoablakPerc != 0 || szabadIdotartamok.Count != 0)
            {
                var kovetkeoSzabadIdotartam = szabadIdotartamok.First();
                szabadIdotartamok.Remove(kovetkeoSzabadIdotartam);
                var kovetkeoSzabadIdotartamHossz = (int)(kovetkeoSzabadIdotartam.Veg - kovetkeoSzabadIdotartam.Kezdet).TotalMinutes;

                if (idoablakPerc >= kovetkeoSzabadIdotartamHossz)
                {
                    hirdetesIdotartamok.Add(kovetkeoSzabadIdotartam);
                    idoablakPerc -= kovetkeoSzabadIdotartamHossz;
                }
                else
                {
                    var levagottIdotartam = new IdotartamDto
                    {
                        Kezdet = kovetkeoSzabadIdotartam.Kezdet,
                        Veg = kovetkeoSzabadIdotartam.Kezdet.Add(new TimeSpan(hours: 0, minutes: idoablakPerc, seconds: 0))
                    };

                    szabadIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = levagottIdotartam.Veg,
                        Veg = kovetkeoSzabadIdotartam.Veg
                    });

                    hirdetesIdotartamok.Add(levagottIdotartam);
                    idoablakPerc = 0;
                }
            }

            return new IdotartamLevagasEredmenyDto
            {
                LevagottIdotartamokHirdetesnek = hirdetesIdotartamok,
                MaradekSzabadIdotartamok = szabadIdotartamok
            };
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

        private IdotartamKivagasEredmenyDto IdotartamKivagas(List<IdotartamDto> regiIdotartamok, IdotartamDto ujIdotartam)
        {
            var ujIdotartamok = new List<IdotartamDto>();
            var kivagottIdotartamok = new List<IdotartamDto>();

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

                    kivagottIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = ujIdotartam.Kezdet,
                        Veg = ujIdotartam.Veg,
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
                    kivagottIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = regiIdotartam.Kezdet,
                        Veg = ujIdotartam.Veg,
                    });

                    ujIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = ujIdotartam.Veg,
                        Veg = regiIdotartam.Veg,
                    });
                }
                else if (ujIdotartam.Kezdet >= regiIdotartam.Kezdet && ujIdotartam.Kezdet < regiIdotartam.Veg)
                {
                    // Részleges átfedés, a régi időtartam végét vágjuk le

                    kivagottIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = ujIdotartam.Kezdet,
                        Veg = regiIdotartam.Veg,
                    });

                    ujIdotartamok.Add(new IdotartamDto
                    {
                        Kezdet = regiIdotartam.Kezdet,
                        Veg = ujIdotartam.Kezdet
                    });
                }

                // Ha keletkezett üres időtartam, megszabadulunk tőle
                ujIdotartamok.RemoveAll(i => i.Kezdet == i.Veg);
                kivagottIdotartamok.RemoveAll(i => i.Kezdet == i.Veg);
            }

            return new IdotartamKivagasEredmenyDto
            {
                KivagottIdotartamok = kivagottIdotartamok,
                KivagasUtaniIdotartamok = ujIdotartamok
            };
        }

        public async Task MegjelenitettHirdetesekKonyvelese(int jaratId, List<MegjelenitettHirdetesDto> megjelenitettHirdetesek) {
            var jarat = await _dbContext.Jaratok.FindAsync(jaratId)
                ?? throw new EntityNotFoundException($"Járat {jaratId} nem található");

            if (jarat.JaratAllapot != JaratAllapot.Uton) {
                throw new BusinessException($"Járat {jaratId} még nem indult el vagy már visszaért");
            }

            jarat.JaratAllapot = JaratAllapot.Visszaert;

            var hirdetesekFolyamatban = await _dbContext.HirdetesekFolyamatban
                .Include(h => h.Hirdetes).ThenInclude(h => h.Elofizetes)
                .Where(hf => hf.JaratId == jarat.JaratId)
                .ToListAsync();

            var haviElofizetesek = await _dbContext.HaviElofizetesek
                .Include(h => h.HaviElofizetesReszletek)
                .Where(h => hirdetesekFolyamatban.Select(hf => hf.HirdetesId).Contains(h.HirdetesId))
                .ToListAsync();

            var mennyisegiElofizetesek = await _dbContext.MennyisegiElofizetesek
                .Where(h => hirdetesekFolyamatban.Select(hf => hf.HirdetesId).Contains(h.HirdetesId))
                .ToListAsync();

            foreach (var megjelenitettHirdetes in megjelenitettHirdetesek) {

                var hirdetesFolyamatban = hirdetesekFolyamatban.SingleOrDefault()
                    ?? throw new EntityNotFoundException($"Hirdetés {megjelenitettHirdetes.HirdetesId} nem kerülhetett megjelenítésre a járaton");

                if (hirdetesFolyamatban.Hirdetes.Elofizetes.ElofizetesTipus == ElofizetesTipus.Mennyisegi) 
                {
                    var mennyisegiElofizetes = mennyisegiElofizetesek.Single(me => me.HirdetesId == hirdetesFolyamatban.HirdetesId);
                    mennyisegiElofizetes.ElhasznaltIdotartam += megjelenitettHirdetes.IdotartamPerc;
                } 
                else if (hirdetesFolyamatban.Hirdetes.Elofizetes.ElofizetesTipus == ElofizetesTipus.Havi) 
                {
                    var haviElofizetes = haviElofizetesek.Single(me => me.HirdetesId == hirdetesFolyamatban.HirdetesId);
                    var jaratDatum = jarat.Datum;

                    var haviElofizetesReszletekToUpdate = haviElofizetes.HaviElofizetesReszletek
                        .Single(her => her.Honap.Date.Year == jaratDatum.Year && her.Honap.Date.Month == jaratDatum.Month);

                    haviElofizetesReszletekToUpdate.ElhasznaltPercek += megjelenitettHirdetes.IdotartamPerc;
                }
            }

            _dbContext.HirdetesekFolyamatban.RemoveRange(hirdetesekFolyamatban);

            await _dbContext.SaveChangesAsync();
        }
    }
}
