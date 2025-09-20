using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data
{
    public class AppDbContext : IdentityDbContext<User, UserRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Dishes> Dishes { get; set; }
        public DbSet<DishIngredients> DishIngredients { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Составной ключ для связующей таблицы
            builder.Entity<DishIngredients>()
                .HasKey(di => new { di.DishID, di.IngredientID });

            builder.Entity<DishIngredients>()
                .HasOne(di => di.Dishes)
                .WithMany(d => d.DishIngredients)
                .HasForeignKey(di => di.DishID);

            builder.Entity<DishIngredients>()
                .HasOne(di => di.Ingredients)
                .WithMany(i => i.DishIngredients)
                .HasForeignKey(di => di.IngredientID);

                        builder.Entity<User>()
            .HasOne(u => u.UserRole)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.IdRole);
        }
    }
}
