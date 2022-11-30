using Microsoft.EntityFrameworkCore;
using Eclipse.Model;


namespace Eclipse.Model
{
    public class EclipseDbContext : DbContext
    {
        public EclipseDbContext(DbContextOptions<EclipseDbContext> options) : base(options) { }

        public DbSet<EclipseGame> EclipseGames => Set<EclipseGame>();
    }
}