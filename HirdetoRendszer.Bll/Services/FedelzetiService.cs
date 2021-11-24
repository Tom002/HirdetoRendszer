using HirdetoRendszer.Bll.Dto.Hirdeto;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Common.Exceptions;
using HirdetoRendszer.Dal.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class FedelzetiService : IFedelzetiService {
        private readonly HirdetoRendszerDbContext _dbContext;

        public FedelzetiService(HirdetoRendszerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }
}
