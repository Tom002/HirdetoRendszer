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
using System.Linq;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class HirdetesHelyettesitoService : IHirdetesHelyettesitoService
    {
        private readonly HirdetoRendszerDbContext _dbContext;
        private readonly IMapper _mapper;

        public HirdetesHelyettesitoService(HirdetoRendszerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PageResponse<HirdetesHelyettesitoDto>> HirdetesHelyettesitokListazasa(PageRequest pageRequest) {
            return await _dbContext.HirdetesHelyettesitok
                .ProjectTo<HirdetesHelyettesitoDto>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(pageRequest);
        }

        public async Task<HirdetesHelyettesitoDto> HirdetesHelyettesitoLetrehozas(HirdetesHelyettesitoHozzaadasDto hirdetesHelyettesitoHozzaadas) {
            var hirdetesHelyettesito = new HirdetesHelyettesito() {
                Aktiv = true,
            };
            _dbContext.HirdetesHelyettesitok.Add(hirdetesHelyettesito);

            var jarmuvek = await _dbContext.Jarmuvek
                .Where(j => hirdetesHelyettesitoHozzaadas.JarmuIdLista
                .Contains(j.JarmuId))
                .ToListAsync();
            foreach (var jarmu in jarmuvek) {
                var hirdetesHelyettesitoToJarmu = new HirdetesHelyettesitoToJarmu() {
                    HirdetesHelyettesito = hirdetesHelyettesito,
                    Jarmu = jarmu,
                };
            }

            if (hirdetesHelyettesitoHozzaadas.ErvenyessegKezdetOra.HasValue
                && hirdetesHelyettesitoHozzaadas.ErvenyessegKezdetPerc.HasValue
                && hirdetesHelyettesitoHozzaadas.ErvenyessegVegOra.HasValue
                && hirdetesHelyettesitoHozzaadas.ErvenyessegVegPerc.HasValue) {
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
