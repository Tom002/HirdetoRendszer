using HirdetoRendszer.Common.Enum;
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

        public DbSet<KepToHirdetes> KepToHirdetes { get; set; }

        public DbSet<KepToHirdetesHelyettesito> KepToHirdetesHelyettesito { get; set; }

        public DbSet<HirdetesHelyettesitoToJarmu> HirdetesHelyettesitoToJarmu { get; set; }

        public DbSet<Jarat> Jaratok { get; set; }

        public DbSet<Jarmu> Jarmuvek { get; set; }

        public DbSet<Vonal> Vonalak { get; set; }

        public DbSet<AllomasToVonal> AllomasToVonal { get; set; }

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

            modelBuilder.Entity<Elofizetes>()
                .HasDiscriminator(e => e.ElofizetesTipus)
                .HasValue<HaviElofizetes>(ElofizetesTipus.Havi)
                .HasValue<MennyisegiElofizetes>(ElofizetesTipus.Mennyisegi);

            modelBuilder.Entity<KepToHirdetes>()
                .HasKey(kh => new { kh.HirdetesId, kh.KepId });

            modelBuilder.Entity<KepToHirdetes>()
                .HasOne(kh => kh.Kep)
                .WithMany(k => k.KepToHirdetes)
                .HasForeignKey(kh => kh.KepId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KepToHirdetes>()
                .HasOne(kh => kh.Hirdetes)
                .WithMany(k => k.HirdetesKepek)
                .HasForeignKey(kh => kh.HirdetesId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KepToHirdetesHelyettesito>()
                .HasKey(kh => new { kh.HirdetesHelyettesitoId, kh.KepId });

            modelBuilder.Entity<KepToHirdetesHelyettesito>()
                .HasOne(kh => kh.Kep)
                .WithMany(k => k.KepToHirdetesHelyettesito)
                .HasForeignKey(kh => kh.KepId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KepToHirdetesHelyettesito>()
                .HasOne(kh => kh.HirdetesHelyettesito)
                .WithMany(k => k.HirdetesHelyettesitoKepek)
                .HasForeignKey(kh => kh.HirdetesHelyettesitoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AllomasToVonal>()
                .HasKey(av => new { av.AllomasId, av.VonalId });

            modelBuilder.Entity<AllomasToVonal>()
                .HasOne(kh => kh.Allomas)
                .WithMany(k => k.AllomasToVonal)
                .HasForeignKey(kh => kh.AllomasId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AllomasToVonal>()
                .HasOne(kh => kh.Vonal)
                .WithMany(k => k.AllomasToVonal)
                .HasForeignKey(kh => kh.VonalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HirdetesToVonal>()
                .HasKey(hv => new { hv.HirdetesId, hv.VonalId });

            modelBuilder.Entity<HirdetesToVonal>()
                .HasOne(hv => hv.Hirdetes)
                .WithMany(h => h.HirdetesToVonal)
                .HasForeignKey(hv => hv.HirdetesId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HirdetesToVonal>()
                .HasOne(hv => hv.Vonal)
                .WithMany(v => v.HirdetesToVonal)
                .HasForeignKey(hv => hv.VonalId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
