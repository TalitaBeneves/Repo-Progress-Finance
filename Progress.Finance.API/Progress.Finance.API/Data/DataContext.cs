using Microsoft.EntityFrameworkCore;
using API.Model;

namespace API.Data
{
    public class DataContext : DbContext //NOME PADÃO
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Metas> progress { get; set; }

        public DbSet<Items> items { get; set; }
    }
}
