using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using api_netcore.Data;
using System.IO;

namespace api_netcore
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();

            // Usar la cadena de conexión de Somee.com directamente
            var connectionString = "workstation id=Api_core.mssql.somee.com;packet size=4096;user id=Slim_SQLLogin_1;pwd=ej7ywpu3js;data source=Api_core.mssql.somee.com;persist security info=False;initial catalog=Api_core;TrustServerCertificate=True";

            builder.UseSqlServer(connectionString);

            return new AppDbContext(builder.Options);
        }
    }
}