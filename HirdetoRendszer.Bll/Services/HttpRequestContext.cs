using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Common.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace HirdetoRendszer.Bll.Services
{
    public class HttpRequestContext : IRequestContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public int FelhasznaloId { get; private set; }

        public string FelhasznaloEmail { get; private set; }

        public FelhasznaloTipus FelhasznaloTipus { get; set; }

        public HttpRequestContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                FelhasznaloId = int.Parse(_httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                FelhasznaloEmail = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
                var roleClaim = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role);
                FelhasznaloTipus = Enum.Parse<FelhasznaloTipus>(roleClaim.Value);
            }
        }
    }
}
