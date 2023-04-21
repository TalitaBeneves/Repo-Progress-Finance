﻿using Microsoft.EntityFrameworkCore;
using API.Model;
using Progress.Finance.API.Model;

namespace API.Data
{
    public class DataContext : DbContext //NOME PADÃO
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Usuarios> usuarios { get; set; }
        public DbSet<Metas> progress { get; set; }
        public DbSet<Items> items { get; set; }
    }
}
