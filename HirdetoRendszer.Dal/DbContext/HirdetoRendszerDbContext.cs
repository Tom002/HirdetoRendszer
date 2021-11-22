using HirdetoRendszer.Dal.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HirdetoRendszer.Dal.DbContext
{
    public class HirdetoRendszerDbContext : IdentityDbContext<Felhasznalo, IdentityRole<int>, int>
    {
        public DbSet<Allomas> Allomasok { get; set; }

        public DbSet<Elofizetes> Elofizetesek { get; set; }

        public DbSet<HaviElofizetes> HaviElofizetesek { get; set; }

        public DbSet<MennyisegiElofizetes> MennyisegiElofizetesek { get; set; }

        public DbSet<HaviElofizetesReszletek> HaviElofizetesReszletek { get; set; }

        public DbSet<Hirdetes> Hirdetesek { get; set; }

        public DbSet<HirdetesFolyamatban> HirdetesekFolyamatban { get; set; }

        public DbSet<HirdetesHelyettesito> HirdetesHelyettesitok { get; set; }

        public DbSet<Kep> Kepek { get; set; }

        public DbSet<HirdetesKep> HirdetesKepek { get; set; }

        public DbSet<HirdetesHelyettesitoKep> HirdetesHelyettesitoKepek { get; set; }

        public DbSet<HirdetesHelyettesitoToJarmu> HirdetesHelyettesitoToJarmu { get; set; }

        public DbSet<Jarat> Jaratok { get; set; }

        public DbSet<Jarmu> Jarmuvek { get; set; }

        public DbSet<Vonal> Vonalak { get; set; }

        public HirdetoRendszerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HirdetesHelyettesitoToJarmu>()
                .HasKey(h => new { h.HirdetesHelyettesitoId, h.JarmuId });

            modelBuilder.Entity<HirdetesHelyettesitoToJarmu>()
                .HasOne(hj => hj.HirdetesHelyettesito)
                .WithMany(h => h.HirdetesHelyettesitokToJarmuvek)
                .HasForeignKey(hj => hj.HirdetesHelyettesitoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HirdetesHelyettesitoToJarmu>()
                .HasOne(hj => hj.Jarmu)
                .WithMany(j => j.HirdetesHelyettesitokToJarmuvek)
                .HasForeignKey(hj => hj.JarmuId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HirdetesFolyamatban>()
                .HasKey(h => new { h.HirdetesId, h.JaratId });

            modelBuilder.Entity<HirdetesFolyamatban>()
                .HasOne(hf => hf.Jarat)
                .WithMany(j => j.HirdetesekFolyamatban)
                .HasForeignKey(hf => hf.JaratId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HirdetesFolyamatban>()
                .HasOne(hf => hf.Hirdetes)
                .WithMany(j => j.HirdetesekFolyamatban)
                .HasForeignKey(hf => hf.HirdetesId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
