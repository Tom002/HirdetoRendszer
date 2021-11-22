using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Dal.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<Felhasznalo> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;

        public TokenService(
            UserManager<Felhasznalo> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }


        public async Task<string> CreateAccessToken(Felhasznalo user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            var secret = _configuration.GetSection("TokenOptions:Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var expirationString = _configuration.GetSection("TokenOptions:ExpirationSeconds").Value;
            var expiration = int.Parse(expirationString);
            var expires = DateTime.Now.AddSeconds(expiration);
            var notBefore = DateTime.Now;

            var token = new JwtSecurityToken(null, null, claims, notBefore, expires, creds);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
