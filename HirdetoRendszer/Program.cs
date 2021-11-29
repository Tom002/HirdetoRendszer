using HirdetoRendszer.Bll.Interfaces;
using HirdetoRendszer.Dal.DbContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HirdetoRendszer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
               .Build();

            Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

            try
            {
                Log.Information("Starting web host");

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<HirdetoRendszerDbContext>();
                    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();

                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();

                    await seedService.SeedSzerepkorok();
                    await seedService.SeedFelhasznalok();
                    await seedService.SeedAllomasokAndVonalak();
                    await seedService.SeedJarmuvek();
                    await seedService.SeedHirdetesKepek();
                    await seedService.SeedHirdetesek();
                    await seedService.SeedHirdetesHelyettesitok();
                    await seedService.SeedJaratok();
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
