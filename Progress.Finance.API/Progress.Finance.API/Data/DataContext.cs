using Microsoft.EntityFrameworkCore;
using API.Model;
using Progress.Finance.API.Model;

namespace API.Data
{
    public class DataContext : DbContext //NOME PADÃO
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Usuarios> usuarios { get; set; }
        public DbSet<UsuarioAtivos> usuarioAtivos { get; set; }
        public DbSet<UsuarioMetaInvestimento> usuarioMetaInvestimento { get; set; }
        public DbSet<Ativos> ativos { get; set; }
        public DbSet<MetaInvestimento> metaInvestimento { get; set; }
        public DbSet<Metas> progress { get; set; }
        public DbSet<Items> items { get; set; }
    }
}
