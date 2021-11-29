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
using Microsoft.AspNetCore.Mvc;
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
            var hirdeto = await _dbContext.Users
                .SingleOrDefaultAsync(f => f.Id == felhasznaloId && f.FelhasznaloTipus == FelhasznaloTipus.Hirdeto)
                ?? throw new EntityNotFoundException($"Hirdető {felhasznaloId} nem található");

            if (!hirdeto.Engedelyezett)
                throw new ForbiddenException("Ez a fiók tiltásra került!");

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
                .Where(kep => hirdetesHozzaadas.KepIdLista.Contains(kep.KepId))
                .Where(kep => kep.FeltoltoFelhasznaloId == felhasznaloId)
                .ToListAsync();

            if (kepek.Count() != hirdetesHozzaadas.KepIdLista.Count())
                throw new ValidationException(new List<ValidationError> { new ValidationError("KepIdLista", "Nem minden kép id érvényes") });

            var hirdetes = new Hirdetes()
            {
                Elofizetes = elofizetes,
                ErvenyessegKezdet = new TimeSpan(0, hirdetesHozzaadas.ErvenyessegKezdetOra.Value, hirdetesHozzaadas.ErvenyessegKezdetPerc.Value, 0, 0),
                ErvenyessegVeg = new TimeSpan(0, hirdetesHozzaadas.ErvenyessegVegOra.Value, hirdetesHozzaadas.ErvenyessegVegPerc.Value, 0, 0),
                FelhasznaloId = felhasznaloId,
                LetrehozasDatum = DateTime.Now
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

        public async Task<PageResponse<HirdetesDto>> HirdetesekListazasa(PageRequest pageRequest)
        {
            return await _dbContext.Hirdetesek
                .ProjectTo<HirdetesDto>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(pageRequest);
        }

        public async Task HirdetesLemondas(int hirdetesId) {
            var felhasznaloId = _requestContext.FelhasznaloId;

            var hirdetes = await _dbContext.Hirdetesek
                .Include(h=>h.Elofizetes)
                .SingleOrDefaultAsync(h => h.HirdetesId == hirdetesId)
                ?? throw new EntityNotFoundException($"Hirdetés {hirdetesId} nem található");

            if (hirdetes.FelhasznaloId != felhasznaloId) {
                throw new ForbiddenException("Más felhasználó hirdetése nem mondható le");
            }

            if (hirdetes.Elofizetes.ElofizetesTipus != ElofizetesTipus.Havi) {
                throw new BusinessException("Mennyiségi előfizetésű hirdetés nem mondható le");
            }

            var haviElofizetes = (HaviElofizetes)hirdetes.Elofizetes;
            haviElofizetes.Aktiv = false;

            await _dbContext.SaveChangesAsync();
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
