using Microsoft.EntityFrameworkCore;
using QRGenerateTask.Models;

namespace QRGenerateTask.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<VCard> VCards { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VCard>(model =>
            {
                model.HasKey(prop => prop.Id);
                model.Property(prop => prop.Firstname).HasMaxLength(30).IsRequired(true);
                model.Property(prop => prop.Lastname).HasMaxLength(30).IsRequired(true);
                model.Property(prop => prop.Email).IsRequired(true);
                model.HasIndex(prop => prop.Email).IsUnique(true);
                model.Property(prop => prop.Phone).IsRequired(true);
                model.Property(prop => prop.Country).HasMaxLength(50).IsRequired(true);
                model.Property(prop => prop.City).HasMaxLength(50).IsRequired(true);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
