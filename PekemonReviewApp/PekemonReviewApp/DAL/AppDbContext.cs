using Microsoft.EntityFrameworkCore;
using PekemonReviewApp.Models;
using PokemonReviewApp.Models;

namespace PekemonReviewApp.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Pokemon> Pokemons { get; set; }
        public DbSet<PokemonCategory> PokemonCategories { get; set; }
        public DbSet<PokemonOwner> PokemonOwners { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PokemonCategory>()
                .HasKey(pc => new { pc.PokemonId, pc.CategoryId });
            modelBuilder.Entity<PokemonCategory>()
                .HasOne(pc => pc.Pokemon)
                .WithMany(pc => pc.PokemonCategory)
                .HasForeignKey(pc => pc.PokemonId);
            modelBuilder.Entity<PokemonCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(pc => pc.PokemonCategory)
                .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<PokemonOwner>()
                .HasKey(po=>new {po.OwnerId,po.PokemonId});
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(po => po.Owner)
                .WithMany(po => po.PokemonOwner)
                .HasForeignKey(po => po.OwnerId);
            modelBuilder.Entity<PokemonOwner>()
                .HasOne(po => po.Pokemon)
                .WithMany(po => po.PokemonOwners)
                .HasForeignKey(po => po.PokemonId);
        }
    }
}
