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

            if (jarat.JaratAllapot != Common.Enum.JaratAllapot.IndulasElott) {
                throw new BusinessException($"Járat {jaratId} már elindult korábban");
            }

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
                .Where(hh => hh.Aktiv
                    && (
                        hh.MindenJarmure || hirdetesHelyettesitokToJarmuvek.Contains(hh.HirdetesHelyettesitoId)
                    )
                )
                .OrderBy(hh => hh.CreatedAt)
                .ToListAsync();

            var megjelenitendoHirdetesHelyettesitok = new List<MegjelenitendoHirdetesHelyettesitoDto>();

            foreach (var hirdetesHelyettesito in hirdetesHelyettesitok) {
                var megjelenitendoHirdetesHelyettesito = new MegjelenitendoHirdetesHelyettesitoDto() {
                    HirdetesHelyettesitoId = hirdetesHelyettesito.HirdetesHelyettesitoId,
                    KepUrlek = new List<string>(hirdetesHelyettesito.HirdetesHelyettesitoKepek.Select(hhk => hhk.Kep.Url)),
                    EloirtIdotartamok = new List<IdotartamDto>(),
                };

                if (hirdetesHelyettesito.IdohozKotott) {
                    megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.Add(new IdotartamDto() {
                        Kezdet = hirdetesHelyettesito.ErvenyessegKezdet.Value,
                        Veg = hirdetesHelyettesito.ErvenyessegVeg.Value,
                    });
                } else {
                    megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.Add(new IdotartamDto() {
                        Kezdet = new TimeSpan(0, 0, 0),
                        Veg = new TimeSpan(23, 59, 0),
                    });
                }

                foreach (var mmh in megjelenitendoHirdetesHelyettesitok) {
                    mmh.EloirtIdotartamok = IdotartamKivagas(mmh.EloirtIdotartamok, megjelenitendoHirdetesHelyettesito.EloirtIdotartamok.First());
                }

                megjelenitendoHirdetesHelyettesitok.Add(megjelenitendoHirdetesHelyettesito);
            }
            #endregion

            // TODO: hirdetések

            await _dbContext.SaveChangesAsync();

            return hirdetesCsoport;
        }

        private List<IdotartamDto> IdotartamKivagas(List<IdotartamDto> regiIdotartamok, IdotartamDto ujIdotartam) {
            var minute = new TimeSpan(0, 1, 0);
            var ujIdotartamok = new List<IdotartamDto>();
            foreach (var regiIdotartam in regiIdotartamok) {
                if (regiIdotartam.Kezdet < ujIdotartam.Veg || regiIdotartam.Veg < ujIdotartam.Kezdet ) {
                    // Nincs átfedés
                    ujIdotartamok.Add(regiIdotartam);
                } else if (regiIdotartam.Kezdet <= ujIdotartam.Kezdet && regiIdotartam.Veg <= ujIdotartam.Veg) {
                    // Régi időtartam teljesen tartalmazza az új időtartamot
                    ujIdotartamok.Add(new IdotartamDto {
                        Kezdet = regiIdotartam.Kezdet,
                        Veg = ujIdotartam.Kezdet.Subtract(minute),
                    });
                    ujIdotartamok.Add(new IdotartamDto {
                        Kezdet = ujIdotartam.Veg.Add(minute),
                        Veg = regiIdotartam.Veg,
                    });
                } else if (regiIdotartam.Kezdet <= ujIdotartam.Veg) {
                    // Részleges átfedés, a régi időtartam elejét vágjuk le
                    ujIdotartamok.Add(new IdotartamDto {
                        Kezdet = ujIdotartam.Veg.Add(minute),
                        Veg = regiIdotartam.Veg,
                    });
                } else if (regiIdotartam.Veg <= ujIdotartam.Kezdet) {
                    // Részleges átfedés, a régi időtartam végét vágjuk le
                    ujIdotartamok.Add(new IdotartamDto {
                        Kezdet = regiIdotartam.Kezdet,
                        Veg = ujIdotartam.Kezdet.Subtract(minute),
                    });
                }
                // Ha keletkezett üres időtartam, megszabadulunk tőle
                for (int i = ujIdotartamok.Count - 1; i >= 0; i--) {
                    if (ujIdotartamok[i].Kezdet == ujIdotartam.Veg) {
                        ujIdotartamok.RemoveAt(i);
                    }
                }
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
