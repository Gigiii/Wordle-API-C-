using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace WordleProject.Data
{
    public class WordleDbContextFactory : IDesignTimeDbContextFactory<WordleDbContext>
    {
        public WordleDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WordleDbContext>();
            optionsBuilder.UseSqlServer("Server=GIGI-DESKTOP;Database=WordleDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new WordleDbContext(optionsBuilder.Options);
        }
    }
}
