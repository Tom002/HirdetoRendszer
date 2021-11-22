using HirdetoRendszer.Bll.Dto.Auth;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Interfaces
{
    public interface IAuthService
    {
        public Task<BejelentkezesValaszDto> Bejelentkezes(BejelentkezesDto bejelentkezesDto);

        public Task<BejelentkezesValaszDto> HirdetoRegisztracio(HirdetoRegisztracioDto registrationDto);
    }
}
