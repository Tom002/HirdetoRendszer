using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Bll.Services;
using HirdetoRendszer.Dal.DbContext;
using HirdetoRendszer.Dal.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HirdetoRendszer.Bll.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Development");
            services.AddDbContext<HirdetoRendszerDbContext>(opt => opt.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddBllServices(this IServiceCollection services, IConfiguration configuration)
        {
            // TODO
            // BLL services should be registered here

            //services.AddScoped<IAuthService, AuthService>();
            //services.AddScoped<ITokenService, TokenService>();

            services.AddTransient<ISeedService, SeedService>();

            return services;
        }

        public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddIdentity<Felhasznalo, IdentityRole<int>>(options =>
            {
                // Production requirements
                options.Password = new PasswordOptions()
                {
                    RequiredLength = 8,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true
                };

                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<HirdetoRendszerDbContext>();

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secret = configuration.GetSection("TokenOptions:Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateAudience = false,
                        ValidateIssuer = false
                    };
                });

            return services;
        }

        
    }
}
