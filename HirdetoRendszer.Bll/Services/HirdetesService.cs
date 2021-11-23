using AutoMapper;
using AutoMapper.QueryableExtensions;
using HirdetoRendszer.Bll.Dto.Hirdetes;
using HirdetoRendszer.Bll.Extensions;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Bll.Pagination;
using HirdetoRendszer.Common.Enum;
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
    public class HirdetesService : IHirdetesService
    {
        private readonly HirdetoRendszerDbContext _dbContext;
        private readonly IRequestContext _requestContext;
        private readonly IMapper _mapper;

        public HirdetesService(HirdetoRendszerDbContext dbContext, IRequestContext requestContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _requestContext = requestContext;
            _mapper = mapper;
        }

        public async Task<HirdetesDto> HirdetesFeladas(HirdetesHozzaadasDto hirdetesHozzaadas)
        {
            var felhasznaloId = _requestContext.FelhasznaloId;

            Elofizetes elofizetes;
            if (hirdetesHozzaadas.ElofizetesTipus == ElofizetesTipus.Havi)
            {
                elofizetes = new HaviElofizetes
                {
                    Aktiv = true,
                    HaviLimit = hirdetesHozzaadas.HaviLimit.Value,
                };
            }
            else
            {
                elofizetes = new MennyisegiElofizetes
                {
                    ElhasznaltIdotartam = 0,
                    VasaroltIdotartam = hirdetesHozzaadas.VasaroltIdotartam.Value
                };
            }

            var vonalak = await _dbContext.Vonalak
                .Where(vonal => hirdetesHozzaadas.VonalIdLista.Contains(vonal.VonalId))
                .ToListAsync();

            if (vonalak.Count() != hirdetesHozzaadas.VonalIdLista.Count())
                throw new ValidationException(new List<ValidationError> { new ValidationError("VonalIdLista", "Nem minden vonal id érvényes") });

            var kepek = await _dbContext.Kepek
                .Where(kep => hirdetesHozzaadas.HirdetesKepIdLista.Contains(kep.KepId))
                .Where(kep => kep.FeltoltoFelhasznaloId == felhasznaloId)
                .ToListAsync();

            if (kepek.Count() != hirdetesHozzaadas.HirdetesKepIdLista.Count())
                throw new ValidationException(new List<ValidationError> { new ValidationError("HirdetesKepIdLista", "Nem minden kép id érvényes") });

            var hirdetes = new Hirdetes()
            {
                Elofizetes = elofizetes,
                ErvenyessegKezdet = new TimeSpan(0, hirdetesHozzaadas.ErvenyessegKezdetOra.Value, hirdetesHozzaadas.ErvenyessegKezdetPerc.Value, 0, 0),
                ErvenyessegVeg = new TimeSpan(0, hirdetesHozzaadas.ErvenyessegVegOra.Value, hirdetesHozzaadas.ErvenyessegVegPerc.Value, 0, 0),
                FelhasznaloId = felhasznaloId,
            };

            foreach (var kep in kepek)
            {
                hirdetes.HirdetesKepek.Add(new KepToHirdetes
                {
                    Kep = kep
                });
            }

            foreach (var vonal in vonalak)
            {
                hirdetes.HirdetesToVonal.Add(new HirdetesToVonal
                {
                    Vonal = vonal
                });
            }

            _dbContext.Hirdetesek.Add(hirdetes);
            await _dbContext.SaveChangesAsync();

            return await _dbContext.Hirdetesek
                .ProjectTo<HirdetesDto>(_mapper.ConfigurationProvider)
                .SingleAsync(h => h.HirdetesId == hirdetes.HirdetesId);
        }

        public async Task<PageResponse<HirdetesDto>> HirdetesListazas(PageRequest pageRequest)
        {
            return await _dbContext.Hirdetesek
                .ProjectTo<HirdetesDto>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(pageRequest);
        }

        public async Task HirdetesTorles(int hirdetesId)
        {
            var hirdetes = await _dbContext.Hirdetesek.SingleOrDefaultAsync(h => h.HirdetesId == hirdetesId)
                ?? throw new EntityNotFoundException($"Hirdetes {hirdetesId} nem található");

            hirdetes.SoftDeleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
