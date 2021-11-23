using HirdetoRendszer.Bll.Dto.Hirdeto;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Common.Exceptions;
using HirdetoRendszer.Dal.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class HirdetoService : IHirdetoService
    {
        private readonly HirdetoRendszerDbContext _dbContext;

        public HirdetoService(HirdetoRendszerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HirdetoEngedelyezes(int felhasznaloId, HirdetoEngedelyezesDto hirdetoEngedelyezesDto)
        {
            var hirdeto = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == felhasznaloId && u.FelhasznaloTipus == FelhasznaloTipus.Hirdeto)
                    ?? throw new EntityNotFoundException($"Felhasználó {felhasznaloId} nem található");

            hirdeto.Engedelyezett = hirdetoEngedelyezesDto.Engedelyezett;
            await _dbContext.SaveChangesAsync();
        }
    }
}
