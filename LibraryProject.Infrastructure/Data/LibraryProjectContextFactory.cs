using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Infrastructure.Data
{
    public class LibraryProjectContextFactory:IDesignTimeDbContextFactory<LibraryProjectContext>
    {
        public LibraryProjectContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("C:\\Users\\USER\\source\\repos\\LibraryProject\\LibraryProject.WebAPI\\appsettings.json")
                .Build();

            var services = new ServiceCollection();

            services.ConfigureInfrastructure(configuration);
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<LibraryProjectContext>();
        }
    }
}
