using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Person.Api;
using Person.Domain.Entities;
using Testcontainers.MsSql;

namespace Person.Integration.Tests.Base
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<APIAssemblyRefrenceMarker>, IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;
      

        public IntegrationTestWebAppFactory()
        {
            _msSqlContainer = new MsSqlBuilder()
             .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
             .WithPassword("yourStrong(!)Password") // SQL Server requires a strong password
             .WithName("test-sqlserver-container")
             .WithCleanUp(true)
             .Build();
        }
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var serviceDescriptor = services.SingleOrDefault(d => d.ServiceType ==
                    typeof(DbContextOptions<Infrastructure.PersonDbContext>));
                if (serviceDescriptor != null) services.Remove(serviceDescriptor);

                services.AddDbContext<Infrastructure.PersonDbContext>(options =>
                {
                    var constr = _msSqlContainer.GetConnectionString();
                    options.UseSqlServer(constr);
                });
            });
        }

      

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
        
            using (var scope = Services.CreateScope())
            {
                var contextManager = scope.ServiceProvider.GetRequiredService<Infrastructure.PersonDbContext>();
                contextManager.Database.EnsureCreated();
                // Seed PersonTypes  will always be present bc i always seed it
                //The requirement made some assumptions about person types 
                // that made me do this
                //if (!contextManager.PersonTypes.Any())
                //{
                //    contextManager.PersonTypes.AddRange(
                //        new PersonType { PersonTypeId = 1, Description = "Teacher" },
                //        new PersonType { PersonTypeId = 2, Description = "Student" }
                //    );
                //    contextManager.SaveChanges();
                //}
            }
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _msSqlContainer.StopAsync();
        }
 
    }
}
