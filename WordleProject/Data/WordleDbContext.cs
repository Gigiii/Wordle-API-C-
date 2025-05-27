namespace WordleProject.Data
{
    using Microsoft.EntityFrameworkCore;
    using WordleProject.Models;

    public class WordleDbContext : DbContext
    {
        public WordleDbContext(DbContextOptions<WordleDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Guess> Guesses { get; set; }
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Statistic>()
                .HasIndex(s => s.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Statistic)
                .WithOne(s => s.User)
                .HasForeignKey<Statistic>(s => s.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
