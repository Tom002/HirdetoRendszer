using HirdetoRendszer.Bll.Dto.Auth;
using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using HirdetoRendszer.Common.Exceptions;
using HirdetoRendszer.Dal.DbContext;
using HirdetoRendszer.Dal.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<Felhasznalo> _userManager;
        private readonly SignInManager<Felhasznalo> _signInManager;
        private readonly HirdetoRendszerDbContext _dbContext;

        public AuthService(
            ITokenService tokenService,
            UserManager<Felhasznalo> userManager,
            SignInManager<Felhasznalo> signInManager,
            HirdetoRendszerDbContext dbContext)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        public async Task<BejelentkezesValaszDto> Bejelentkezes(BejelentkezesDto bejelentkezesDto)
        {
            var user = await GetFelhasznaloByEmail(bejelentkezesDto.Email)
                ?? throw new BusinessException("Invalid credentials");

            await CheckSignInCredentials(user, bejelentkezesDto.Jelszo);
            return await Bejelentkezes(user);
        }

        public async Task<BejelentkezesValaszDto> HirdetoRegisztracio(HirdetoRegisztracioDto registrationDto)
        {
            var user = new Felhasznalo
            {
                Email = registrationDto.Email,
                UserName = registrationDto.Email,
                KeresztNev = registrationDto.KeresztNev,
                VezetekNev = registrationDto.VezetekNev,
                EmailConfirmed = true,
                CegNev = registrationDto.CegNev,
                CegCim = registrationDto.CegCim,
                FelhasznaloTipus = FelhasznaloTipus.Hirdeto
            };

            await _userManager.CreateAsync(user, registrationDto.Jelszo);
            await _userManager.AddToRoleAsync(user, FelhasznaloTipus.Hirdeto.ToString());
            await _dbContext.SaveChangesAsync();

            return await Bejelentkezes(user);
        }

        private async Task<BejelentkezesValaszDto> Bejelentkezes(Felhasznalo felhasznalo)
        {
            var accessToken = await _tokenService.CreateAccessToken(felhasznalo);
            if (accessToken is string)
            {
                return new BejelentkezesValaszDto
                {
                    UserId = felhasznalo.Id,
                    Token = accessToken
                };
            }
            throw new BusinessException("Unexpected error while creating token");
        }

        public async Task<Felhasznalo> GetFelhasznaloByEmail(string email)
            => await _dbContext.Users.FirstOrDefaultAsync(a => a.Email == email);

        private async Task CheckSignInCredentials(Felhasznalo user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if(!result.Succeeded)
                throw new BusinessException("Invalid credentials");
        }
    }
}
