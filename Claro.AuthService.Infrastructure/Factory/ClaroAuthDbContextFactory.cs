using Claro.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claro.AuthService.Infrastructure.Factory
{
    public class ClaroAuthDbContextFactory : IDesignTimeDbContextFactory<ClaroAuthDbContext>
    {
        public ClaroAuthDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ClaroAuthDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new ClaroAuthDbContext(optionsBuilder.Options);
        }
    }
}
