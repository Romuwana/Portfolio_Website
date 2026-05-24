using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Models;

namespace PortfolioAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Certification> Certifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 2. TELL POSTGRESQL TO STORE THESE AS ARRAYS
            modelBuilder.Entity<Project>()
                .Property(p => p.TechStack)
                .HasColumnType("text[]");

            modelBuilder.Entity<Project>()
                .Property(p => p.ImageUrls)
                .HasColumnType("text[]");
        }
    }
}