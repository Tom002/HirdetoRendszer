using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Dal.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace HirdetoRendszer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HirdetoRendszerDbContext>();
                var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();

                //await seedService.SeedRoles();
                //await seedService.SeedUsers();
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
