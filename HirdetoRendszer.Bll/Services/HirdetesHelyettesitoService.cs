using AutoMapper;
using AutoMapper.QueryableExtensions;
using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Extensions;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Bll.Pagination;
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
    public class HirdetesHelyettesitoService : IHirdetesHelyettesitoService
    {
        private readonly HirdetoRendszerDbContext _dbContext;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        public HirdetesHelyettesitoService(
            HirdetoRendszerDbContext dbContext,
            IRequestContext requestContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _requestContext = requestContext;
            _mapper = mapper;
        }

        public async Task<PageResponse<HirdetesHelyettesitoDto>> HirdetesHelyettesitokListazasa(PageRequest pageRequest) {
            return await _dbContext.HirdetesHelyettesitok
                .ProjectTo<HirdetesHelyettesitoDto>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(pageRequest);
        }

        public async Task<HirdetesHelyettesitoDto> HirdetesHelyettesitoLetrehozas(HirdetesHelyettesitoHozzaadasDto hirdetesHelyettesitoHozzaadas) {
            var felhasznaloId = _requestContext.FelhasznaloId;

            var hirdetesHelyettesito = new HirdetesHelyettesito() {
                Aktiv = true,
                MindenJarmure = hirdetesHelyettesitoHozzaadas.MindenJarmure,
                IdohozKotott = hirdetesHelyettesitoHozzaadas.MindenJarmure,
            };

            _dbContext.HirdetesHelyettesitok.Add(hirdetesHelyettesito);

            if (!hirdetesHelyettesitoHozzaadas.MindenJarmure) {
                var jarmuvek = await _dbContext.Jarmuvek
                    .Where(j => hirdetesHelyettesitoHozzaadas.JarmuIdLista
                    .Contains(j.JarmuId))
                    .ToListAsync();

                if (jarmuvek.Count() != hirdetesHelyettesitoHozzaadas.JarmuIdLista.Count())
                    throw new ValidationException(new List<ValidationError> {
                        new ValidationError(nameof(hirdetesHelyettesitoHozzaadas.JarmuIdLista), "Nem minden jármű id érvényes")
                    });

                foreach (var jarmu in jarmuvek) {
                    hirdetesHelyettesito.HirdetesHelyettesitokToJarmuvek.Add(new HirdetesHelyettesitoToJarmu() {
                        Jarmu = jarmu,
                    });
                }
            }

            var kepek = await _dbContext.Kepek
                .Where(kep => hirdetesHelyettesitoHozzaadas.KepIdLista.Contains(kep.KepId))
                .Where(kep => kep.FeltoltoFelhasznaloId == felhasznaloId)
                .ToListAsync();

            if (kepek.Count() != hirdetesHelyettesitoHozzaadas.KepIdLista.Count())
                throw new ValidationException(new List<ValidationError> {
                    new ValidationError(nameof(hirdetesHelyettesitoHozzaadas.KepIdLista), "Nem minden kép id érvényes")
                });

            foreach (var kep in kepek) {
                hirdetesHelyettesito.HirdetesHelyettesitoKepek.Add(new KepToHirdetesHelyettesito() {
                    Kep = kep,
                });
            }

            if (hirdetesHelyettesitoHozzaadas.IdohozKotott) {
                if (hirdetesHelyettesitoHozzaadas.ErvenyessegKezdetOra.HasValue
                    && hirdetesHelyettesitoHozzaadas.ErvenyessegKezdetPerc.HasValue
                    && hirdetesHelyettesitoHozzaadas.ErvenyessegVegOra.HasValue
                    && hirdetesHelyettesitoHozzaadas.ErvenyessegVegPerc.HasValue) {
                    // TODO: validáció hogy a vége később van-e mint az eleje és hogy 00:00 és 23:59 közé esnek-e
                    hirdetesHelyettesito.ErvenyessegKezdet = new TimeSpan(
                        hirdetesHelyettesitoHozzaadas.ErvenyessegKezdetOra.Value,
                        hirdetesHelyettesitoHozzaadas.ErvenyessegKezdetPerc.Value,
                        0
                    );
                    hirdetesHelyettesito.ErvenyessegVeg = new TimeSpan(
                        hirdetesHelyettesitoHozzaadas.ErvenyessegVegOra.Value,
                        hirdetesHelyettesitoHozzaadas.ErvenyessegVegPerc.Value,
                        0
                    );
                } else {
                    throw new ValidationException(new List<ValidationError> {
                        new ValidationError(nameof(hirdetesHelyettesitoHozzaadas.IdohozKotott), "Nincs minden érvényesség mező kitöltve")
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<HirdetesHelyettesitoDto>(hirdetesHelyettesito);
        }

        public async Task HirdetesHelyettesitoTorles(int hirdetesHelyettesitoId) {
            var hirdetesHelyettesito = await _dbContext.HirdetesHelyettesitok
                .FindAsync(hirdetesHelyettesitoId)
                ?? throw new EntityNotFoundException($"Hirdetés helyettesítő {hirdetesHelyettesitoId} nem található");

            hirdetesHelyettesito.SoftDeleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
