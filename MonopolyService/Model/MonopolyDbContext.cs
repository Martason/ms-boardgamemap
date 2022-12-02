using Microsoft.EntityFrameworkCore;
using Monopoly.Model;


namespace Monopoly.Model
{
    public class MonopolyDbContext : DbContext
    {
        public MonopolyDbContext(DbContextOptions<MonopolyDbContext> options) : base(options) { }

        public DbSet<MonopolyGame> MonopolyGames => Set<MonopolyGame>();
    }
}