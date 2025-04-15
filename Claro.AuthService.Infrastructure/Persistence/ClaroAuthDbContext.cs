using Claro.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claro.AuthService.Infrastructure.Persistence
{
    public class ClaroAuthDbContext : DbContext
    {
        public ClaroAuthDbContext(DbContextOptions<ClaroAuthDbContext> options):base (options) { }
        
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=DESKTOP-8AS3MAS\\SQLEXPRESS01;Database=ClaroAuthDb;Trusted_Connection=True;TrustServerCertificate=True",
        //            b => b.MigrationsAssembly("Claro.AuthService.Infrastructure"));
        //    }
        //}
    }
}
