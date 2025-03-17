using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace MyRazorApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<Dishes> Dishes { get; set; }

        public DbSet<DishIngredients> DishIngredients { get; set; }

        public DbSet<Ingredients> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Определяем составной ключ для таблицы DishIngredient
            modelBuilder.Entity<DishIngredients>()
                .HasKey(di => new { di.DishID, di.IngredientID });

            // Настраиваем связи
            modelBuilder.Entity<DishIngredients>()
                .HasOne(di => di.Dishes)
                .WithMany(d => d.DishIngredients)
                .HasForeignKey(di => di.DishID);

            modelBuilder.Entity<DishIngredients>()
                .HasOne(di => di.Ingredients)
                .WithMany(i => i.DishIngredients)
                .HasForeignKey(di => di.IngredientID);
        }
    }
}
