using Microsoft.EntityFrameworkCore;
using AuthVerif.Models;

namespace AuthVerif.Data
{
    public class AppDbContext: DbContext
    {

        protected readonly IConfiguration Configuration;
        internal object AuthTokenVerif;

        /// attribut confifuration
        /// int

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //injection de dépendance
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("bdd"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AuthTokenVerif> AuthTokenVerifs { get; set; }
    }
}
